using CommandSystem;
using System;

namespace forsaken.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ForsakenCommand : ParentCommand
    {
        public ForsakenCommand() => LoadGeneratedCommands();

        public override string Command => "forsaken";
        public override string[] Aliases => Array.Empty<string>();
        public override string Description => "Main command for Forsaken plugin";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new PregameCommand());
            RegisterCommand(new StartGameCommand());
            RegisterCommand(new OverCommand());
            RegisterCommand(new StartLmsCommand());
            RegisterCommand(new LoadMapCommand());
            RegisterCommand(new MemeCommand());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Available subcommands:\n";
            foreach (ICommand command in AllCommands)
            {
                response += $"\n- {command.Command} ({string.Join(", ", command.Aliases)})\n  {command.Description}";
            }
            return true;
        }
    }
} 