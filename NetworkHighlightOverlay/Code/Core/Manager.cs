using System;
using System.Collections.Generic;
using NetworkHighlightOverlay.Settings;
using UnityEngine;

namespace NetworkHighlightOverlay.Core
{
    public sealed class Manager
    {
        private readonly HighlightCache _cache;
        private readonly OverlayRenderer _renderer;
        private readonly List<KeyValuePair<ushort, Color>> _segmentSnapshot = new List<KeyValuePair<ushort, Color>>(1024);
        private bool _isActive;
        private bool _isCacheDirty = true;

        public Manager(ModSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            _cache = new HighlightCache(settings);
            _renderer = new OverlayRenderer(settings);
        }

        public void OnActivated()
        {
            _isActive = true;
            if (!_isCacheDirty) return;

            _cache.RebuildCache();
            _isCacheDirty = false;
        }

        public void OnDeactivated()
        {
            _isActive = false;
        }

        public void ResetForLevelUnload()
        {
            _cache.Clear();
            _isActive = false;
            _isCacheDirty = true;
        }

        public void OnHighlightRulesChanged()
        {
            if (MarkDirtyIfInactive()) return;

            _cache.RebuildCache();
            _isCacheDirty = false;
        }

        public void OnSegmentCreated(ushort segmentId)
        {
            if (MarkDirtyIfInactive()) return;

            _cache.OnSegmentCreated(segmentId);
        }

        public void OnSegmentReleased(ushort segmentId)
        {
            if (MarkDirtyIfInactive()) return;

            _cache.OnSegmentReleased(segmentId);
        }

        public void RenderIfActive(RenderManager.CameraInfo cameraInfo)
        {
            if (!_isActive) return;

            _cache.CopySegmentsTo(_segmentSnapshot);
            _renderer.Render(cameraInfo, _segmentSnapshot);
        }

        private bool MarkDirtyIfInactive() => !_isActive && (_isCacheDirty = true);
    }
}
