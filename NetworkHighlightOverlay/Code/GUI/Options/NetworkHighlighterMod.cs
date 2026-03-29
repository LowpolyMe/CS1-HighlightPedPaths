using ICities;
using NetworkHighlightOverlay.Settings;

namespace NetworkHighlightOverlay.GUI.Options
{
    public class NetworkHighlighterMod : IUserMod
    {
        public string Name => "Network Highlighter";
        public string Description =>
            "Highlights various networks (paths, roads, rails, etc.) including hidden/invisible ones.";

        public void OnSettingsUI(UIHelperBase helper)
        {
            new NetworkHighlighterOptionsUi(ModSettings.Shared).Build(helper);
        }
    }
}
