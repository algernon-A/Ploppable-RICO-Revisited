// <copyright file="CategoryUtils.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using AlgernonCommons.Translation;

    /// <summary>
    /// Building selection filter categories.
    /// </summary>
    public enum Category
    {
        /// <summary>
        /// No selection.
        /// </summary>
        None = -1,
        
        /// <summary>
        /// Monuments, unique buildings, and miscellaneous.
        /// </summary>
        Monument,

        /// <summary>
        /// Beautification buildings.
        /// </summary>
        Beautification,

        /// <summary>
        /// Residential buildings.
        /// </summary>
        Residential,

        /// <summary>
        /// Commercial buildings.
        /// </summary>
        Commercial,

        /// <summary>
        /// Office buildings.
        /// </summary>
        Office,

        /// <summary>
        /// Industrial buildings.
        /// </summary>
        Industrial,

        /// <summary>
        /// Education buildings.
        /// </summary>
        Education,

        /// <summary>
        /// Health and deathcare buildings.
        /// </summary>
        Health,

        /// <summary>
        /// Fire service buildings.
        /// </summary>
        Fire,

        /// <summary>
        /// Police service buildings.
        /// </summary>
        Police,

        /// <summary>
        /// Electricity buildings.
        /// </summary>
        Power,

        /// <summary>
        /// Water, sewage, and heating buildings.
        /// </summary>
        Water,

        /// <summary>
        /// Garbage processing buildings.
        /// </summary>
        Garbage,

        /// <summary>
        /// Industries DLC buildings.
        /// </summary>
        PlayerIndustry,

        /// <summary>
        /// Number of categories.
        /// </summary>
        NumCategories
    }

    /// <summary>
    /// Data class for original building categories (to sort settings panel building list).
    /// </summary>
    internal static class OriginalCategories
    {
        // Icons representing each category.

        /// <summary>
        /// Sprite icon names for each category.
        /// </summary>
        internal static readonly string[] SpriteNames =
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

        /// <summary>
        /// Sprite icon atlas names for each category.
        /// </summary>
        internal static readonly string[] Atlases =
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

        /// <summary>
        /// Tooltips for each category.
        /// </summary>
        internal static readonly string[] TooltipKeys =
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
    /// Ploppable RICO Revisited UI category names (translated keys) for display.
    /// </summary>
    internal class UICategories
    {
        /// <summary>
        /// UI Category names.
        /// </summary>
        internal readonly string[] Names =
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