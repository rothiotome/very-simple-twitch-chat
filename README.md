# Very Simple Twitch Chat
A very simple plugin to connect your Unity games to the Twitch chat. It's possible to Login with just one line of code and start reading the messages. Custom settings for different environments and more complex login methods are also supported.

# To add the package to your project
* In Unity, navigate to `Window` -> `PackageManager`
* In the Package Manager Window, click on the `+` sign in the top left corner and click on `Add Package from GIT url`
* Type the repository URL in the text box: `https://github.com/rothiotome/very-simple-twitch-chat.git`

# How to use
## Login using simple anonymous connection
This is the easiest way to use the plugin. You can use ``TwitchController.Login("Channel_name")`` to connect to the channel without needing  a token or any settings customization.

```CSharp
using UnityEngine;

public class TestingChat : MonoBehaviour
{
    string channelName = "RothioTome";
    void Start()
    {
        TwitchController.Login(channelName);
    }
}
```

## Login with customized settings
To customize the default settings got  to ``Window`` -> ``Twitch Chat`` -> ``Setup``. This will create a ``defaultChatSettings`` file in your ``Resources`` folder that will be used unless a custom one is specified. 

It's possible to create new custom Twitch settings directly in your project to have multiple profiles. To create a new profile, right click in the Project window -> ``Create`` -> ``TwitchSettings``. You can then use the custom Twitch Settings reference as an argument to the Login method.

```CSharp
using System.Collections.Generic;
using UnityEngine;

public class TestingChat : MonoBehaviour
{
    [SerializeField] TwitchSettings customSettings;
    string channelName = "RothioTome";

    void Start()
    {
        TwitchController.Login(channelName, customSettings);
    }
}
```

![Chat Settings](readme-screenshot.png)


## Receiving Chat Messages
To receive the Twitch chat messages and commands, subscribe to the `onChannelJoined`, `onTwitchMessageReceived` and `onTwitchCommandReceived` static delegates.

```CSharp
using System.Collections.Generic;
using UnityEngine;

public class TestingChat : MonoBehaviour
{
    void Start()
    {
        TwitchController.onChannelJoined += ChannelJoined;
        TwitchController.onTwitchMessageReceived += OnMessageReceived;
        TwitchController.onTwitchCommandReceived += OnCommandReceived;
    }

    private void OnDestroy()
    {
        TwitchController.onChannelJoined -= ChannelJoined;
        TwitchController.onTwitchMessageReceived -= OnMessageReceived;
        TwitchController.onTwitchCommandReceived -= OnCommandReceived;
    }

    private void ChannelJoined()
    {
        Debug.Log("Channel Joined");
    }

    private void OnMessageReceived(string user, string message)
    {
        Debug.Log($"Message received from {user} : {message}");
    }

    private void OnCommandReceived(string user, string command, List<string> arguments)
    {
        Debug.Log($"Command received from {user} : {command}");
    }
}
```

> Note: Sometimes the Channel Joined event occurs after the first message is received, even if the login was succesfull.

# License
This project is released under the MIT License by RothioTome (2023)