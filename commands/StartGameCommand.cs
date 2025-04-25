using CommandSystem;
using Exiled.API.Features;
using System;

namespace forsaken.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class StartGameCommand : ICommand
    {
        public string Command => "startgame";
        public string[] Aliases => Array.Empty<string>();
        public string Description => "Starts the game with a countdown";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                var plugin = ForsakenPlugin.Instance;

                if (plugin.Config.Debug)
                {
                    Log.Debug("[StartGame] Command execution starting...");
                }

                // Start the game sequence
                plugin.StartGameSequence(); // Just call the method directly

                Log.Debug("[StartGame] Game sequence started");
                response = "Game sequence started!";
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"[StartGame] Error executing command: {ex.Message}");
                response = $"An error occurred while starting the game: {ex.Message}";
                return false;
            }
        }
    }
} 