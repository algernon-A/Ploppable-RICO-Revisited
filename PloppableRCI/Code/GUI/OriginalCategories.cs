// <copyright file="OriginalCategories.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
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
        NumCategories,
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
            "ZoningOffice",
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
            "Ingame",
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
            "Ingame",
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
            "PRR_CAT_PIN",
        };
    }
}