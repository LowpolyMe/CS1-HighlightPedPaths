using ICities;
using NetworkHighlightOverlay.Settings;

namespace NetworkHighlightOverlay.GUI.Options
{
    public class NetworkHighlighterMod : IUserMod
    {
        public string Name => "Network Highlighter";
        public string Description =>
            "Highlights various networks (paths, roads, rails, etc.) including hidden/invisible ones.";

        private readonly NetworkHighlighterOptionsUi _optionsUi;

        public NetworkHighlighterMod()
        {
            _optionsUi = new NetworkHighlighterOptionsUi(ModSettings.Shared);
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            _optionsUi.Build(helper);
        }
    }
}
