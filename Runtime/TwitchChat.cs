using System;
using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;

namespace VerySimpleTwitchChat
{
    [AddComponentMenu("")]
    public class TwitchChat : MonoBehaviour
    {
        private string currentMainChannelName = "";

        private List<string> secondaryChannelNames = new List<string>();
        public bool isConnectedToIRC { get; private set; }
        public bool hasJoinedChannel { get; private set; }

        private TwitchChatSettings settings;

        private IRCTags channelTags;

        #region SINGLETON

        private static TwitchChat instance;

        private void Awake()
        {
            if (settings != null && settings.preserveAcrossScenes)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        #endregion

        public delegate void OnTwitchMessageReceived(Chatter chatter);

        public static OnTwitchMessageReceived onTwitchMessageReceived;

        public delegate void OnChannelTagsReceived(IRCTags tags);

        public static OnChannelTagsReceived onChannelTagsReceived;

        public delegate void OnChannelJoined();

        public static OnChannelJoined onChannelJoined;

        private TcpClient sock;
        private StreamReader input;
        private StreamWriter output;
        private NetworkStream networkStream;
        private List<string> recievedMsgs = new List<string>();
        private Queue<string> commandQueue = new Queue<string>();

        private float timer;
        private float retryTimer;

        private int connectionTries;

        private readonly int sessionRandom = DateTime.Now.Second;

        public static void Ping() => SendCommand("PING :tmi.twitch.tv");
        private static void Pong() => SendCommand("PONG :tmi.twitch.tv");

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

        public static void Login(string channel)
        {
            // Attempt to load the TwitchChatSettings asset from Resources folder
            TwitchChatSettings defaultTwitchChatSettings =
                Resources.Load<TwitchChatSettings>("defaultTwitchChatSettings");

            // If the asset doesn't exist, create a new
            if (defaultTwitchChatSettings == null)
            {
                defaultTwitchChatSettings = ScriptableObject.CreateInstance<TwitchChatSettings>();
                // Optionally, you can initialize defaultTwitchChatSettings with default values
                // defaultTwitchChatSettings.InitializeDefaultValues();
            }

            UpdateTwitchChatInstance(TrimChannelName(channel), defaultTwitchChatSettings);
        }

        public static void Login(string channel, TwitchChatSettings settings)
        {
            UpdateTwitchChatInstance(TrimChannelName(channel), settings);
        }

        public static void Login(string channel, TwitchLoginInfo loginInfo)
        {
            //TwitchChatSettings defaultTwitchChatSettings = Resources.Load<TwitchChatSettings>("defaultTwitchChatSettings");
            TwitchChatSettings defaultTwitchChatSettings =
                Resources.Load<TwitchChatSettings>("defaultTwitchChatSettings");

            // If the asset doesn't exist, create a new
            if (defaultTwitchChatSettings == null)
            {
                defaultTwitchChatSettings = ScriptableObject.CreateInstance<TwitchChatSettings>();
                // Optionally, you can initialize defaultTwitchChatSettings with default values
                // defaultTwitchChatSettings.InitializeDefaultValues();
            }

            defaultTwitchChatSettings.useAnonymousConnection = false;
            defaultTwitchChatSettings.oAuthToken = loginInfo.password;
            defaultTwitchChatSettings.username = loginInfo.user;

            UpdateTwitchChatInstance(TrimChannelName(channel), defaultTwitchChatSettings);
        }

        private static string TrimChannelName(string channel)
        {
            int startIndex = channel.IndexOf(".tv/") == -1 ? 0 : channel.IndexOf(".tv/") + 4;
            channel = channel.TrimEnd('/').Trim();
            return channel.Substring(startIndex, channel.Length - startIndex);
        }

        private static void UpdateTwitchChatInstance(string channelName, TwitchChatSettings twitchChatSettings)
        {
            if (instance == null)
            {
                GameObject go = new GameObject("TwitchChat");
                instance = go.AddComponent<TwitchChat>();
            }

            instance.currentMainChannelName = channelName;
            instance.settings = twitchChatSettings;
            instance.StartIRC();
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

            sock.Connect(settings.server, settings.port);

            networkStream = sock.GetStream();
            input = new StreamReader(networkStream);
            output = new StreamWriter(networkStream);

            TwitchLoginInfo loginInfo = settings.GetLoginInfo();

            SendCommand($"PASS {loginInfo.password.ToLower()}");
            SendCommand($"NICK {loginInfo.user.ToLower()}");
            SendCommand("CAP REQ :twitch.tv/tags twitch.tv/commands");
        }

        private void IRCInputProcedure()
        {
            if (!isConnectedToIRC)
            {
                retryTimer += Time.deltaTime;
                if (retryTimer > settings.secondsToRetry)
                {
                    retryTimer = 0;
                    connectionTries++;
                    if (connectionTries >= settings.maxRetry)
                    {
                        Debug.Log($"Can't connect to Twitch after {connectionTries} tries. Destroying Controller");
                        Destroy(gameObject);
                    }
                    else
                    {
                        StartIRC();
                        return;
                    }
                }
            }

            if (!networkStream.DataAvailable)
                return;

            string buffer = input.ReadLine();
#if UNITY_EDITOR
            if (settings.debugMode) Debug.Log($"<color=blue>-></color> {buffer}");
#endif

            recievedMsgs.Add(buffer);
        }

        private void IRCOutputProcedure()
        {
            timer += Time.deltaTime;

            if (commandQueue.Count > 0) //do we have any commands to send?
            {
                //have enough time passed since we last sent a message/command?
                if (timer > 1.750f)
                {
#if UNITY_EDITOR
                    if (settings.debugMode) Debug.Log($"<color=green><-</color> {commandQueue.Peek()}");
#endif

                    //send msg.
                    output.WriteLine(commandQueue.Dequeue());
                    output.Flush();
                    //restart stopwatch.
                    timer = 0;
                }
            }
        }

        private void ParseChatMessage(string msg)
        {
            string ircString = msg;
            string tagString = string.Empty;

            // Parsing the raw IRC lines...

            if (msg[0] == '@')
            {
                int ind = msg.IndexOf(' ');
                tagString = msg.Substring(0, ind);
                ircString = msg.Substring(ind).TrimStart();
            }

            if (ircString[0] == ':')
            {
                string type = ircString.Substring(ircString.IndexOf(' ')).TrimStart();
                type = type.Substring(0, type.IndexOf(' '));

                switch (type)
                {
                    case "PRIVMSG": // = Chat message
                        HandlePRIVMSG(ircString, tagString);
                        break;
                    case "USERSTATE": // = Userstate
                        HandleUSERSTATE(ircString, tagString);
                        break;
                    case "NOTICE": // = Notice
                        HandleNOTICE(ircString, tagString);
                        break;
                    case "ROOMSTATE": // RoomState
                        HandleROOMSTATE(ircString, tagString);
                        break;

                    // RPL messages
                    case "353": // = Successful channel join
                    case "001": // = Successful IRC connection
                        HandleRPL(type);
                        break;
                }
            }

            // Respond to PING messages with PONG
            if (msg.StartsWith("PING"))
                Pong();
        }

        private static void SendCommand(string cmd)
        {
            lock (instance.commandQueue)
            {
                instance.commandQueue.Enqueue(cmd);
            }
        }

        public static void JoinChannel(string channelName)
        {
            if (instance == null)
            {
                Debug.LogWarning(
                    $"Can't join the channel {channelName}: To join a secondary channel(s), you must login your primary channel first!");
                return;
            }

            instance.secondaryChannelNames.Add(channelName);
            SendCommand("JOIN #" + channelName);
        }

        public static bool LeaveChannel(string channelName)
        {
            if (instance == null)
            {
                Debug.LogWarning($"Can't leave a channel because you didn't join any channels yet");
                return false;
            }

            if (instance.secondaryChannelNames.Remove(channelName))
            {
                SendCommand("PART #" + channelName);
                return true;
            }

            Debug.LogWarning($"Can't leave {channelName} because you were not in that channel!");
            return false;
        }

        public static void SendChatMessage(string message, string channelName = null)
        {
            if (message.Length <= 0)
            {
                Debug.LogWarning($"{Tags.write} Tried sending an empty chat message");
                return;
            }

            if (instance == null)
            {
                Debug.LogWarning($"Can't send a message before the login!");
                return;
            }

            if (instance.settings.useAnonymousConnection)
            {
                Debug.LogWarning(
                    "You can't send messages using anonymous connection. Please use a OAuth Token with writing permissions instead");
                return;
            }

            if (!String.IsNullOrEmpty(channelName) && channelName.Equals(instance.currentMainChannelName) &&
                !instance.secondaryChannelNames.Contains(channelName))
            {
                Debug.LogWarning($"You must join the channel {channelName} first to send messages!");
                return;
            }

            // Place the chat message into the write queue
            SendCommand(
                $"PRIVMSG #{(String.IsNullOrEmpty(channelName) ? instance.currentMainChannelName : channelName.ToLower())} :{message}");
        }

        #region Response Handlers

        private void HandlePRIVMSG(string ircString, string tagString)
        {
            var login = ParseHelper.ParseLoginName(ircString);
            var channel = ParseHelper.ParseChannel(ircString);
            var message = ParseHelper.ParseMessage(ircString);
            var tags = ParseHelper.ParseTags(tagString);

            // Not all users have set their Twitch name color, so we need to check for that
            if (tags.colorHex.Length <= 0)
                tags.colorHex = settings.useRandomColorForUndefined
                    ? ChatColors.GetRandomNameColor(sessionRandom, login)
                    : "#FFFFFF";

            // Sort emotes by startIndex to match emote order in the actual chat message
            if (tags.emotes.Length > 0)
            {
                Array.Sort(tags.emotes, (a, b) =>
                    a.indexes[0].startIndex.CompareTo(b.indexes[0].startIndex));
            }

            // Queue new chatter object
            onTwitchMessageReceived?.Invoke(new Chatter(login, channel, message, tags));
        }

        private void HandleUSERSTATE(string ircString, string tagString)
        {
            IRCTags tags = ParseHelper.ParseTags(tagString);
        }

        private void HandleNOTICE(string ircString, string tagString)
        {
            if (ircString.Contains(":Login authentication failed"))
            {
                Debug.Log("Login authentication failed");
            }
        }

        private void HandleROOMSTATE(string ircString, string tagString)
        {
            channelTags = ParseHelper.ParseTags(tagString);
            onChannelTagsReceived?.Invoke(channelTags);
        }

        private void HandleRPL(string type)
        {
            switch (type)
            {
                case "001":
                    JoinChannel(currentMainChannelName.ToLower());
                    isConnectedToIRC = true;

                    hasJoinedChannel = true;
                    onChannelJoined?.Invoke();
                    break;
                case "353":

                    break;
            }
        }

        #endregion
    }
}