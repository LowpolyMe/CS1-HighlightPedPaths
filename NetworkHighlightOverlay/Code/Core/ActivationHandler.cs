using System;
using NetworkHighlightOverlay.Settings;
using UnityEngine;

namespace NetworkHighlightOverlay.Core
{
    public sealed class ActivationHandler : MonoBehaviour
    {
        private Manager _manager;
        private ModSettings _settings;
        private bool _isActive;

        public event Action<bool> ActivationChanged;

        public bool IsActive => _isActive;

        public void Initialize(Manager manager, ModSettings settings)
        {
            _manager = manager ?? throw new ArgumentNullException("manager");
            _settings = settings ?? throw new ArgumentNullException("settings");
        }

        public void SetActive(bool isActive)
        {
            if (_isActive == isActive)
            {
                return;
            }

            _isActive = isActive;
            if (isActive) _manager.OnActivated();
            else _manager.OnDeactivated();

            Action<bool> activationChanged = ActivationChanged;
            if (activationChanged != null)
            {
                activationChanged(isActive);
            }
        }

        private void Start()
        {
            EnsureInitialized();
            _settings.HighlightRulesChanged += _manager.OnHighlightRulesChanged;
        }

        private void Update()
        {
            if (!_settings.ToggleHighlightsHotkey.IsKeyUp()) return;

            SetActive(!IsActive);
        }

        private void OnDestroy()
        {
            SetActive(false);
            if (_settings != null && _manager != null)
            {
                _settings.HighlightRulesChanged -= _manager.OnHighlightRulesChanged;
            }
        }

        private void EnsureInitialized()
        {
            if (_manager == null || _settings == null)
                throw new InvalidOperationException("ActivationHandler.Initialize must be called before Start.");
        }
    }
}
