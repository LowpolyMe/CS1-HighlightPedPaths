using System;
using NetworkHighlightOverlay.Core;
using NetworkHighlightOverlay.Settings;
using NetworkHighlightOverlay.Utility;
using UnifiedUI.Helpers;
using UnityEngine;

namespace NetworkHighlightOverlay.GUI.UUI
{
    public sealed class UuiButtonController
    {
        private const string ButtonName = "NetworkHighlightOverlay.ToggleButton";
        private const string ToggleTooltip = "Toggle Network Highlights";

        private UUICustomButton _button;
        private ModSettings _settings;
        private ActivationHandler _activationHandler;
        private bool _lastUseUuiButton;

        public void Initialize(ModSettings settings, ActivationHandler activationHandler)
        {
            _settings = settings ?? throw new ArgumentNullException("settings");
            _activationHandler = activationHandler ?? throw new ArgumentNullException("activationHandler");

            _settings.SettingsChanged += SyncRegistration;
            _activationHandler.ActivationChanged += SetPressed;

            SyncRegistration();
        }

        public void Dispose()
        {
            if (_activationHandler != null)
            {
                _activationHandler.ActivationChanged -= SetPressed;
            }

            if (_settings != null)
            {
                _settings.SettingsChanged -= SyncRegistration;
            }

            UnregisterUui();
            _activationHandler = null;
            _settings = null;
        }

        private void RegisterUui()
        {
            if (_button != null && _button.Button != null)
                return;

            _button = UUIHelpers.RegisterCustomButton(
                name: ButtonName,
                groupName: null,
                tooltip: ToggleTooltip,
                icon: ModResources.GetTexture("UUIIcon.png"),
                onToggle: _activationHandler.SetActive,
                onToolChanged: null);
        }

        private void UnregisterUui()
        {
            if (_button != null && _button.Button != null)
            {
                UUIHelpers.Destroy(_button.Button);
            }

            _button = null;
        }

        private void SetPressed(bool isPressed)
        {
            if (_button == null || _button.IsPressed == isPressed)
                return;

            _button.IsPressed = isPressed;
        }

        private void SyncRegistration()
        {
            bool useUuiButton = _settings.UseUuiButton;
            if (useUuiButton == _lastUseUuiButton)
                return;

            _lastUseUuiButton = useUuiButton;
            if (useUuiButton)
            {
                RegisterUui();
                SetPressed(_activationHandler.IsActive);
                return;
            }

            UnregisterUui();
        }
    }
}
