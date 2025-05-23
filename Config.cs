using Exiled.API.Interfaces;
using System.ComponentModel;

namespace forsaken
{
    public sealed class Config : IConfig
    {
        [Description("Whether the plugin is enabled or not")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether debug logs should be shown")]
        public bool Debug { get; set; } = false;

        [Description("The name of the MER map to load")]
        public string MapName { get; set; } = "MiniGame";

        [Description("Duration of the countdown in seconds")]
        public float CountdownDuration { get; set; } = 10f;

        [Description("Duration of the main game timer in seconds (default: 360 = 6 minutes)")]
        public int GameDuration { get; set; } = 360;

        [Description("Duration of Last Man Standing mode in seconds (default: 96 = 1:36)")]
        public int LmsDuration { get; set; } = 96;

        [Description("Position where players are teleported during pregame (x, y, z)")]
        public string SpawnPosition { get; set; } = "92,998,1";

        [Description("Whether to broadcast remaining time")]
        public bool ShowTimeRemaining { get; set; } = true;

        [Description("Message to show when time is up")]
        public string TimeUpMessage { get; set; } = "Times Up! All alive players have been set to Tutorial.";
    }
} 