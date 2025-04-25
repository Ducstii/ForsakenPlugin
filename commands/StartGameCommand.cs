using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using System;

namespace forsaken.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class StartGameCommand : ICommand
    {
        public string Command => "startgame";
        public string[] Aliases => Array.Empty<string>();
        public string Description => "Starts the game with countdown and timer";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Log.Debug("StartGame command executing...");

            // Disable item pickups
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
            Log.Debug("Item pickups disabled");

            var plugin = ForsakenPlugin.Instance;
            _ = plugin.StartGameSequence(); // Fire and forget
            Log.Debug("Game sequence started");

            response = "Game sequence started!";
            return true;
        }

        private void OnPickingUpItem(Exiled.Events.EventArgs.Player.PickingUpItemEventArgs ev)
        {
            Log.Debug($"Blocked item pickup attempt by {ev.Player.Nickname}");
            ev.IsAllowed = false;
        }
    }
} 