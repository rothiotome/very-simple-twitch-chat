# Very Simple Twitch Chat
A very simple way to connect your Unity Games to the Twitch Chat without any authentication token

## How to add the package to the project

* In Unity, navigate to `Window` -> `PackageManager`
* In the Package Manager Window, click on the `+` sign in the top left corner and click on `Add Package from GIT url`
* Type the repository url in the text field: `https://github.com/rothiotome/very-simple-twitch-chat.git`

## How to use

You have to either copy the `TwitchController` prefab to your scene or create a new GameObject and Add the `TwitchController.cs` component.

You can subscribe to the `onChannelJoined`, `onTwitchMessageReceived` and `onTwitchCommandReceived` delegates to receive the Twitch Chat information.


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

This project has been released under the MIT license by RothioTome (2023)