using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using PlayerRoles;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using forsaken.Commands;
using forsaken.Events;
using MEC;

namespace forsaken
{
    public class ForsakenPlugin : Plugin<Config>
    {
        public override string Author => "Ducstii";
        public override string Name => "Forsaken";
        public override Version Version => new Version(1, 0, 0);
        public override PluginPriority Priority => PluginPriority.Medium;

        private static ForsakenPlugin Singleton;
        public static ForsakenPlugin Instance { get; private set; }
        private CoroutineHandle timerCoroutine;
        private int countdownTime;
        private PlayerEvents playerEvents;
        private bool isGameRunning;

        public bool IsGameRunning => isGameRunning;

        private string FindPluginDll(string pluginName)
        {
            // Common paths where plugins might be located
            var paths = new[]
            {
                Path.Combine(Paths.Plugins, $"{pluginName}.dll"),
                Path.Combine(Paths.Plugins, pluginName, $"{pluginName}.dll"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED", "Plugins", $"{pluginName}.dll")
            };

            foreach (var path in paths)
            {
                if (File.Exists(path))
                    return path;
            }

            return null;
        }

        public override void OnEnabled()
        {
            Instance = this;
            Singleton = this;
            playerEvents = new PlayerEvents();
            isGameRunning = false;
            
            if (Config.Debug)
            {
                Log.Debug("[Forsaken] Plugin is being enabled...");
            
            RegisterEvents();
            RegisterCommands();
            
            if (Config.Debug)
            {
                Log.Debug("[Forsaken] Plugin enabled successfully!");
            }
        }

        private void RegisterEvents()
        {
            if (Config.Debug)
            {
                Log.Debug("[Forsaken] Registering events...");
            }
                
            // Create plugin directory if it doesn't exist
            string pluginDir = Path.Combine(Paths.Configs, "forsaken");
            if (!Directory.Exists(pluginDir))
            {
                Directory.CreateDirectory(pluginDir);
                Log.Info($"Created plugin directory at {pluginDir}");
            }

            // Create config file if it doesn't exist
            string configPath = Path.Combine(pluginDir, "config.yml");
            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, GenerateDefaultConfig());
                Log.Info($"Created default config file at {configPath}");
            }

            // Register item interaction events based on config
            if (!Config.AllowItemDrops)
            {
                Exiled.Events.Handlers.Player.DroppingItem += playerEvents.OnDroppingItem;
                if (Config.Debug)
                {
                    Log.Debug("[Forsaken] Registered item drop prevention");
                }
            }

            if (!Config.AllowItemPickups)
            {
                Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
                if (Config.Debug)
                {
                    Log.Debug("[Forsaken] Registered item pickup prevention");
                }
            }

            // Check for required plugins
            var missingDependencies = new List<string>();

            // Check for MapEditorReborn
            var merPath = FindPluginDll("MapEditorReborn");
            if (merPath == null)
            {
                missingDependencies.Add("MapEditorReborn.dll");
            }

            // Check for pickupsplugin
            var pickupsPath = FindPluginDll("pickupsplugin");
            if (pickupsPath == null)
            {
                missingDependencies.Add("pickupsplugin.dll");
            }

            // Report any missing dependencies
            if (missingDependencies.Any())
            {
                Exiled.API.Features.Log.Warn($"Missing or invalid dependencies: {string.Join(", ", missingDependencies)}");
                Exiled.API.Features.Log.Warn("Some features may not work without these dependencies!");
            }
            
            if (Config.Debug)
                Log.Debug("[Forsaken] Events registered successfully");
        }

        private void RegisterCommands()
        {
            if (Config.Debug)
            {
                Log.Debug("[Forsaken] Registering commands...");
            }
            // Register your commands here
        }

        public override void OnDisabled()
        {
            if (Config.Debug)
            {
                Log.Debug("[Forsaken] Plugin is being disabled...");
            }

            // Unregister item interaction events
            Exiled.Events.Handlers.Player.DroppingItem -= playerEvents.OnDroppingItem;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpItem;

            // Stop any running coroutines
            if (timerCoroutine.IsRunning)
            {
                Timing.KillCoroutines(timerCoroutine);
            }

            Instance = null;
            Singleton = null;
            
            if (Config.Debug)
            {
                Log.Debug("[Forsaken] Plugin disabled successfully!");
            }
        }

        public override void OnReloaded()
        {
            if (Config.Debug)
            {
                Log.Debug("[Forsaken] Plugin is being reloaded...");
            }

            // Clean up any existing state
            if (timerCoroutine.IsRunning)
            {
                Timing.KillCoroutines(timerCoroutine);
                timerCoroutine = default;
            }

            if (Config.Debug)
            {
                Log.Debug("[Forsaken] Plugin reloaded successfully!");
            }
        }

        public IEnumerator<float> LoadMapCoroutine(string mapName)
        {
            if (Config.Debug)
            {
                Log.Debug($"[Forsaken:LoadMap] Starting map load process");
                Log.Debug($"[Forsaken:LoadMap] Map name: {mapName}");
                
                string mapPath = Path.Combine(Paths.Configs, "MapEditorReborn", "Maps", $"{mapName}.yml");
                Log.Debug($"[Forsaken:LoadMap] Full map path: {mapPath}");
                
                string mapDirectory = Path.Combine(Paths.Configs, "MapEditorReborn", "Maps");
                if (!Directory.Exists(mapDirectory))
                {
                    Log.Debug($"[Forsaken:LoadMap] Map directory does not exist: {mapDirectory}");
                    Directory.CreateDirectory(mapDirectory);
                    Log.Debug("[Forsaken:LoadMap] Created map directory");
                }
            }

            try
            {
                MapEditorReborn.API.Features.MapUtils.LoadMap(mapName);
                
                if (Config.Debug)
                {
                    Log.Debug("[Forsaken:LoadMap] Map load command executed");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[Forsaken:LoadMap] Error loading map: {ex.Message}");
                if (Config.Debug)
                {
                    Log.Debug($"[Forsaken:LoadMap] Full exception details:");
                    Log.Debug($"[Forsaken:LoadMap] Stack trace: {ex.StackTrace}");
                }
                yield break;
            }

            // Wait for map to load
            yield return Timing.WaitForSeconds(2f);

            if (Config.Debug)
            {
                Log.Debug("[Forsaken:LoadMap] Map load completed");
            }
        }

        public bool LoadMap(string mapName)
        {
            try
            {
                // Start the coroutine and wait for it to complete
                Timing.RunCoroutine(LoadMapCoroutine(mapName));
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"[Forsaken:LoadMap] Error starting map load: {ex.Message}");
                return false;
            }
        }

        public void StartGameSequence()
        {
            if (isGameRunning)
            {
                Log.Error("[Forsaken] Attempted to start game while another game is in progress!");
                return;
            }

            if (Config.Debug)
            {
                Log.Debug("[Forsaken] Starting game sequence...");
            }

            isGameRunning = true;

            // Ensure item interactions are properly restricted based on config
            Exiled.Events.Handlers.Player.DroppingItem -= playerEvents.OnDroppingItem;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpItem;

            if (!Config.AllowItemDrops)
            {
                Exiled.Events.Handlers.Player.DroppingItem += playerEvents.OnDroppingItem;
                if (Config.Debug)
                {
                    Log.Debug("[Forsaken] Re-registered item drop prevention for game sequence");
                }
            }

            if (!Config.AllowItemPickups)
            {
                Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
                if (Config.Debug)
                {
                    Log.Debug("[Forsaken] Re-registered item pickup prevention for game sequence");
                }
            }

            // Turn off all lights at the start
            Map.TurnOffAllLights(99999f);
            if (Config.Debug)
            {
                Log.Debug("[Forsaken] Lights turned off for game sequence");
            }

            // Play the CASSIE countdown with exact spacing
            Cassie.Message("<color=red> 5 . 4 . 3 . 2 . 1 . HIDE </color>", isSubtitles: true);

            // Start the sequence coroutine
            timerCoroutine = Timing.RunCoroutine(GameSequenceCoroutine());

            if (Config.Debug)
            {
                Log.Debug("[Forsaken] Game sequence started");
            }
        }

        private IEnumerator<float> GameSequenceCoroutine()
        {
            // Wait for CASSIE to finish (approximately 10 seconds)
            yield return Timing.WaitForSeconds(10f);

            // Open and lock the 173 connector door after CASSIE finishes
            try
            {
                foreach (var door in Door.List)
                {
                    if (door.Name == "173_CONNECTOR")
                    {
                        // Lock the door with admin command and force it open
                        door.ChangeLock(DoorLockType.AdminCommand);
                        door.IsOpen = true;
                        
                        if (Config.Debug)
                        {
                            Log.Debug("[Forsaken] 173 connector door opened and locked");
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error handling 173 connector door: {ex.Message}");
                if (Config.Debug)
                {
                    Log.Debug($"[Forsaken] Full exception details: {ex.StackTrace}");
                }
            }

            // Start timer based on config
            countdownTime = Config.GameDuration;
            StopTimer(); // Stop any existing timer
            timerCoroutine = Timing.RunCoroutine(GameTimer());
        }

        private IEnumerator<float> GameTimer()
        {
            while (countdownTime > 0)
            {
                if (Config.ShowTimeRemaining)
                {
                    Map.Broadcast(1, $"Time remaining: {countdownTime / 60:D2}:{countdownTime % 60:D2}");
                }
                countdownTime--;
                yield return Timing.WaitForSeconds(1f);
            }

            // Time's up - handle game end
            foreach (var player in Player.List.Where(p => p.IsAlive))
            {
                player.Role.Set(RoleTypeId.Tutorial);
            }
            Map.Broadcast(5, Config.TimeUpMessage);
            
            // Turn the lights back on
            Map.TurnOffAllLights(0f);
            if (Config.Debug)
            {
                Log.Debug("[Forsaken] Game ended - lights turned back on");
            }
            
            StopTimer();
            isGameRunning = false;
        }

        public void StopTimer()
        {
            if (timerCoroutine.IsRunning)
            {
                Timing.KillCoroutines(timerCoroutine);
            }
            isGameRunning = false;
        }

        public void StartLmsSequence(CommandSender sender)
        {
            countdownTime = Config.LmsDuration;
            StopTimer(); // Stop any existing timer
            timerCoroutine = Timing.RunCoroutine(GameTimer());
        }

        private string GenerateDefaultConfig()
        {
            return @"forsaken:
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
  allow_item_pickups: false";
        }

        // Item interaction event handlers
        private void OnDroppingItem(Exiled.Events.EventArgs.Player.DroppingItemEventArgs ev)
        {
            if (Config.Debug)
            {
                Log.Debug($"[Forsaken] Preventing item drop for {ev.Player.Nickname}");
            }
            ev.IsAllowed = false;
        }

        private void OnPickingUpItem(Exiled.Events.EventArgs.Player.PickingUpItemEventArgs ev)
        {
            if (Config.Debug)
            {
                Log.Debug($"[Forsaken] Preventing item pickup for {ev.Player.Nickname}");
            }
            ev.IsAllowed = false;
        }
    }
} 