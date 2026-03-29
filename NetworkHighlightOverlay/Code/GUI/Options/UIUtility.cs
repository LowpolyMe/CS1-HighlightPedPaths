using System;
using ColossalFramework.UI;
using UnityEngine;

namespace NetworkHighlightOverlay.GUI.Options
{
    public static class UIUtility
    {
        public static UIScrollablePanel CreateTab(
            UITabContainer tabContainer,
            UITabstrip tabStrip,
            string title,
            Color tintColor)
        {
            UIPanel page;
            UIScrollablePanel scrollablePanel = CreateTabPage(tabContainer, title, out page);
            UIButton tabButton = CreateTabButton(tabStrip, title, tintColor);

            tabStrip.AddTab(title, tabButton.gameObject, page.gameObject);
            return scrollablePanel;
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
            UILabel rowLabel = null;
            UISlider slider = null;
            int childCount = row.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
            
                UIComponent child = row.transform.GetChild(i).GetComponent<UIComponent>();
                if (rowLabel == null)
                    rowLabel = child as UILabel;

                if (slider == null)
                    slider = child as UISlider;
            }

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
            UILabel label = null;
            UIButton button = null;
            int childCount = row.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                UIComponent child = row.transform.GetChild(i).GetComponent<UIComponent>();
                if (label == null)
                    label = child as UILabel;

                if (button == null)
                    button = child as UIButton;
            }

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

        private static UIScrollablePanel CreateTabPage(
            UITabContainer tabContainer,
            string title,
            out UIPanel page)
        {
            const float scrollbarWidth = 12f;
            const float scrollbarGap = 5f;

            page = tabContainer.AddUIComponent<UIPanel>();
            page.name = $"NHO_{title}_Page";
            page.width = tabContainer.width;
            page.height = tabContainer.height;
            page.clipChildren = true;

            UIScrollablePanel scrollablePanel = page.AddUIComponent<UIScrollablePanel>();
            scrollablePanel.name = $"NHO_{title}_ScrollPanel";
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
            scrollbar.name = $"NHO_{title}_Scrollbar";
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
    }
}
