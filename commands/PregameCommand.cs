using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MEC;

namespace forsaken.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class PregameCommand : ICommand
    {
        private readonly HashSet<string> doorsToLockAndOpen = new HashSet<string>
        {
            "LCZ_173_ARMORY",
            "LCZ_173_BOTTOM",
            "GR18",
            "LCZ_ARMORY",
            "LCZ_WC"
        };

        private readonly HashSet<string> doorsToLockOnly = new HashSet<string>
        {
            "LCZ_079_ARMORY",
            "LCZ_079_FIRST",
            "LCZ_079_SECOND",
            "173_CONNECTOR",
            "LCZ_939",
            "914",
            "330",
            "330_CHAMBER"
        };

        public string Command => "pregame";
        public string[] Aliases => Array.Empty<string>();
        public string Description => "Loads the map and prepares it for the game";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            // Get a reference to the plugin instance
            var plugin = ForsakenPlugin.Instance;

            if (plugin.Config.Debug)
            {
                Log.Debug("[Pregame] Command execution starting...");
                Log.Debug($"[Pregame] Current map name: {plugin.Config.MapName}");
            }

            try
            {
                // Clean up any existing state
                Map.CleanAllItems();
                Log.Debug("Cleaned up existing items");

                // Unregister any existing event handlers to prevent duplicates
                Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;

                // Load the MiniGame map first
                string mapName = plugin.Config.MapName;
                if (string.IsNullOrEmpty(mapName))
                {
                    Log.Error("Map name is not configured");
                    response = "Map name is not configured in the plugin config.";
                    return false;
                }

                // Try loading the map multiple times if needed
                bool mapLoaded = false;
                int attempts = 0;
                const int maxAttempts = 3;

                while (!mapLoaded && attempts < maxAttempts)
                {
                    attempts++;
                    Log.Debug($"Attempting to load map (attempt {attempts}/{maxAttempts})");
                    
                    if (plugin.LoadMap(mapName))
                    {
                        mapLoaded = true;
                        Log.Debug("MiniGame map loaded successfully");
                        // Give the map a moment to load
                        System.Threading.Thread.Sleep(3000);
                        break;
                    }
                    
                    System.Threading.Thread.Sleep(1000);
                }

                if (!mapLoaded)
                {
                    Log.Error("Failed to load map after multiple attempts");
                    response = "Failed to load MiniGame map. Make sure MapEditorReborn is installed and loaded.";
                    return false;
                }

                // Handle doors in LCZ
                int doorsHandled = 0;
                foreach (var door in Door.List)
                {
                    try
                    {
                        if (door.Zone == ZoneType.LightContainment)
                        {
                            if (doorsToLockAndOpen.Contains(door.Name))
                            {
                                // Lock and open specific doors
                                door.IsOpen = true;
                                door.ChangeLock(DoorLockType.AdminCommand);
                                Log.Debug($"Door {door.Name} locked and opened");
                                doorsHandled++;
                            }
                            else if (doorsToLockOnly.Contains(door.Name))
                            {
                                // Just lock specific doors without opening them
                                door.IsOpen = false;
                                door.ChangeLock(DoorLockType.AdminCommand);
                                Log.Debug($"Door {door.Name} locked and closed");
                                doorsHandled++;
                            }
                            else
                            {
                                // Lock and open all other LCZ doors
                                door.IsOpen = true;
                                door.ChangeLock(DoorLockType.AdminCommand);
                                Log.Debug($"Door {door.Name} locked and opened (default)");
                                doorsHandled++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error handling door {door.Name}: {ex.Message}");
                    }
                }
                Log.Debug($"Handled {doorsHandled} doors in Light Containment Zone");

                // Handle doors with ! nametag
                foreach (Door door in Door.List)
                {
                    if (door?.Nametag != null)
                    {
                        string nametag = door.Nametag.ToString();
                        if (nametag.StartsWith("!"))
                        {
                            try
                            {
                                Log.Debug($"Destroying door with nametag: {nametag}");
                                door.IsOpen = true;  // Force the door open
                                door.ChangeLock(DoorLockType.AdminCommand);  // Lock it permanently
                                Log.Debug($"Door {nametag} destroyed successfully");
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"Failed to destroy door {nametag}: {ex.Message}");
                            }
                        }
                    }
                }

                // Turn off all lights
                Map.TurnOffAllLights(99999f);
                Log.Debug("All lights turned off");

                // Teleport all players to the specified position
                int playersTeleported = 0;
                var spawnCoords = plugin.Config.SpawnPosition.Split(',');
                if (spawnCoords.Length == 3 && 
                    float.TryParse(spawnCoords[0], out float x) && 
                    float.TryParse(spawnCoords[1], out float y) && 
                    float.TryParse(spawnCoords[2], out float z))
                {
                    Timing.CallDelayed(1f, () =>
                    {
                        foreach (var player in Player.List)
                        {
                            player.Position = new Vector3(x, y, z);
                            playersTeleported++;
                        }
                        Log.Debug($"Teleported {playersTeleported} players to spawn position");
                    });
                }
                else
                {
                    Log.Error("Invalid spawn position format in config!");
                }

                // Disable item drops if configured
                if (!plugin.Config.AllowItemDrops)
                {
                    Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
                    Log.Debug("Item drops disabled");
                }

                response = "MiniGame map loaded, specific doors managed, lights turned off, and players teleported.";
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error in pregame command: {ex.Message}");
                response = $"An error occurred while executing the pregame command: {ex.Message}";
                return false;
            }
        }

        private void OnDroppingItem(Exiled.Events.EventArgs.Player.DroppingItemEventArgs ev)
        {
            Log.Debug($"Blocked item drop attempt by {ev.Player.Nickname}");
            ev.IsAllowed = false;
        }
    }
} 