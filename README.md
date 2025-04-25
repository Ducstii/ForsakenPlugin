# Forsaken Plugin

A plugin that remakes the idea of "Forsaken" within SCP SL, my second plugin to date.

## Features
- Configurable game durations and settings
- Optimized code, using MEC to prevent game crashes etc.
- 

## Dependencies

- EXILED (v8.9.11)
- MapEditorReborn
- pickups plugin (not necessary, recommended.)

## Installation

1. Make sure you have EXILED installed on your SCP:SL server
2. Download the latest release from the releases page
3. Place the `Forsaken.dll` file in your server's `EXILED/Plugins` folder
4. Configure the plugin using the config file in `EXILED/Configs/forsaken.yml`

## Commands

All commands are Remote Admin commands:

| Command | Description | Function |
|---------|-------------|-----------|
| `pregame` | Prepares the game for all players. | - Loads specified map<br>- Turns off lights<br>- Teleports players to spawn position<br>- Disables item drops if configured |
| `startlms` | Starts Last Man Standing mode | - Initiates LMS sequence<br>- Sets timer to 1:36 |
| `over` | Ends the current game | - Stops any running game sequences |
| `startgame` | Starts the game with countdown | - Initiates game sequence<br>- Starts countdown timer<br>- Opens the 173 connector door. |
| `loadmap` | Loads the MiniGame map | - Loads specified map from config |
| `forsaken` | Main command | - Shows all available subcommands |
| `meme` | Fun command | - Adds some fun to the game |

## Configuration

```yaml
forsaken:
  is_enabled: true
  debug: true
  map_name: MiniGame
  countdown_duration: 10
  game_duration: 360
  lms_duration: 96
  spawn_position: 92,998,1
  show_time_remaining: true
  time_up_message: Times Up! All alive players have been set to Tutorial.
  allow_item_drops: false
  allow_item_pickups: false
```

## Development

This plugin is built using .NET Framework 4.8 and EXILED API.

#### NOTE
For this to function correctly, you need to properly configure your map in this plugins config. This minigame is meant to take place in LCZ only, so it locks down the checkpoints. When the startgame is used, it opens the "173_CONNECTOR" door to let the killer out. Contact me if you need a base minigame map.

## Required manual setup
- Teleport the "Killer" (if not using the requested map from my discord, which automates this.) to the 173 gate, not the connector.
- More will be listed if I find more.

## Contacts
- Discord: https://discord.com/user/1083550921419673610
- Email: Ducstii.MD@gmail.com