# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
