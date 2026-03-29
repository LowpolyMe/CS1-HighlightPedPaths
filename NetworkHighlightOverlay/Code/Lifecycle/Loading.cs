using System;
using ColossalFramework.UI;
using HarmonyLib;
using ICities;
using NetworkHighlightOverlay.Core;
using NetworkHighlightOverlay.GUI.TogglePanel;
using NetworkHighlightOverlay.GUI.UUI;
using NetworkHighlightOverlay.Settings;
using UnityEngine;

namespace NetworkHighlightOverlay.Lifecycle
{
    public class Loading : LoadingExtensionBase
    {
        private readonly ModSettings _settings = ModSettings.Shared;
        private Manager _manager;
        private UuiButtonController _uuiButtonController;
        private ToggleButtonAtlas _toggleButtonAtlas;
        private GameObject _controllerObject;
        private ActivationHandler _activationHandler;
        private TogglePanel _togglePanel;
        private Harmony _harmony;
        
        private const string HarmonyId = "com.lowpolyme.NetworkHighlightOverlay";

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            PatchHarmony();
        }

        public override void OnReleased()
        {
            base.OnReleased();
            ReleaseRuntime();
            UnpatchHarmony();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            CreateRuntime();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            ReleaseRuntime();
        }

        private void PatchHarmony()
        {
            if (_harmony != null)
                return;
            
            _harmony = new Harmony(HarmonyId);
            _harmony.PatchAll();
        }

        private void UnpatchHarmony()
        {
            if (_harmony != null)
            {
                _harmony.UnpatchAll(HarmonyId);
                _harmony = null;
            }
        }

        private void CreateRuntime()
        {
            ReleaseRuntime();

            _manager = new Manager(_settings);
            RuntimeHooks.Attach(_manager);
            _uuiButtonController = new UuiButtonController();
            _toggleButtonAtlas = new ToggleButtonAtlas();
            _controllerObject = new GameObject("PathHighlightRenderer");
            _activationHandler = _controllerObject.AddComponent<ActivationHandler>();
            _activationHandler.Initialize(_manager, _settings);
            _uuiButtonController.Initialize(_settings, _activationHandler);
            GameObject.DontDestroyOnLoad(_controllerObject);

            UIView view = UIView.GetAView();
            if (view == null)
                throw new InvalidOperationException("Loading requires an active UIView before creating the toggle panel.");

            TogglePanel panel = view.AddUIComponent(typeof(TogglePanel)) as TogglePanel;
            if (panel == null)
                throw new InvalidOperationException("Failed to create the toggle panel.");

            panel.isVisible = false;
            panel.Initialize(_settings, _activationHandler, _toggleButtonAtlas);
            _togglePanel = panel;
        }

        private void ReleaseRuntime()
        {
            if (_togglePanel != null)
            {
                UnityEngine.Object.Destroy(_togglePanel.gameObject);
                _togglePanel = null;
            }

            if (_controllerObject != null)
            {
                if (_uuiButtonController != null)
                {
                    _uuiButtonController.Dispose();
                }

                UnityEngine.Object.Destroy(_controllerObject);
                _controllerObject = null;
            }

            _activationHandler = null;
            RuntimeHooks.Detach();

            if (_toggleButtonAtlas != null)
            {
                _toggleButtonAtlas.Dispose();
                _toggleButtonAtlas = null;
            }

            if (_manager != null)
            {
                _manager.ResetForLevelUnload();
                _manager = null;
            }

            _uuiButtonController = null;
        }
    }
}
