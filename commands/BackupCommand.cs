using CommandSystem;
using System;
using Exiled.API.Features;

namespace forsaken.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class BackupCommand : ICommand
    {
        public string Command => "backup";
        public string[] Aliases => Array.Empty<string>();
        public string Description => "Emergency stop - cancels any running timer";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                var plugin = ForsakenPlugin.Instance;

                if (plugin.Config.Debug)
                {
                    Log.Debug("[Backup] Command execution starting...");
                }

                // Stop any running timer
                plugin.StopTimer();

                if (plugin.Config.Debug)
                {
                    Log.Debug("[Backup] Timer stopped successfully");
                }

                response = "Timer stopped. Use 'startgame' to begin a new game or 'pregame' to reset the map.";
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"[Backup] Error executing command: {ex.Message}");
                response = $"An error occurred while stopping the timer: {ex.Message}";
                return false;
            }
        }
    }
} 