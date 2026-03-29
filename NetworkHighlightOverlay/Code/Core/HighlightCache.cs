using System;
using System.Collections.Generic;
using NetworkHighlightOverlay.Settings;
using UnityEngine;

namespace NetworkHighlightOverlay.Core
{
    public sealed class HighlightCache
    {
        private readonly ModSettings _settings;
        private readonly Dictionary<ushort, Color> _highlightedSegments = new Dictionary<ushort, Color>();
        private readonly Dictionary<NetInfo, HighlightSelection.SegmentFlags> _segmentFlagsByInfo =
            new Dictionary<NetInfo, HighlightSelection.SegmentFlags>();

        public HighlightCache(ModSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            _settings = settings;
        }

        public void CopySegmentsTo(List<KeyValuePair<ushort, Color>> destination)
        {
            if (destination == null)
                throw new ArgumentNullException("destination");

            destination.Clear();

            lock (_highlightedSegments)
            {
                int count = _highlightedSegments.Count;
                if (destination.Capacity < count)
                    destination.Capacity = count;

                foreach (KeyValuePair<ushort, Color> entry in _highlightedSegments)
                {
                    destination.Add(entry);
                }
            }
        }

        public void Clear()
        {
            lock (_highlightedSegments)
            {
                _highlightedSegments.Clear();
                _segmentFlagsByInfo.Clear();
            }
        }

        public void RebuildCache()
        {
            lock (_highlightedSegments)
            {
                _highlightedSegments.Clear();
                if (!_settings.HasAnyCategoryEnabled)
                    return;

                NetManager netManager = NetManager.instance;
                Array16<NetSegment> segments = netManager.m_segments;

                for (ushort i = 1; i < segments.m_size; i++)
                {
                    ref NetSegment segment = ref segments.m_buffer[i];

                    if ((segment.m_flags & NetSegment.Flags.Created) == 0)
                        continue;

                    TryAddSegmentInternal(i, ref segment);
                }
            }
        }

        public void OnSegmentCreated(ushort segmentId)
        {
            if (segmentId == 0 || !_settings.HasAnyCategoryEnabled)
                return;

            ref NetSegment segment = ref NetManager.instance.m_segments.m_buffer[segmentId];
            if ((segment.m_flags & NetSegment.Flags.Created) == 0)
                return;

            lock (_highlightedSegments)
            {
                TryAddSegmentInternal(segmentId, ref segment);
            }
        }

        public void OnSegmentReleased(ushort segmentId)
        {
            if (segmentId == 0)
                return;

            lock (_highlightedSegments)
            {
                _highlightedSegments.Remove(segmentId);
            }
        }

        private void TryAddSegmentInternal(ushort id, ref NetSegment segment)
        {
            NetInfo info = segment.Info;
            if (info == null)
                return;

            HighlightSelection.SegmentFlags flags;
            if (!_segmentFlagsByInfo.TryGetValue(info, out flags))
            {
                flags = HighlightRules.GetSegmentFlags(info);
                _segmentFlagsByInfo[info] = flags;
            }

            Color color;
            if (HighlightRules.TryGetHighlightColor(flags, _settings, out color))
            {
                _highlightedSegments[id] = color;
            }
        }
    }
}
