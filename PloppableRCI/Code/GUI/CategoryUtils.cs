namespace PloppableRICO
{
    using AlgernonCommons.Translation;

    public enum Category
    {
        None = -1,
        Monument,
        Beautification,
        Residential,
        Commercial,
        Office,
        Industrial,
        Education,
        Health,
        Fire,
        Police,
        Power,
        Water,
        Garbage,
        PlayerIndustry,
        NumCategories
    }


    /// <summary>
    /// Data struct for original building categories (to sort settings panel building list).
    /// </summary>
    internal static class OriginalCategories
    {
        // Icons representing each category.
        internal static readonly string[] spriteNames =
        {
            "ToolbarIconMonuments",
            "ToolbarIconBeautification",
            "ZoningResidentialHigh",
            "ZoningCommercialHigh",
            "ZoningOffice" ,
            "ZoningIndustrial",
            "ToolbarIconEducation",
            "ToolbarIconHealthcare",
            "ToolbarIconFireDepartment",
            "ToolbarIconPolice",
            "ToolbarIconElectricity",
            "ToolbarIconWaterAndSewage",
            "InfoIconGarbage",
            "ToolbarIconPlayerIndustry",
        };

        // Icon atlas name for each of the above sprites.
        internal static readonly string[] atlases =
        {
            "Ingame",
            "Ingame" ,
            "Thumbnails",
            "Thumbnails",
            "Thumbnails",
            "Thumbnails",
            "Ingame",
            "Ingame",
            "Ingame",
            "Ingame",
            "Ingame",
            "Ingame",
            "Ingame",
            "Ingame"
        };

        // Tooltips.
        internal static readonly string[] tooltipKeys =
        {
            "PRR_CAT_MON",
            "PRR_CAT_BEA",
            "PRR_CAT_RES",
            "PRR_CAT_COM",
            "PRR_CAT_OFF",
            "PRR_CAT_IND",
            "PRR_CAT_EDU",
            "PRR_CAT_HEA",
            "PRR_CAT_FIR",
            "PRR_CAT_POL",
            "PRR_CAT_POW",
            "PRR_CAT_WAT",
            "PRR_CAT_GAR",
            "PRR_CAT_PIN"
        };
    }



    /// <summary>
    /// Data struct for Ploppable RICO Revisited UI category names.
    /// </summary>
    internal class UICategories
    {
        internal readonly string[] names =
        {
            Translations.Translate("PRR_UIC_REL"),
            Translations.Translate("PRR_UIC_REH"),
            Translations.Translate("PRR_UIC_COL"),
            Translations.Translate("PRR_UIC_COH"),
            Translations.Translate("PRR_UIC_OFF"),
            Translations.Translate("PRR_UIC_IND"),
            Translations.Translate("PRR_UIC_FAR"),
            Translations.Translate("PRR_UIC_FOR"),
            Translations.Translate("PRR_UIC_OIL"),
            Translations.Translate("PRR_UIC_ORE"),
            Translations.Translate("PRR_UIC_LEI"),
            Translations.Translate("PRR_UIC_TOU"),
            Translations.Translate("PRR_UIC_ORG"),
            Translations.Translate("PRR_UIC_ITC"),
            Translations.Translate("PRR_UIC_ECO"),
            Translations.Translate("PRR_UIC_NON")
        };
    }
}