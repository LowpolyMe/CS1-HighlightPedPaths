using System.Linq;

namespace NetworkHighlightOverlay.HighlightCategories
{
    public static class HighlightCategoryCatalog
    {
        public static readonly HighlightCategoryDefinition[] All = new HighlightCategoryDefinition[]
        {
            new HighlightCategoryDefinition(
                HighlightCategoryId.PedestrianPaths,
                "Pedestrian paths",
                "SubBarBeautificationPedestrianZoneEssentials",
                SteamHelper.DLC.None,
                HighlightCategoryGroup.RoadsAndPaths),
            new HighlightCategoryDefinition(
                HighlightCategoryId.Roads,
                "Roads",
                "SubBarRoadsSmall",
                SteamHelper.DLC.None,
                HighlightCategoryGroup.RoadsAndPaths),
            new HighlightCategoryDefinition(
                HighlightCategoryId.Highways,
                "Highways",
                "SubBarRoadsHighway",
                SteamHelper.DLC.None,
                HighlightCategoryGroup.RoadsAndPaths),
            new HighlightCategoryDefinition(
                HighlightCategoryId.TrainTracks,
                "Train tracks",
                "SubBarPublicTransportTrain",
                SteamHelper.DLC.None,
                HighlightCategoryGroup.PublicTransport),
            new HighlightCategoryDefinition(
                HighlightCategoryId.MetroTracks,
                "Metro tracks",
                "SubBarPublicTransportMetro",
                SteamHelper.DLC.None,
                HighlightCategoryGroup.PublicTransport),
            new HighlightCategoryDefinition(
                HighlightCategoryId.TramTracks,
                "Tram and Trolley tracks",
                "SubBarPublicTransportTram",
                SteamHelper.DLC.None,
                HighlightCategoryGroup.PublicTransport),
            new HighlightCategoryDefinition(
                HighlightCategoryId.MonorailTracks,
                "Monorail tracks",
                "SubBarPublicTransportMonorail",
                SteamHelper.DLC.None,
                HighlightCategoryGroup.PublicTransport),
            new HighlightCategoryDefinition(
                HighlightCategoryId.CableCars,
                "Cable car paths",
                "SubBarPublicTransportCableCar",
                SteamHelper.DLC.None,
                HighlightCategoryGroup.PublicTransport),
            new HighlightCategoryDefinition(
                HighlightCategoryId.PinkPaths,
                "Pink paths",
                "SubBarRoadsMaintenance",
                SteamHelper.DLC.None,
                HighlightCategoryGroup.SpecialNetworks),
            new HighlightCategoryDefinition(
                HighlightCategoryId.TerraformingNetworks,
                "Terraforming networks",
                "ToolbarIconLandscaping",
                SteamHelper.DLC.None,
                HighlightCategoryGroup.SpecialNetworks),
            new HighlightCategoryDefinition(
                HighlightCategoryId.RaceRoads,
                "Races and Parades",
                "SubBarRoadsRacesAndParades",
                SteamHelper.DLC.RacesAndParadesDLC,
                HighlightCategoryGroup.SpecialNetworks),
            new HighlightCategoryDefinition(
                HighlightCategoryId.EventRoads,
                "Event roads",
                "SubBarRoadsRacesAndParades",
                SteamHelper.DLC.RacesAndParadesDLC,
                HighlightCategoryGroup.SpecialNetworks),
            new HighlightCategoryDefinition(
                HighlightCategoryId.AirportRoads,
                "Airport road",
                "SubBarPublicTransportAirportArea",
                SteamHelper.DLC.AirportDLC,
                HighlightCategoryGroup.SpecialNetworks)
        };

        public static HighlightCategoryDefinition[] GetAllEligible()
        {
            return All
                .Where(IsEligible)
                .ToArray();
        }

        public static HighlightCategoryDefinition[] GetEligible(HighlightCategoryGroup categoryGroup)
        {
            return All
                .Where(definition => definition.CategoryGroup == categoryGroup)
                .Where(IsEligible)
                .ToArray();
        }

        private static bool IsEligible(HighlightCategoryDefinition definition)
        {
            return definition.RequiredDlc == SteamHelper.DLC.None ||
                   SteamHelper.IsDLCOwned(definition.RequiredDlc);
        }
    }
}
