using System;
using ColossalFramework;
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
        private readonly Texture2D _hueTexture;
        private readonly Texture2D _valueTexture;
        private readonly Texture2D _widthTexture;
        private SavedInputKey _editingBinding;

        public NetworkHighlighterOptionsUi(ModSettings settings)
        {
            _settings = settings;
            _hueTexture = ModResources.GetTexture("HueGradient.png");
            _valueTexture = ModResources.GetTexture("ValueGradient.png");
            _widthTexture = ModResources.GetTexture("HighlightWidth.png");
        }

        public void Build(UIHelperBase helper)
        {
            UIComponent rootComponent = (UIComponent)((UIHelper)helper).self;
            BuildOptionsTabs(rootComponent);
        }

        private void BuildOptionsTabs(UIComponent rootComponent)
        {
            UIUtility.TabLayout tabLayout = UIUtility.CreateTabLayout(rootComponent);

            BuildHighlightsTab(tabLayout);
            BuildControlsTab(tabLayout);
            BuildDangerZoneTab(tabLayout);
            tabLayout.TabStrip.selectedIndex = 0;
        }

        private void BuildHighlightsTab(UIUtility.TabLayout tabLayout)
        {
            UIScrollablePanel highlightsPanel = UIUtility.CreateTab(tabLayout, "Highlights", Color.white);

            AddSections(
                highlightsPanel,
                AddGeneralHighlightSection,
                panel => AddHighlightCategorySection(panel, "Roads and Paths", HighlightCategoryGroup.RoadsAndPaths),
                panel => AddHighlightCategorySection(panel, "Public Transport", HighlightCategoryGroup.PublicTransport),
                panel => AddHighlightCategorySection(panel, "Special Networks", HighlightCategoryGroup.SpecialNetworks));
        }

        private void BuildControlsTab(UIUtility.TabLayout tabLayout)
        {
            UIScrollablePanel controlsPanel = UIUtility.CreateTab(tabLayout, "Controls", Color.white);
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

            AddKeyBinding(
                controlsPanel,
                "Toggle highlights hotkey",
                _settings.ToggleHighlightsHotkey,
                false);
        }

        private void BuildDangerZoneTab(UIUtility.TabLayout tabLayout)
        {
            UIScrollablePanel dangerPanel = UIUtility.CreateTab(tabLayout, "DANGER ZONE", Color.red);
            UIHelper dangerHelper = new UIHelper(dangerPanel);
            dangerHelper.AddSpace(20);
            dangerHelper.AddButton(
                "Reset ALL settings to defaults",
                () =>
                {
                    _settings.ResetToDefaults();
                    Rebuild(tabLayout.Root);
                });
        }

        private void Rebuild(UIPanel tabRoot)
        {
            if (tabRoot == null || tabRoot.parent == null)
                return;

            UIComponent rootComponent = tabRoot.parent;
            rootComponent.RemoveUIComponent(tabRoot);
            UnityEngine.Object.Destroy(tabRoot.gameObject);
            BuildOptionsTabs(rootComponent);
        }

        private void AddGeneralHighlightSection(UIScrollablePanel highlightsPanel)
        {
            UIHelper generalSettings = UIUtility.CreateSection(highlightsPanel, "General");
            highlightsPanel.autoLayoutPadding = new RectOffset(0, 0, 10, 10);

            UIUtility.CreateSettingSlider(
                generalSettings,
                _settings.HighlightStrength,
                value => _settings.HighlightStrength = value,
                _valueTexture,
                "Highlight Strength");

            UIUtility.CreateSettingSlider(
                generalSettings,
                _settings.HighlightWidth,
                value => _settings.HighlightWidth = value,
                _widthTexture,
                "Highlight Thickness");

            UIUtility.CreateSettingToggle(
                generalSettings,
                _settings.HighlightBridges,
                value => _settings.HighlightBridges = value,
                "Highlight bridges",
                "toggle highlight for bridges");

            UIUtility.CreateSettingToggle(
                generalSettings,
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
            UIHelper groupHelper = UIUtility.CreateSection(settingsPanel, title);

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

        private void AddKeyBinding(
            UIComponent parent,
            string label,
            SavedInputKey savedInputKey,
            bool useAlternateBackground)
        {
            UIButton bindingButton = UIUtility.CreateKeyBindingButton(
                parent,
                label,
                savedInputKey.ToLocalizedString("KEYNAME"),
                savedInputKey,
                useAlternateBackground);

            bindingButton.eventKeyDown += OnBindingKeyDown;
            bindingButton.eventMouseDown += OnBindingMouseDown;
            bindingButton.eventVisibilityChanged += OnBindingVisibilityChanged;
        }

        private static bool IsModifierKey(KeyCode code)
        {
            switch (code)
            {
                case KeyCode.LeftControl:
                case KeyCode.RightControl:
                case KeyCode.LeftShift:
                case KeyCode.RightShift:
                case KeyCode.LeftAlt:
                case KeyCode.RightAlt:
                    return true;
                default:
                    return false;
            }
        }

        private static void OnBindingVisibilityChanged(UIComponent component, bool isVisible)
        {
            if (isVisible && component.objectUserData is SavedInputKey savedInputKey)
                (component as UIButton).text = savedInputKey.ToLocalizedString("KEYNAME");
        }

        private void OnBindingKeyDown(UIComponent component, UIKeyEventParameter parameter)
        {
            if (_editingBinding == null || IsModifierKey(parameter.keycode))
                return;

            parameter.Use();
            UIView.PopModal();

            InputKey value = parameter.keycode == KeyCode.Escape
                ? _editingBinding.value
                : SavedInputKey.Encode(parameter.keycode, parameter.control, parameter.shift, parameter.alt);
            if (parameter.keycode == KeyCode.Backspace)
                value = SavedInputKey.Empty;

            _editingBinding.value = value;
            (parameter.source as UITextComponent).text = _editingBinding.ToLocalizedString("KEYNAME");
            _editingBinding = null;
        }

        private void OnBindingMouseDown(UIComponent component, UIMouseEventParameter parameter)
        {
            if (_editingBinding == null)
            {
                parameter.Use();
                _editingBinding = (SavedInputKey)parameter.source.objectUserData;
                UIButton button = parameter.source as UIButton;
                button.buttonsMask = UIMouseButton.Left |
                                     UIMouseButton.Right |
                                     UIMouseButton.Middle |
                                     UIMouseButton.Special0 |
                                     UIMouseButton.Special1 |
                                     UIMouseButton.Special2 |
                                     UIMouseButton.Special3;
                button.text = "Press any key";
                parameter.source.Focus();
                UIView.PushModal(parameter.source);
                return;
            }

            InputKey value;
            if (!TryEncodeMouseBinding(parameter.buttons, out value))
                return;

            parameter.Use();
            UIView.PopModal();

            _editingBinding.value = value;

            UIButton sourceButton = parameter.source as UIButton;
            sourceButton.text = _editingBinding.ToLocalizedString("KEYNAME");
            sourceButton.buttonsMask = UIMouseButton.Left;
            _editingBinding = null;
        }

        private static bool TryEncodeMouseBinding(UIMouseButton button, out InputKey value)
        {
            value = SavedInputKey.Empty;
            if (button == UIMouseButton.Left || button == UIMouseButton.Right)
                return false;

            bool control;
            bool shift;
            bool alt;
            GetCurrentModifiers(out control, out shift, out alt);
            value = SavedInputKey.Encode(TranslateMouseButton(button), control, shift, alt);
            return true;
        }

        private static void GetCurrentModifiers(out bool control, out bool shift, out bool alt)
        {
            control = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        }

        private static KeyCode TranslateMouseButton(UIMouseButton button)
        {
            switch (button)
            {
                case UIMouseButton.Left:
                    return KeyCode.Mouse0;
                case UIMouseButton.Right:
                    return KeyCode.Mouse1;
                case UIMouseButton.Middle:
                    return KeyCode.Mouse2;
                case UIMouseButton.Special0:
                    return KeyCode.Mouse3;
                case UIMouseButton.Special1:
                    return KeyCode.Mouse4;
                case UIMouseButton.Special2:
                    return KeyCode.Mouse5;
                case UIMouseButton.Special3:
                    return KeyCode.Mouse6;
                default:
                    return KeyCode.None;
            }
        }

        private static void AddSections(
            UIScrollablePanel panel,
            params Action<UIScrollablePanel>[] sectionBuilders)
        {
            int sectionCount = sectionBuilders.Length;
            for (int i = 0; i < sectionCount; i++)
            {
                if (i > 0)
                    UIUtility.AddSectionDivider(panel);

                sectionBuilders[i](panel);
            }
        }
    }
}
