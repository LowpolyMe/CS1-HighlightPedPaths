namespace NetworkHighlightOverlay.HighlightCategories
{
    public sealed class HighlightCategoryDefinition
    {
        public HighlightCategoryDefinition(
            HighlightCategoryId id,
            string label,
            string spriteName,
            SteamHelper.DLC requiredDlc,
            HighlightCategoryGroup categoryGroup)
        {
            Id = id;
            Label = label;
            SpriteName = spriteName;
            RequiredDlc = requiredDlc;
            CategoryGroup = categoryGroup;
        }

        public HighlightCategoryId Id { get; }
        public string Label { get; }
        public string SpriteName { get; }
        public SteamHelper.DLC RequiredDlc { get; }
        public HighlightCategoryGroup CategoryGroup { get; }
    }
}
