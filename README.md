# Very Simple Twitch Chat
A very simple plugin to connect your Unity games to the Twitch chat. It's possible to Login with just one line of code and start reading the messages. Custom settings for different environments and more complex login methods are also supported.

# To add the package to your project
* In Unity, navigate to `Window` -> `PackageManager`
* In the Package Manager Window, click on the `+` sign in the top left corner and click on `Add Package from GIT url`
* Type the repository URL in the text box: `https://github.com/rothiotome/very-simple-twitch-chat.git`

# How to use

Import TwitchChat namespace in each class/script you want to use it:
```CSharp
using TwitchChat
```

## Login using simple anonymous connection
This is the easiest way to use the plugin. You can use ``TwitchController.Login("Channel_name")`` to connect to the channel without needing  a token or any settings customization.

```CSharp
using UnityEngine;
using TwitchChat;

public class TestingChat : MonoBehaviour
{
    string channelName = "RothioTome";
    void Start()
    {
        TwitchController.Login(channelName);
    }
}
```

## Login with customized premade settings
To customize the default settings got  to ``Window`` -> ``Twitch Chat`` -> ``Setup``. This will create a ``defaultChatSettings`` file in your ``Resources`` folder that will be used unless a custom one is specified. 

It's possible to create new custom Twitch settings directly in your project to have multiple profiles. To create a new profile, right click in the Project window -> ``Create`` -> ``TwitchSettings``. You can then use the custom Twitch Settings reference as an argument to the Login method.

```CSharp
using System.Collections.Generic;
using UnityEngine;
using TwitchChat;

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

## Login with OAuth Token
To send messages the Twitch chat, you need writting permissions. Anonymous connection is not allowed, so you need to create a new ``TwitchLoginInfo`` with a token that has writting permissions.

```CSharp
using System.Collections.Generic;
using UnityEngine;
using TwitchChat;

public class TestingChat : MonoBehaviour
{
    string channelName = "RothioTome";

    void Start()
    {
        TwitchController.Login(channelName, new TwitchLoginInfo("Username", "OauthToken"));
    }
}
```
Once you are connection, you can send messages using the ``TwitchController.SendMessage("message")`` method.
## Receiving Chat Messages
To receive the Twitch chat messages, subscribe to the `onTwitchMessageReceived` static delegates.

```CSharp
using System.Collections.Generic;
using UnityEngine;
using TwitchChat;

public class TestingChat : MonoBehaviour
{
    void Start()
    {
        TwitchController.onTwitchMessageReceived += OnMessageReceived;
    }

    private void OnDestroy()
    {
        TwitchController.onTwitchMessageReceived -= OnMessageReceived;
    }

    private void OnMessageReceived(Chatter chatter)
    {
        Debug.Log($"Message received from <color={chatter.tags.colorHex}>{chatter.tags.displayName}</color> : {chatter.message}");
    }
}
```

## Sending Chat Messages
To send chat messages you can use the ``TwitchController.SendChatMessage("message")`` static method. Sending chat messages is only available when you use OAuth connection method with a Token that has writting permissions.


> Note: Sometimes the Channel Joined event occurs a few seconds before the messages start arriving. Please be aware of the delay

# License
This project is released under the MIT License by RothioTome (2023)