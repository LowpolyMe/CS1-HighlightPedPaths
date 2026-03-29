using System;
using ColossalFramework.UI;
using ICities;
using NetworkHighlightOverlay.HighlightCategories;
using NetworkHighlightOverlay.Settings;
using NetworkHighlightOverlay.Utility;
using UnityEngine;

namespace NetworkHighlightOverlay.GUI.Options
{
    public sealed class NetworkHighlighterOptionsUi
    {
        private readonly ModSettings _settings;
        private Texture2D _hueTexture;
        private Texture2D _valueTexture;
        private Texture2D _widthTexture;

        public NetworkHighlighterOptionsUi(ModSettings settings)
        {
            _settings = settings;
        }

        public void Build(UIHelperBase helper)
        {
            EnsureTexturesLoaded();

            UIComponent rootComponent = (UIComponent)((UIHelper)helper).self;
            BuildTabbedSettingsUi(rootComponent);
        }

        private void EnsureTexturesLoaded()
        {
            if (_hueTexture == null)
            {
                _hueTexture = ModResources.LoadTexture("HueGradient.png");
                if (_hueTexture == null)
                    throw new InvalidOperationException("Missing required texture: Resources/HueGradient.png");
            }

            if (_valueTexture == null)
            {
                _valueTexture = ModResources.LoadTexture("ValueGradient.png");
                if (_valueTexture == null)
                    throw new InvalidOperationException("Missing required texture: Resources/ValueGradient.png");
            }

            if (_widthTexture == null)
            {
                _widthTexture = ModResources.LoadTexture("HighlightWidth.png");
                if (_widthTexture == null)
                    throw new InvalidOperationException("Missing required texture: Resources/HighlightWidth.png");
            }
        }

        private void BuildTabbedSettingsUi(UIComponent rootComponent)
        {
            UIPanel tabRoot = CreateRootPanel(rootComponent);
            UITabstrip tabStrip = CreateUITabstrip(tabRoot);
            UITabContainer tabContainer = CreateUITabContainer(tabRoot, tabStrip);

            tabStrip.tabPages = tabContainer;
            tabStrip.selectedIndex = -1;

            BuildHighlightsTab(tabContainer, tabStrip);
            BuildControlsTab(tabContainer, tabStrip);
            BuildDangerZoneTab(rootComponent, tabContainer, tabStrip, tabRoot);

            tabStrip.selectedIndex = 0;
        }

        private void BuildHighlightsTab(UITabContainer tabContainer, UITabstrip tabStrip)
        {
            UIScrollablePanel highlightsPanel = UIUtility.CreateTab(tabContainer, tabStrip, "Highlights", Color.white);
            UIPanel generalPanel = CreateSectionPanel(highlightsPanel, "General");
            
            highlightsPanel.autoLayoutPadding = new RectOffset(0, 0, 10, 10);
            
            AddGeneralHighlightSettings(new UIHelper(generalPanel));
            AddDivider(highlightsPanel);
            AddHighlightCategorySection(highlightsPanel, "Roads and Paths", HighlightCategoryGroup.RoadsAndPaths);
            AddDivider(highlightsPanel);
            AddHighlightCategorySection(highlightsPanel, "Public Transport", HighlightCategoryGroup.PublicTransport);
            AddDivider(highlightsPanel);
            AddHighlightCategorySection(highlightsPanel, "Special Networks", HighlightCategoryGroup.SpecialNetworks);
        }

        private static void AddDivider(UIScrollablePanel parent)
        {
            UIPanel divider = parent.AddUIComponent<UIPanel>();
            divider.name = "NHO_Divider";
            divider.width = Mathf.Max(0f, parent.width - parent.autoLayoutPadding.horizontal);
            divider.height = 5f;
            divider.backgroundSprite = "ContentManagerItemBackground";
        }

        private void BuildControlsTab(UITabContainer tabContainer, UITabstrip tabStrip)
        {
            UIScrollablePanel controlsPanel = UIUtility.CreateTab(tabContainer, tabStrip, "Controls", Color.white);
            UIHelper controlsHelper = new UIHelper(controlsPanel);

            UIUtility.CreateSettingToggle(
                controlsHelper,
                _settings.UseUuiButton,
                value => _settings.UseUuiButton = value,
                "Use UUI button");

            UIUtility.CreateSettingToggle(
                controlsHelper,
                _settings.IsInGameTogglePanelEnabled,
                value => _settings.IsInGameTogglePanelEnabled = value,
                "Show in-game toggle panel");

            UIKeymappingsPanel keymappingsPanel = controlsPanel.gameObject.AddComponent<UIKeymappingsPanel>();
            keymappingsPanel.AddKeymapping("Toggle highlights hotkey", _settings.ToggleHighlightsHotkey);
        }

        private void BuildDangerZoneTab(
            UIComponent rootComponent,
            UITabContainer tabContainer,
            UITabstrip tabStrip,
            UIPanel tabRoot)
        {
            UIScrollablePanel dangerPanel = UIUtility.CreateTab(tabContainer, tabStrip, "DANGER ZONE", Color.red);
            UIHelper dangerHelper = new UIHelper(dangerPanel);
            dangerHelper.AddSpace(20);
            dangerHelper.AddButton(
                "Reset ALL settings to defaults",
                () =>
                {
                    _settings.ResetToDefaults();
                    Rebuild(rootComponent, tabRoot);
                });
        }

        private void Rebuild(UIComponent rootComponent, UIPanel tabRoot)
        {
            if (tabRoot != null && tabRoot.parent != null)
            {
                tabRoot.parent.RemoveUIComponent(tabRoot);
                UnityEngine.Object.Destroy(tabRoot.gameObject);
            }

            BuildTabbedSettingsUi(rootComponent);
        }

        private void AddGeneralHighlightSettings(UIHelper settingsHelper)
        {
            UIUtility.CreateSettingSlider(
                settingsHelper,
                _settings.HighlightStrength,
                value => _settings.HighlightStrength = value,
                _valueTexture,
                "Highlight Strength");

            UIUtility.CreateSettingSlider(
                settingsHelper,
                _settings.HighlightWidth,
                value => _settings.HighlightWidth = value,
                _widthTexture,
                "Highlight Thickness");

            UIUtility.CreateSettingToggle(
                settingsHelper,
                _settings.HighlightBridges,
                value => _settings.HighlightBridges = value,
                "Highlight bridges",
                "toggle highlight for bridges");

            UIUtility.CreateSettingToggle(
                settingsHelper,
                _settings.HighlightTunnels,
                value => _settings.HighlightTunnels = value,
                "Highlight tunnels",
                "toggle highlight for tunnels");
            
        }

        private void AddHighlightCategorySection(
            UIScrollablePanel settingsPanel,
            string title,
            HighlightCategoryGroup categoryGroup)
        {
            UIPanel groupPanel = CreateSectionPanel(settingsPanel, title);
            UIHelper groupHelper = new UIHelper(groupPanel);

            HighlightCategoryDefinition[] definitions = HighlightCategoryCatalog.GetEligible(categoryGroup);
            int definitionCount = definitions.Length;
            for (int i = 0; i < definitionCount; i++)
            {
                AddHighlightCategoryEntry(groupHelper, definitions[i]);
            }
        }

        private void AddHighlightCategoryEntry(UIHelper groupHelper, HighlightCategoryDefinition definition)
        {
            HighlightCategoryId categoryId = definition.Id;
            UIUtility.CreateSettingToggle(
                groupHelper,
                _settings.GetCategoryEnabled(categoryId),
                value => _settings.SetCategoryEnabled(categoryId, value),
                definition.Label,
                "toggle highlight for " + definition.Label);

            UIUtility.CreateSettingSlider(
                groupHelper,
                _settings.GetCategoryHue(categoryId),
                value => _settings.SetCategoryHue(categoryId, value),
                _hueTexture);
        }

        private static UIPanel CreateSectionPanel(UIScrollablePanel settingsPanel, string title)
        {
            UIPanel groupPanel = settingsPanel.AddUIComponent<UIPanel>();
            groupPanel.autoLayout = true;
            groupPanel.autoLayoutDirection = LayoutDirection.Vertical;
            groupPanel.autoLayoutPadding = new RectOffset(0, 0, 0, 6);
            groupPanel.autoFitChildrenHorizontally = false;
            groupPanel.autoFitChildrenVertically = true;
            groupPanel.clipChildren = true;
            groupPanel.width = Mathf.Max(0f, settingsPanel.width - settingsPanel.autoLayoutPadding.horizontal);
            AddSectionHeader(groupPanel, title);
            return groupPanel;
        }

        private static void AddSectionHeader(UIComponent parent, string title)
        {
            UILabel label = parent.AddUIComponent<UILabel>();
            label.name = "NHO_SectionHeader_" + title.Replace(' ', '_');
            label.text = title;
            label.textColor = Color.white;
            label.textScale = 1.1f;
            label.autoSize = true;
        }

        private static UIPanel CreateRootPanel(UIComponent rootComponent)
        {
            UIPanel tabRoot = rootComponent.AddUIComponent<UIPanel>();
            tabRoot.name = "NHO_TabRoot";
            tabRoot.autoLayout = false;
            tabRoot.clipChildren = true;
            tabRoot.relativePosition = new Vector3(10f, 10f);
            tabRoot.width = Mathf.Max(0f, rootComponent.width - 20f);
            tabRoot.height = Mathf.Max(0f, rootComponent.height - 20f);
            return tabRoot;
        }

        private static UITabContainer CreateUITabContainer(UIComponent parent, UITabstrip tabStrip)
        {
            UITabContainer tabContainer = parent.AddUIComponent<UITabContainer>();
            tabContainer.name = "NHO_TabContainer";
            tabContainer.relativePosition = new Vector3(0f, tabStrip.height + 5f);
            tabContainer.width = parent.width;
            tabContainer.height = Mathf.Max(0f, parent.height - tabStrip.height - 5f);
            return tabContainer;
        }

        private static UITabstrip CreateUITabstrip(UIComponent parent)
        {
            UITabstrip tabStrip = parent.AddUIComponent<UITabstrip>();
            tabStrip.name = "NHO_TabStrip";
            tabStrip.width = parent.width;
            tabStrip.height = 30f;
            tabStrip.relativePosition = new Vector3(0f, 0f);
            tabStrip.padding = new RectOffset(5, 5, 0, 0);
            return tabStrip;
        }

    }
}
