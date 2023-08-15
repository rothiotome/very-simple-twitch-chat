using UnityEngine;

[CreateAssetMenu(menuName = "Twitch Chat Settings", fileName = "TwitchChatSettings", order = 0)]
public class TwitchChatSettings : ScriptableObject
{
    public float secondsToRetry = 5;
    public int maxRetry = 10;
    public bool debugMode = false;
    public bool preserveAcrossScenes = true;

    public bool useAnonymousConnection = true;
    public string username;
    public string oAuthToken;

    public int port = 6667;
    public string server = "irc.chat.twitch.tv";
    public string anonUsername = "justinfan5555";
    public string anonPassword = "kappa";
    public bool useRandomColorForUndefined = true;

    public TwitchLoginInfo GetLoginInfo()
    {
        return useAnonymousConnection
            ? new TwitchLoginInfo(anonUsername, anonPassword)
            : new TwitchLoginInfo(username, oAuthToken.StartsWith("oauth:") ? oAuthToken : $"oauth:{oAuthToken}");
    }
}

public struct TwitchLoginInfo
{
    public string user;
    public string password;

    public TwitchLoginInfo(string username, string pass)
    {
        user = username;
        password = pass;
    }
}