using CommandSystem;
using System;

namespace forsaken.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class MemeCommand : ICommand
    {
        public string Command => "Memer";
        public string[] Aliases => Array.Empty<string>();
        public string Description => "A meme command";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Fuck you";
            return true;
        }
    }
} 