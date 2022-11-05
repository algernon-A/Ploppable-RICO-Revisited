// <copyright file="BuildingData.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using AlgernonCommons.Utils;
    using ColossalFramework.UI;

    /// <summary>
    /// Ploppable RICO data object definition for the dictionary.
    /// Every Ploppable RICO building has one entry, with (in turn) up to three PloppableRICODef sub-entries (local, author, and/or mod).
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Basic data class")]
    public class BuildingData
    {
        /// <summary>
        /// Active building prefab.
        /// </summary>
        public readonly BuildingInfo Prefab;

        /// <summary>
        /// Building name.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Building UI category.
        /// </summary>
        public Category Category;

        /// <summary>
        /// Original prefab.
        /// </summary>
        public BuildingInfo OriginalPrefab;

        /// <summary>
        /// Local RICO settings.
        /// </summary>
        public RICOBuilding Local;

        /// <summary>
        /// Author RICO settings.
        /// </summary>
        public RICOBuilding Author;

        /// <summary>
        /// Mod RICO settings.
        /// </summary>
        public RICOBuilding Mod;

        /// <summary>
        /// Indicates whether this building has local RICO settings.
        /// </summary>
        public bool HasLocal;

        /// <summary>
        /// Indicates whether this building has author RICO settings.
        /// </summary>
        public bool HasAuthor;

        /// <summary>
        /// Indicates whether this building has mod RICO settings.
        /// </summary>
        public bool HasMod;

        /// <summary>
        /// Building thumbnails.
        /// </summary>
        public UITextureAtlas ThumbnailAtlas;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingData"/> class.
        /// </summary>
        /// <param name="prefab">Building prefab to assign.</param>
        public BuildingData(BuildingInfo prefab)
        {
            Prefab = prefab;
            Name = prefab.name;
            Category = AssignCategory(prefab);
        }

        /// <summary>
        /// Gets the currently active RICO settings for this building (null if none).
        /// </summary>
        public RICOBuilding ActiveSetting
        {
            get
            {
                // In order of priority.
                if (HasLocal)
                {
                    return Local;
                }
                else if (HasAuthor)
                {
                    return Author;
                }
                else if (HasMod)
                {
                    return Mod;
                }

                // If we got here, there's no active settings.
                return null;
            }
        }

        /// <summary>
        /// Gets the display name for this prefab.
        /// </summary>
        public string DisplayName => PrefabUtils.GetDisplayName(Prefab);

        /// <summary>
        /// Assigns settings panel categories for prefabs.
        /// </summary>
        /// <param name="prefab">The relevant prefab.</param>
        /// <returns>Assigned category for this prefab.</returns>
        private Category AssignCategory(BuildingInfo prefab)
        {
            switch (prefab.GetService())
            {
                case ItemClass.Service.Monument:
                    return Category.Monument;

                case ItemClass.Service.Electricity:
                    return Category.Power;

                case ItemClass.Service.Water:
                    return Category.Water;

                case ItemClass.Service.Education:
                    return Category.Education;

                case ItemClass.Service.PlayerEducation:
                    return Category.Education;

                case ItemClass.Service.HealthCare:
                    return Category.Health;

                case ItemClass.Service.PoliceDepartment:
                    return Category.Police;

                case ItemClass.Service.FireDepartment:
                    return Category.Fire;

                case ItemClass.Service.Residential:
                    return Category.Residential;

                case ItemClass.Service.Industrial:
                    return Category.Industrial;

                case ItemClass.Service.PlayerIndustry:
                    return Category.PlayerIndustry;

                case ItemClass.Service.Fishing:
                    return Category.PlayerIndustry;

                case ItemClass.Service.Garbage:
                    return Category.Garbage;

                case ItemClass.Service.Office:
                    return Category.Office;

                case ItemClass.Service.Commercial:
                    return Category.Commercial;

                case ItemClass.Service.Beautification:
                default:
                    return Category.Beautification;
            }
        }
    }
}