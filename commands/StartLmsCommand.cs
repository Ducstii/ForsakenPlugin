using CommandSystem;
using Exiled.API.Features;
using System;

namespace forsaken.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class StartLmsCommand : ICommand
    {
        public string Command => "startlms";
        public string[] Aliases => Array.Empty<string>();
        public string Description => "Starts Last Man Standing mode";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Log.Debug("StartLMS command executing...");
            var plugin = ForsakenPlugin.Instance;
            plugin.StartLmsSequence(sender as CommandSender);
            Cassie.Message("<color=red>Last Man Standing!</color>", isSubtitles: true);
            Log.Debug("LMS sequence started");
            response = "LMS mode started!";
            return true;
        }
    }
} 