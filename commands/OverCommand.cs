using CommandSystem;
using Exiled.API.Features;
using PlayerRoles;
using System;
using Exiled.API.Enums;

namespace forsaken.Commands
{
    public class OverCommand : ICommand
    {
        public string Command => "over";
        public string[] Aliases => Array.Empty<string>();
        public string Description => "Ends the current game";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Log.Debug("Over command executing...");

            // Clear items and set players to Tutorial
            Map.CleanAllItems();
            Log.Debug("All items cleared");

            // Stop and dispose of the timer
            var plugin = ForsakenPlugin.Instance;
            plugin.StopTimer();
            Log.Debug("Timer stopped and disposed");

            // Reset facility lights
            Map.TurnOffAllLights(0f); // First turn them off with 0 duration
            Map.TurnOnAllLights(new[] { ZoneType.LightContainment, ZoneType.HeavyContainment, ZoneType.Entrance, ZoneType.Surface }); // Then turn them back on
            Log.Debug("Facility lights reset");

            int playersConverted = 0;
            foreach (var player in Player.List)
            {
                player.Role.Set(RoleTypeId.Tutorial);
                playersConverted++;
            }
            Log.Debug($"Converted {playersConverted} players to Tutorial role");

            // Try to reload map using MER
            if (ForsakenPlugin.Instance.LoadMap("MiniGame"))
            {
                Log.Debug("Successfully reloaded MiniGame map");
                response = "Game ended, lights reset, and map reloaded successfully.";
                return true;
            }
            
            Log.Debug("Failed to reload map - MapEditorReborn might be missing");
            response = "MapEditorReborn is not available or does not support map loading. Please ensure MER is installed and loaded.";
            return false;
        }
    }
} 