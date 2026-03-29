using System;
using NetworkHighlightOverlay.HighlightCategories;
using NetworkHighlightOverlay.Settings;
using UnityEngine;

namespace NetworkHighlightOverlay.Core
{
    public static class HighlightRules
    {
        private const string PinkPathNetworkName = "Pedestrian Connection";
        private const string TerraformingToken = "terraforming";
        private const string PedestrianStreetClassName = "Pedestrian Street";

        public static HighlightSelection.SegmentFlags GetSegmentFlags(NetInfo info)
        {
            if (info == null)
                return default(HighlightSelection.SegmentFlags);

            NetAI ai = info.m_netAI;
            if (ai == null)
                return default(HighlightSelection.SegmentFlags);

            const VehicleInfo.VehicleType tramLikeMask = VehicleInfo.VehicleType.Tram | VehicleInfo.VehicleType.Trolleybus;

            bool isRoadFamily = ai is RoadAI || ai is RoadBridgeAI || ai is RoadTunnelAI;
            bool isRoadBridge = ai is RoadBridgeAI;
            bool isRoadTunnel = ai is RoadTunnelAI;
            bool isPedestrianStreet = false;
            bool isHighway = false;
            bool hasTramOrTrolleyLanes = false;
            bool hasMonorailLanes = false;
            bool hasCarLanes = false;
            bool isRaceRoad = false;
            bool isEventRoad = false;
            bool isPitLane = false;
            bool isAirportTaxiway = ai is AirportAreaTaxiwayAI;
            bool isAirportRunway = ai is AirportAreaRunwayAI;

            if (isRoadFamily)
            {
                VehicleInfo.VehicleType laneVehicleTypes = GetLaneVehicleTypes(info);
                ItemClass.Service service = info.GetService();
                ItemClass.Level classLevel = info.GetClassLevel();
                bool isRaceService = service == ItemClass.Service.Race;

                isPedestrianStreet = IsPedestrianStreet(info);
                isHighway = ai.IsHighway();
                hasTramOrTrolleyLanes = (laneVehicleTypes & tramLikeMask) != 0;
                hasMonorailLanes = (laneVehicleTypes & VehicleInfo.VehicleType.Monorail) != 0;
                hasCarLanes = (laneVehicleTypes & VehicleInfo.VehicleType.Car) != 0;
                isRaceRoad = isRaceService && classLevel == ItemClass.Level.Level4;
                isEventRoad = isRaceService && classLevel == ItemClass.Level.Level3;
                isPitLane = isRaceService &&
                            (classLevel == ItemClass.Level.Level2 || classLevel == ItemClass.Level.Level1);
            }

            return new HighlightSelection.SegmentFlags
            {
                IsPinkPath = IsPinkPath(info, ai),
                IsTerraformingNetwork = IsTerraformingNetwork(info),
                IsPedestrianPath = ai is PedestrianPathAI || ai is PedestrianWayAI || ai is PedestrianZoneRoadAI,
                IsPedestrianBridge = ai is PedestrianBridgeAI || ai is PedestrianZoneBridgeAI,
                IsPedestrianTunnel = ai is PedestrianTunnelAI,
                IsTrainTrack = ai is TrainTrackAI,
                IsTrainBridge = ai is TrainTrackBridgeAI,
                IsTrainTunnel = ai is TrainTrackTunnelAI,
                IsMetroTrack = ai is MetroTrackAI,
                IsMetroBridge = ai is MetroTrackBridgeAI,
                IsMetroTunnel = ai is MetroTrackTunnelAI,
                IsMonorailTrack = ai is MonorailTrackAI,
                IsCableCarPath = ai is CableCarPathAI,
                IsRaceRoad = isRaceRoad,
                IsEventRoad = isEventRoad,
                IsPitLane = isPitLane,
                IsAirportTaxiway = isAirportTaxiway,
                IsAirportRunway = isAirportRunway,
                IsRoadFamily = isRoadFamily,
                IsRoadBridge = isRoadBridge,
                IsRoadTunnel = isRoadTunnel,
                IsPedestrianStreet = isPedestrianStreet,
                IsHighway = isHighway,
                HasTramOrTrolleyLanes = hasTramOrTrolleyLanes,
                HasMonorailLanes = hasMonorailLanes,
                HasCarLanes = hasCarLanes
            };
        }

        public static bool TryGetHighlightColor(HighlightSelection.SegmentFlags flags, ModSettings settings, out Color color)
        {
            color = default(Color);
            if (settings == null) return false;

            HighlightCategoryId categoryId;
            bool isBridge;
            bool isTunnel;

            bool didSelect = HighlightSelection.TrySelectCategory(
                flags,
                settings.GetCategoryEnabled,
                out categoryId,
                out isBridge,
                out isTunnel);

            if (!didSelect) return false;

            bool isEnabled = HighlightSelection.IsCategoryEnabledForSegment(
                categoryId,
                isBridge,
                isTunnel,
                settings.HighlightBridges,
                settings.HighlightTunnels,
                settings.GetCategoryEnabled);

            if (!isEnabled) return false;

            color = settings.GetCategoryColor(categoryId);
            return true;
        }

        private static bool IsPinkPath(NetInfo info, NetAI ai)
        {
            return ai is PedestrianPathAI &&
                   string.Equals(info.name, PinkPathNetworkName, StringComparison.Ordinal);
        }

        private static bool IsTerraformingNetwork(NetInfo info)
        {
            string infoName = info.name;
            return !string.IsNullOrEmpty(infoName) &&
                   infoName.IndexOf(TerraformingToken, StringComparison.OrdinalIgnoreCase) >= 0 &&
                   info.m_flattenTerrain;
        }

        private static bool IsPedestrianStreet(NetInfo info)
        {
            if (info.m_class == null) return false;

            string className = info.m_class.name;
            return !string.IsNullOrEmpty(className) &&
                   string.Equals(className, PedestrianStreetClassName, StringComparison.Ordinal);
        }

        private static VehicleInfo.VehicleType GetLaneVehicleTypes(NetInfo info)
        {
            if (info.m_lanes == null) return VehicleInfo.VehicleType.None;

            VehicleInfo.VehicleType laneVehicleTypes = VehicleInfo.VehicleType.None;
            foreach (NetInfo.Lane lane in info.m_lanes)
            {
                laneVehicleTypes |= lane.m_vehicleType;
            }

            return laneVehicleTypes;
        }
    }
}
