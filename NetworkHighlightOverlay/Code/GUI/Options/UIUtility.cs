using System;
using ColossalFramework.UI;
using UnityEngine;

namespace NetworkHighlightOverlay.GUI.Options
{
    internal static class UIUtility
    {
        public sealed class TabLayout
        {
            public readonly UIPanel Root;
            public readonly UITabstrip TabStrip;
            public readonly UITabContainer TabContainer;

            public TabLayout(UIPanel root, UITabstrip tabStrip, UITabContainer tabContainer)
            {
                Root = root;
                TabStrip = tabStrip;
                TabContainer = tabContainer;
            }
        }

        public static TabLayout CreateTabLayout(UIComponent rootComponent)
        {
            UIPanel root = CreateRootPanel(rootComponent);
            UITabstrip tabStrip = CreateTabStrip(root);
            UITabContainer tabContainer = CreateTabContainer(root, tabStrip);

            tabStrip.tabPages = tabContainer;
            tabStrip.selectedIndex = -1;
            return new TabLayout(root, tabStrip, tabContainer);
        }

        public static UIScrollablePanel CreateTab(TabLayout tabLayout, string title, Color tintColor)
        {
            UIPanel page;
            UIScrollablePanel scrollablePanel = CreateTabPage(tabLayout.TabContainer, title, out page);
            UIButton tabButton = CreateTabButton(tabLayout.TabStrip, title, tintColor);

            tabLayout.TabStrip.AddTab(title, tabButton.gameObject, page.gameObject);
            return scrollablePanel;
        }

        public static UIHelper CreateSection(UIScrollablePanel settingsPanel, string title)
        {
            UIPanel sectionPanel = CreateSectionPanel(settingsPanel);
            AddSectionHeader(sectionPanel, title);
            return new UIHelper(sectionPanel);
        }

        public static void AddSectionDivider(UIScrollablePanel parent)
        {
            UIPanel divider = parent.AddUIComponent<UIPanel>();
            divider.name = "NHO_Divider";
            divider.width = Mathf.Max(0f, parent.width - parent.autoLayoutPadding.horizontal);
            divider.height = 5f;
            divider.backgroundSprite = "ContentManagerItemBackground";
        }

        public static void CreateSettingToggle(
            UIHelper helper,
            bool initialValue,
            Action<bool> onChanged,
            string label,
            string tooltip = null)
        {
            UIComponent root = (UIComponent)helper.self;
            UICheckBox toggle = (UICheckBox)root.AttachUIComponent(UITemplateManager.GetAsGameObject("OptionsCheckBoxTemplate"));
            toggle.label.verticalAlignment = UIVerticalAlignment.Middle;
            toggle.text = label;
            toggle.tooltip = tooltip;
            toggle.isChecked = initialValue;
            toggle.eventCheckChanged += (component, value) => onChanged(value);
        }

        public static void CreateSettingSlider(
            UIHelper helper,
            float initialValue,
            Action<float> onChanged,
            Texture2D backgroundTexture,
            string label = "")
        {
            const float min = 0f;
            const float max = 1f;
            const float step = 0.01f;

            UIComponent root = (UIComponent)helper.self;
            UIPanel row = (UIPanel)root.AttachUIComponent(UITemplateManager.GetAsGameObject("OptionsSliderTemplate"));
            UILabel rowLabel;
            UISlider slider;
            FindTemplateParts(row, out rowLabel, out slider);

            if (string.IsNullOrEmpty(label))
            {
                row.RemoveUIComponent(rowLabel);
                UnityEngine.Object.Destroy(rowLabel.gameObject);
                row.autoLayoutPadding = new RectOffset(0, 0, 0, 5);
                row.autoFitChildrenVertically = true;
            }
            else
            {
                rowLabel.text = label;
            }

            slider.minValue = min;
            slider.maxValue = max;
            slider.stepSize = step;
            slider.value = initialValue;
            slider.eventValueChanged += (component, value) => onChanged(value);
            ApplySettingSliderStyle(slider, backgroundTexture);
        }
       
        public static UIButton CreateKeyBindingButton(
            UIComponent parent,
            string labelText,
            string bindingText,
            object userData,
            bool useAlternateBackground)
        {
            GameObject template = UITemplateManager.GetAsGameObject("KeyBindingTemplate");
            UIPanel row = (UIPanel)parent.AttachUIComponent(template);
            UILabel label;
            UIButton button;
            FindTemplateParts(row, out label, out button);

            if (useAlternateBackground)
                row.backgroundSprite = null;

            label.text = labelText;
            button.text = bindingText;
            button.objectUserData = userData;
            return button;
        }

        private static UIButton CreateTabButton(UITabstrip tabStrip, string title, Color tintColor)
        {
            UIButton tabButton = tabStrip.AddUIComponent<UIButton>();
            tabButton.text = title;
            tabButton.textColor = tintColor;
            tabButton.autoSize = false;
            tabButton.width = 150f;
            tabButton.height = 30f;
            tabButton.textScale = 0.9f;
            tabButton.normalBgSprite = "ButtonMenu";
            tabButton.hoveredBgSprite = "ButtonMenuHovered";
            tabButton.pressedBgSprite = "ButtonMenuPressed";
            tabButton.disabledBgSprite = "ButtonMenuDisabled";
            return tabButton;
        }

        private static UITabContainer CreateTabContainer(UIComponent parent, UITabstrip tabStrip)
        {
            UITabContainer tabContainer = parent.AddUIComponent<UITabContainer>();
            tabContainer.name = "NHO_TabContainer";
            tabContainer.relativePosition = new Vector3(0f, tabStrip.height + 5f);
            tabContainer.width = parent.width;
            tabContainer.height = Mathf.Max(0f, parent.height - tabStrip.height - 5f);
            return tabContainer;
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

        private static UIPanel CreateSectionPanel(UIScrollablePanel settingsPanel)
        {
            UIPanel sectionPanel = settingsPanel.AddUIComponent<UIPanel>();
            sectionPanel.autoLayout = true;
            sectionPanel.autoLayoutDirection = LayoutDirection.Vertical;
            sectionPanel.autoLayoutPadding = new RectOffset(0, 0, 0, 6);
            sectionPanel.autoFitChildrenHorizontally = false;
            sectionPanel.autoFitChildrenVertically = true;
            sectionPanel.clipChildren = true;
            sectionPanel.width = Mathf.Max(0f, settingsPanel.width - settingsPanel.autoLayoutPadding.horizontal);
            return sectionPanel;
        }

        private static UITabstrip CreateTabStrip(UIComponent parent)
        {
            UITabstrip tabStrip = parent.AddUIComponent<UITabstrip>();
            tabStrip.name = "NHO_TabStrip";
            tabStrip.width = parent.width;
            tabStrip.height = 30f;
            tabStrip.relativePosition = new Vector3(0f, 0f);
            tabStrip.padding = new RectOffset(5, 5, 0, 0);
            return tabStrip;
        }

        private static UIScrollablePanel CreateTabPage(
            UITabContainer tabContainer,
            string title,
            out UIPanel page)
        {
            const float scrollbarWidth = 12f;
            const float scrollbarGap = 5f;

            page = tabContainer.AddUIComponent<UIPanel>();
            page.name = "NHO_" + title + "_Page";
            page.width = tabContainer.width;
            page.height = tabContainer.height;
            page.clipChildren = true;

            UIScrollablePanel scrollablePanel = page.AddUIComponent<UIScrollablePanel>();
            scrollablePanel.name = "NHO_" + title + "_ScrollPanel";
            scrollablePanel.relativePosition = Vector3.zero;
            scrollablePanel.width = Mathf.Max(0f, page.width - scrollbarWidth - scrollbarGap);
            scrollablePanel.height = page.height;
            scrollablePanel.autoLayout = true;
            scrollablePanel.autoLayoutDirection = LayoutDirection.Vertical;
            scrollablePanel.autoLayoutPadding = new RectOffset(5, 5, 5, 5);
            scrollablePanel.clipChildren = true;
            scrollablePanel.builtinKeyNavigation = true;
            scrollablePanel.scrollWheelDirection = UIOrientation.Vertical;
            scrollablePanel.scrollWheelAmount = 40;

            UIScrollbar scrollbar = page.AddUIComponent<UIScrollbar>();
            scrollbar.name = "NHO_" + title + "_Scrollbar";
            scrollbar.width = scrollbarWidth;
            scrollbar.height = page.height;
            scrollbar.relativePosition = new Vector3(page.width - scrollbar.width, 0f);
            scrollbar.orientation = UIOrientation.Vertical;
            scrollbar.incrementAmount = 40f;

            UIPanel track = scrollbar.AddUIComponent<UIPanel>();
            track.relativePosition = Vector3.zero;
            track.width = scrollbar.width;
            track.height = scrollbar.height;
            track.backgroundSprite = "GenericPanel";
            track.color = new Color32(55, 55, 55, 255);
            scrollbar.trackObject = track;

            UIPanel thumb = track.AddUIComponent<UIPanel>();
            thumb.relativePosition = Vector3.zero;
            thumb.width = track.width;
            thumb.height = 40f;
            thumb.backgroundSprite = "GenericPanel";
            thumb.color = new Color32(170, 170, 170, 255);
            scrollbar.thumbObject = thumb;

            scrollablePanel.verticalScrollbar = scrollbar;
            return scrollablePanel;
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

        private static void ApplySettingSliderStyle(UISlider slider, Texture2D backgroundTexture)
        {
            slider.backgroundSprite = string.Empty;
            slider.color = Color.white;
            slider.clipChildren = true;

            UITextureSprite textureBar = slider.AddUIComponent<UITextureSprite>();
            textureBar.texture = backgroundTexture;
            textureBar.relativePosition = Vector3.zero;
            textureBar.zOrder = 0;
            slider.eventSizeChanged += (component, size) => textureBar.size = size;
            textureBar.size = slider.size;
            slider.thumbObject.zOrder = textureBar.zOrder + 1;
        }

        private static void FindTemplateParts<TControl>(
            UIPanel row,
            out UILabel label,
            out TControl control)
            where TControl : UIComponent
        {
            label = null;
            control = null;

            int childCount = row.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                UIComponent child = row.transform.GetChild(i).GetComponent<UIComponent>();
                if (label == null)
                    label = child as UILabel;

                if (control == null)
                    control = child as TControl;

                if (label != null && control != null)
                    return;
            }
        }
    }
}
