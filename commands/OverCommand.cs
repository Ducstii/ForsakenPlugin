using CommandSystem;
using Exiled.API.Features;
using PlayerRoles;
using System;
using Exiled.API.Enums;

namespace forsaken.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class OverCommand : ICommand
    {
        public string Command => "over";
        public string[] Aliases => Array.Empty<string>();
        public string Description => "Ends the current game";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                var plugin = ForsakenPlugin.Instance;

                if (plugin.Config.Debug)
                {
                    Log.Debug("[Over] Command execution starting...");
                }

                // Clear items and set players to Tutorial
                Map.CleanAllItems();
                Log.Debug("All items cleared");

                // Stop and dispose of the timer
                plugin.StopTimer();
                Log.Debug("Timer stopped and disposed");

                // Reset facility lights
                Map.TurnOffAllLights(0f); // First turn them off with 0 duration
                Map.TurnOnAllLights(new[] { ZoneType.LightContainment, ZoneType.HeavyContainment, ZoneType.Entrance, ZoneType.Surface }); // Then turn them back on
                Log.Debug("Facility lights reset");

                // Set game state to not running
                plugin.IsGameRunning = false;
                Log.Debug("Game state set to not running");

                // Re-enable handcuffing
                plugin.EnableHandcuffing();
                Log.Debug("Handcuffing re-enabled");

                int playersConverted = 0;
                foreach (var player in Player.List)
                {
                    player.Role.Set(RoleTypeId.Tutorial);
                    playersConverted++;
                }
                Log.Debug($"Converted {playersConverted} players to Tutorial role");

                // Try to reload map using MER
                if (plugin.LoadMap("MiniGame"))
                {
                    Log.Debug("Successfully reloaded MiniGame map");
                    response = "Game ended, lights reset, and map reloaded successfully.";
                    return true;
                }
                
                Log.Debug("Failed to reload map - MapEditorReborn might be missing");
                response = "Game ended but failed to reload map. MapEditorReborn might be missing.";
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"[Over] Error executing command: {ex.Message}");
                response = $"An error occurred while ending the game: {ex.Message}";
                return false;
            }
        }
    }
} 