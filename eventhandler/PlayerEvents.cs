using Exiled.Events.EventArgs.Player;

namespace forsaken.Events
{
    public class PlayerEvents
    {
        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (!ForsakenPlugin.Instance.Config.IsEnabled) return;
            ev.IsAllowed = false;
        }
    }
} 