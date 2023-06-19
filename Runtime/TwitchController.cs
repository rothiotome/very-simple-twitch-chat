using System;
using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using Random = UnityEngine.Random;

public class TwitchController : MonoBehaviour
{
    private static readonly int PORT = 6667;
    private static readonly string SERVER = "irc.chat.twitch.tv";

    private static readonly string PASSWORD = "kappa";
    private static readonly string USERNAME = "justinfan";

    private string currentChannelName = "";
    public bool isConnectedToIRC { get; private set; }
    public bool hasJoinedChannel { get; private set; }

    [SerializeField] private bool autoConnect = false;
    [SerializeField] private float secondsToRetry = 5;
    [SerializeField] private bool debugMode = false;
    [SerializeField] private string defaultChannelName = "";

    #region SINGLETON

    private static TwitchController instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        if (autoConnect)
        {
            Login(defaultChannelName);
        }
    }

    public static TwitchController Instance => instance;

    #endregion
    
    public delegate void OnTwitchCommandReceived(string user, string command, List<string> arguments);
    public static OnTwitchCommandReceived onTwitchCommandReceived;

    public delegate void OnTwitchMessageReceived(string user, string message);
    public static OnTwitchMessageReceived onTwitchMessageReceived;

    public delegate void OnChannelJoined();
    public static OnChannelJoined onChannelJoined;

    private TcpClient sock;
    private StreamReader input;
    private StreamWriter output;
    private NetworkStream networkStream;
    private List<string> recievedMsgs = new();
    private Queue<string> commandQueue = new();

    private float timer;
    private float retryTimer;


    private void Update()
    {
        if (networkStream == null) return;

        IRCInputProcedure();
        IRCOutputProcedure();

        if (recievedMsgs.Count > 0)
        {
            for (int i = 0; i < recievedMsgs.Count; i++)
            {
                ParseChatMessage(recievedMsgs[i]);
            }

            recievedMsgs.Clear();
        }
    }

    public void Login(string channel)
    {
        int startIndex = channel.IndexOf(".tv/") == -1 ? 0 : channel.IndexOf(".tv/") + 4;
        channel = channel.TrimEnd('/').Trim();
        string trimmedChannelName = channel.Substring(startIndex, channel.Length - startIndex);
        currentChannelName = trimmedChannelName;
        StartIRC();
    }

    private void StartIRC()
    {
        retryTimer = 0;

        if (sock != null)
        {
            input.Close();
            output.Close();
            networkStream.Close();
            sock.Close();
        }

        sock = new TcpClient();

        sock.Connect(SERVER, PORT);

        networkStream = sock.GetStream();
        input = new StreamReader(networkStream);
        output = new StreamWriter(networkStream);

        output.WriteLine("PASS " + PASSWORD);
        output.WriteLine(
            $"NICK {USERNAME}{Random.Range(1, 10)}{Random.Range(1, 10)}{Random.Range(1, 10)}{Random.Range(1, 10)}");
        output.Flush();
    }

    private void IRCInputProcedure()
    {
        if (!isConnectedToIRC)
        {
            retryTimer += Time.deltaTime;
            if (retryTimer > secondsToRetry)
            {
                retryTimer = 0;
#if UNITY_EDITOR
                Debug.Log("Retry");
#endif
                StartIRC();
                return;
            }
        }

        if (!networkStream.DataAvailable)
            return;

        string buffer = input.ReadLine();
#if UNITY_EDITOR
        if (debugMode) Debug.Log(buffer);
#endif
        //was message?
        if (buffer.Contains("PRIVMSG #"))
        {
            recievedMsgs.Add(buffer);
        }

        //Send pong reply to any ping messages
        if (buffer.StartsWith("PING "))
        {
            SendCommand(buffer.Replace("PING", "PONG"));
        }

        //After server sends 001 command, we can join a channel
        if (!isConnectedToIRC && buffer.Split(' ')[1] == "001")
        {
            SendCommand("JOIN #" + currentChannelName.ToLower());
            isConnectedToIRC = true;
        }

        //After server sends 353 command, the channel has successfully joined
        if (!hasJoinedChannel && buffer.Split(' ')[1] == "353")
        {
            hasJoinedChannel = true;
            onChannelJoined?.Invoke();
        }
    }

    private void IRCOutputProcedure()
    {
        timer += Time.deltaTime;

        if (commandQueue.Count > 0) //do we have any commands to send?
        {
            //have enough time passed since we last sent a message/command?
            if (timer > 1.750f)
            {
                //send msg.
                output.WriteLine(commandQueue.Peek());
                output.Flush();
                //remove msg from queue.
                commandQueue.Dequeue();
                //restart stopwatch.
                timer = 0;
            }
        }
    }

    private void SendCommand(string cmd)
    {
        lock (commandQueue)
        {
            commandQueue.Enqueue(cmd);
        }
    }

    private void ParseChatMessage(string msg)
    {
        int msgIndex = msg.IndexOf("PRIVMSG #");
        string msgString = msg.Substring(msgIndex + currentChannelName.Length + 11);
        string user = msg.Substring(1, msg.IndexOf('!') - 1);
        if (msgString.Length > 0)
        {
            if (msgString[0].Equals('!'))
            {
                List<string> arguments = new List<string>(msgString.Split(' '));
                string command = arguments[0].Substring(1);
                arguments.RemoveAt(0);
                onTwitchCommandReceived?.Invoke(user, command.ToLower(), arguments);
            }
            else
            {
                onTwitchMessageReceived?.Invoke(user, msgString);
            }
        }
    }
}