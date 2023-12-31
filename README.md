![Last release: 1.2.1](https://img.shields.io/badge/release-v1.2.1-blue.png) 
![Buy in Asset Store](https://img.shields.io/badge/Asset_Store-Soon-FA5c5c?logo=unity&logoColor=FFFFFF&)
[![Made for Unity](https://img.shields.io/badge/Unity-2017+-blue?logo=unity&logoColor=white)](https://unity.com)
[![Under MIT license](https://img.shields.io/github/license/RothioTome/very-simple-twitch-chat)](LICENSE)

# Very Simple Twitch Chat
A very simple plugin to connect your Unity games to the Twitch chat. It's possible to Login with just one line of code and start reading the messages. Custom settings for different environments and more complex login methods are also supported.

## Table of contents
- [How to install](#how-to-install)
- [How to use](#how-to-use)
    - [Login](#how-to-login)
        - [Simple annonymous connection](#simple-anonymous-connection)
        - [Customized premade settings](#customized-premade-settings)
        - [OAuth Token](#oauth-token)
    - [Receive chat messages](#how-to-receive-chat-messages)
    - [Send chat messages](#how-to-send-chat-messages)
    - [Join a second Twitch Channel](#join-a-second-twitch-channel)
- [FAQ and Troubleshooting](#faq-and-troubleshooting)
    - [Git error adding package](#error-adding-package)
- [License](#license)

## How to install
* In Unity, navigate to `Window` -> `PackageManager`
* In the Package Manager Window, click on the `+` sign in the top left corner and click on `Add Package from GIT url`
* Type the repository URL in the text box: `https://github.com/rothiotome/very-simple-twitch-chat.git`

## How to use
Import VerySimpleTwitchChat namespace in each class/script you want to use it:
```CSharp
using VerySimpleTwitchChat
```

## How to Login
### Simple anonymous connection
This is the easiest way to use the plugin. You can use ``TwitchChat.Login("Channel_name")`` to connect to the channel without needing  a token or any settings customization.

```CSharp
using UnityEngine;
using VerySimpleTwitchChat;

public class TestingChat : MonoBehaviour
{
    string channelName = "RothioTome";
    void Start()
    {
        TwitchChat.Login(channelName);
    }
}
```

### Customized premade settings
To customize the default settings got  to ``Window`` -> ``Twitch Chat`` -> ``Setup``. This will create a ``defaultTwitchChatSettings`` file in your ``Resources`` folder that will be used unless a custom one is specified. 

It's possible to create new custom Twitch settings directly in your project to have multiple profiles. To create a new profile, right click in the Project window -> ``Create`` -> ``TwitchChatSettings``. You can then use the custom Twitch Settings reference as an argument to the Login method.

```CSharp
using System.Collections.Generic;
using UnityEngine;
using VerySimpleTwitchChat;

public class TestingChat : MonoBehaviour
{
    [SerializeField] TwitchChatSettings customSettings;
    string channelName = "RothioTome";

    void Start()
    {
        TwitchChat.Login(channelName, customSettings);
    }
}
```

![Chat Settings](readme-screenshot.png)

### OAuth Token
To send messages the Twitch chat, you need writting permissions. Anonymous connection is not allowed, so you need to create a new ``TwitchLoginInfo`` with a token that has writting permissions.

```CSharp
using System.Collections.Generic;
using UnityEngine;
using VerySimpleTwitchChat;

public class TestingChat : MonoBehaviour
{
    string channelName = "RothioTome";

    void Start()
    {
        TwitchChat.Login(channelName, new TwitchLoginInfo("Username", "OauthToken"));
    }
}
```
Once you are connection, you can send messages using the ``TwitchChat.SendChatMessage("message")`` static method.

## How to receive chat messages
To receive the Twitch chat messages, subscribe to the `onTwitchMessageReceived` static delegates.

```CSharp
using System.Collections.Generic;
using UnityEngine;
using VerySimpleTwitchChat;

public class TestingChat : MonoBehaviour
{
    void Start()
    {
        TwitchChat.onTwitchMessageReceived += OnMessageReceived;
    }

    private void OnDestroy()
    {
        TwitchChat.onTwitchMessageReceived -= OnMessageReceived;
    }

    private void OnMessageReceived(Chatter chatter)
    {
        Debug.Log($"Message received from <color={chatter.tags.colorHex}>{chatter.tags.displayName}</color> : {chatter.message}");
    }
}
```

## How to send chat messages
To send chat messages you can use the ``TwitchChat.SendChatMessage("message")`` static method. Sending chat messages is only available when you use OAuth connection method with a Token that has writting permissions.

## Join a Second Twitch Channel
Very Simple Twitch Chat will automatically join the channel you use in the login method. However, this plugin supports multi channel join, so you will be able to Join a second channel (and virtually infinite of them) by using the ``TwitchChat.JoinChannel("channelName")`` method. To know if a message has been received in your main channel or another one, you can use the method ```Chatter.IsFromChannel("channelName")```. To leave a channel you've previously joined, you can use the method ``TwitchChat.LeaveChannel("channelName)``.

> Note: Sometimes the Channel Joined event occurs a few seconds before the messages start arriving. Please be aware of the delay

## FAQ and Troubleshooting
### Error adding package: 
```Error adding package: https://github.com/rothiotome/very-simple-twitch-chat.git. Unable to add package```
![Git Error](git-error.png)

A GIT client installed on your computer is required to add a Unity package using the Git address. You can download the Git client from [git-scm.com](https://git-scm.com/downloads).

If you don't want to install the Git client you can clone/download the repository to a specific folder and then add that folder to the package manager.

## License
This project is released under the MIT License by RothioTome (2023)