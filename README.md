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

## Commands

- `pregame` - Loads the map and prepares it for the game
- Additional commands coming soon

## Development

This plugin is built using .NET Framework 4.8 and EXILED API. 