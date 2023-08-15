# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.0] - 2023-08-15

### Added
- MultiChannel join functionallity
    - Join Public Method to join a second channel
    - Chatter.IsFromChannel to check if a message is in one of the channels

### Changed
- Namespace is now called VerySimpleTwitchChat in order to integrate it with other Twitch plugins
- Classes renamed
    - TwitchControllerEditor -> TwitchChatEditor
    - TwitchController -> TwitchChat
    - TwitchSettingsEditor -> TwitchChatSettingsEditor
    - TwitchSettings -> TwitchChatSettings
    - TwitchSettingsWindowEditor -> TwitchChatSettingsWindowEditor

## [1.1.1] - 2023-07-18

### Added
- Extra debug information (writting command queue + formatting)

### Changed
- Now SendCommand method is static

## [1.1.0] - 2023-07-07

### Added
- Added a Chatter class that contains more chatter information
    - login: username. Lowercase and font-safe
    - channel: channel name where the message was sent
    - message: user message
    - tags: array containing the following items
        - badges
        - emotes
        - color
        - display name
        - user Id
        - channel Id
    - GetNameColor: can be normalized
- A new Login overload that allows you to use custom Login Settings

### Changed
- Changed Parsing methods to include all the new information and handle room settings

### Removed
- Removed OnCommandReceived. Now the Chatter class is used, that includes a method to check if the message starts with a given command
