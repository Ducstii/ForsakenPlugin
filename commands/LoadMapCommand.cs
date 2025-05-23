using CommandSystem;
using Exiled.API.Features;
using System;

namespace forsaken.Commands
{
    public class LoadMapCommand : ICommand
    {
        public string Command => "loadmap";
        public string[] Aliases => Array.Empty<string>();
        public string Description => "Loads the MiniGame map if necessary";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Log.Debug("LoadMap command executing...");
            
            if (ForsakenPlugin.Instance.LoadMap("MiniGame"))
            {
                Log.Debug("Successfully loaded MiniGame map");
                response = "MiniGame map loaded successfully.";
                return true;
            }

            Log.Debug("Failed to load map - MapEditorReborn might be missing");
            response = "Failed to load MiniGame map. Make sure MapEditorReborn is installed and loaded.";
            return false;
        }
    }
}