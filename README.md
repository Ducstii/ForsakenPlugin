# Forsaken Plugin

A custom plugin for SCP:SL servers that adds minigame functionality.

## Features

- Custom map loading with MapEditorReborn integration
- Automated game sequences with countdown timers
- Door management system
- Last Man Standing (LMS) mode
- Configurable game durations and settings

## Dependencies

- EXILED (v8.9.11)
- MapEditorReborn
- PickupsPlugin

## Installation

1. Make sure you have EXILED installed on your SCP:SL server
2. Download the latest release from the releases page
3. Place the `Forsaken.dll` file in your server's `EXILED/Plugins` folder
4. Configure the plugin using the config file in `EXILED/Configs/forsaken.yml`

## Commands

All commands are Remote Admin commands:

| Command | Description | Function |
|---------|-------------|-----------|
| `pregame` | Loads the map and prepares it for the game | - Loads MiniGame map<br>- Manages door states in LCZ<br>- Turns off lights<br>- Teleports players to spawn position<br>- Disables item drops if configured |
| `startlms` | Starts Last Man Standing mode | - Initiates LMS sequence<br>- Plays CASSIE announcement |
| `over` | Ends the current game | - Stops any running game sequences |
| `startgame` | Starts the game with countdown | - Initiates game sequence<br>- Starts countdown timer |
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

### Door Management

The plugin manages several types of doors in Light Containment Zone:

#### Doors that are locked and opened:
- LCZ_173_ARMORY
- LCZ_173_BOTTOM
- GR18
- LCZ_ARMORY
- LCZ_WC

#### Doors that are locked only:
- LCZ_079_ARMORY
- LCZ_079_FIRST
- LCZ_079_SECOND
- 173_CONNECTOR
- LCZ_939
- 914
- 330
- 330_CHAMBER 