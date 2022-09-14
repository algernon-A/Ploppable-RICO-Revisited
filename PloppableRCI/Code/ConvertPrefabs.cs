// <copyright file="ConvertPrefabs.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System;
    using AlgernonCommons;

    /// <summary>
    ///This class assigns the RICO settings to the prefabs. 
    /// </summary>
    internal class ConvertPrefabs
    {
        /// <summary>
        /// Interpret and apply RICO settings to a building prefab.
        /// </summary>
        /// <param name="buildingData">RICO building data to apply</param>
        /// <param name="prefab">The building prefab to be changed</param>
        internal void ConvertPrefab(RICOBuilding buildingData, BuildingInfo prefab)
        {
            // AI class  for prefab init.
            string aiClass;


            if (prefab != null)
            {
                // Check eligibility for any growable assets.
                if (buildingData.growable)
                {
                    // Growables can't have any dimension greater than 4.
                    if (prefab.GetWidth() > 4 || prefab.GetLength() > 4)
                    {
                        buildingData.growable = false;
                        Logging.Error("building '", prefab.name, "' can't be growable because it is too big");
                    }

                    // Growables can't have net structures.
                    if (prefab.m_paths != null && prefab.m_paths.Length != 0)
                    {
                        buildingData.growable = false;
                        Logging.Error("building '", prefab.name, "' can't be growable because it contains network assets");
                    }
                }

                // Apply AI based on service.
                switch (buildingData.service)
                {
                    // Dummy AI.
                    case "dummy":

                        // Get AI.
                        DummyBuildingAI dummyAI = prefab.gameObject.AddComponent<DummyBuildingAI>();

                        // Use beautification ItemClass to avoid issues, and never make growable.
                        InitializePrefab(prefab, dummyAI, "Beautification Item", false);

                        // Final circular reference.
                        prefab.m_buildingAI.m_info = prefab;

                        // Dummy is a special case, and we're done here.
                        return;

                    // Residential AI.
                    case "residential":

                        // Get AI.
                        GrowableResidentialAI residentialAI = buildingData.growable ? prefab.gameObject.AddComponent<GrowableResidentialAI>() : prefab.gameObject.AddComponent<PloppableResidentialAI>();
                        if (residentialAI == null)
                        {
                            throw new Exception("Ploppable RICO residential AI not found.");
                        }

                        // Assign basic parameters.
                        residentialAI.m_ricoData = buildingData;
                        residentialAI.m_constructionCost = buildingData.ConstructionCost;
                        residentialAI.m_homeCount = buildingData.homeCount;

                        // Determine AI class string according to subservice.
                        switch (buildingData.subService)
                        {
                            case "low eco":
                                // Apply eco service if GC installed, otherwise use normal low residential.
                                if (Util.IsGCinstalled())
                                {
                                    aiClass = "Low Residential Eco - Level";
                                }
                                else
                                {
                                    aiClass = "Low Residential - Level";
                                }
                                break;

                            case "high eco":
                                // Apply eco service if GC installed, otherwise use normal high residential.
                                if (Util.IsGCinstalled())
                                {
                                    aiClass = "High Residential Eco - Level";
                                }
                                else
                                {
                                    aiClass = "High Residential - Level";
                                }
                                break;

                            case "high":
                                // Stock standard high commercial.
                                aiClass = "High Residential - Level";
                                break;

                            case "wall2wall":
                                // Wall-to-wall - requires Plazas & Promenades.
                                if (Util.IsPPinstalled())
                                {
                                    // Need to do W2W manually as the ItemClassCollection may not have loaded the expansion when this is called.
                                    ItemClass itemClass = new ItemClass()
                                    {
                                        m_service = ItemClass.Service.Residential,
                                        m_subService = ItemClass.SubService.ResidentialWallToWall,
                                        m_level = (ItemClass.Level)(buildingData.level - 1),
                                    };

                                    InitializePrefab(prefab, residentialAI, itemClass, buildingData.growable);
                                    return;
                                }

                                // Plazas & Promenades not installed - fall back to high residential.
                                aiClass = "High Residential - Level";
                                break;

                            default:
                                // Fall back to low residential as default.
                                aiClass = "Low Residential - Level";

                                // If invalid subservice, report.
                                if (buildingData.subService != "low")
                                {
                                    Logging.Message("Residential building ", buildingData.Name, " has invalid subservice ", buildingData.subService, "; reverting to low residential");
                                }
                                break;
                        }

                        // Initialize the prefab.
                        InitializePrefab(prefab, residentialAI, aiClass + buildingData.level, buildingData.growable);

                        break;

                    // Office AI.
                    case "office":

                        // Get AI.
                        GrowableOfficeAI officeAI = buildingData.growable ? prefab.gameObject.AddComponent<GrowableOfficeAI>() : prefab.gameObject.AddComponent<PloppableOfficeAI>();
                        if (officeAI == null)
                        {
                            throw new Exception("Ploppable RICO Office AI not found.");
                        }

                        // Assign basic parameters.
                        officeAI.m_ricoData = buildingData;
                        officeAI.m_workplaceCount = buildingData.WorkplaceCount;
                        officeAI.m_constructionCost = buildingData.ConstructionCost;

                        // Determine AI class string according to subservice.
                        switch (buildingData.subService)
                        {
                            case "high tech":
                                // Apply IT cluster if GC installed, otherwise use Level 3 office.
                                if (Util.IsGCinstalled())
                                {
                                    aiClass = "Office - Hightech";
                                }
                                else
                                {
                                    aiClass = "Office - Level3";
                                }
                                break;

                            case "wall2wall":
                                // Wall-to-wall - requires Plazas & Promenades.
                                if (Util.IsPPinstalled())
                                {
                                    // Need to do W2W manually as the ItemClassCollection may not have loaded the expansion when this is called.
                                    ItemClass itemClass = new ItemClass()
                                    {
                                        m_service = ItemClass.Service.Office,
                                        m_subService = ItemClass.SubService.OfficeWallToWall,
                                        m_level = (ItemClass.Level)(buildingData.level - 1),
                                    };

                                    InitializePrefab(prefab, officeAI, itemClass, buildingData.growable);
                                    return;
                                }

                                // Plazas & Promenades not installed - fall back to standard office.
                                aiClass = "Office - Level" + buildingData.level;
                                break;

                            default:
                                // Boring old ordinary office.
                                aiClass = "Office - Level" + buildingData.level;
                                break;
                        }

                        // Initialize the prefab.
                        InitializePrefab(prefab, officeAI, aiClass, buildingData.growable);

                        break;

                    // Industrial AI.
                    case "industrial":
                        // Get AI.
                        GrowableIndustrialAI industrialAI = buildingData.growable ? prefab.gameObject.AddComponent<GrowableIndustrialAI>() : prefab.gameObject.AddComponent<PloppableIndustrialAI>();
                        if (industrialAI == null)
                        {
                            throw new Exception("Ploppable RICO Industrial AI not found.");
                        }

                        // Assign basic parameters.
                        industrialAI.m_ricoData = buildingData;
                        industrialAI.m_workplaceCount = buildingData.WorkplaceCount;
                        industrialAI.m_constructionCost = buildingData.ConstructionCost;
                        industrialAI.m_pollutionEnabled = buildingData.pollutionEnabled;

                        // Determine AI class string according to subservice.
                        // Check for valid subservice.
                        if (IsValidIndSubServ(buildingData.subService))
                        {
                            // Specialised industry.
                            aiClass = ServiceName(buildingData.subService) + " - Processing";
                        }
                        else
                        {
                            // Generic industry.
                            aiClass = "Industrial - Level" + buildingData.level;
                        }

                        // Initialize the prefab.
                        InitializePrefab(prefab, industrialAI, aiClass, buildingData.growable);

                        break;

                    // Extractor AI.
                    case "extractor":
                        // Get AI.
                        GrowableExtractorAI extractorAI = buildingData.growable ? prefab.gameObject.AddComponent<GrowableExtractorAI>() : prefab.gameObject.AddComponent<PloppableExtractorAI>();
                        if (extractorAI == null)
                        {
                            throw new Exception("Ploppable RICO Extractor AI not found.");
                        }

                        // Assign basic parameters.
                        extractorAI.m_ricoData = buildingData;
                        extractorAI.m_workplaceCount = buildingData.WorkplaceCount;
                        extractorAI.m_constructionCost = buildingData.ConstructionCost;
                        extractorAI.m_pollutionEnabled = buildingData.pollutionEnabled;

                        // Check that we have a valid industry subservice.
                        if (IsValidIndSubServ(buildingData.subService))
                        {
                            // Initialise the prefab.
                            InitializePrefab(prefab, extractorAI, ServiceName(buildingData.subService) + " - Extractor", buildingData.growable);
                        }
                        else
                        {
                            Logging.Error("invalid industry subservice ", buildingData.subService, " for extractor ", buildingData.Name);
                        }

                        break;

                    // Commercial AI.
                    case "commercial":
                        // Get AI.
                        GrowableCommercialAI commercialAI = buildingData.growable ? prefab.gameObject.AddComponent<GrowableCommercialAI>() : prefab.gameObject.AddComponent<PloppableCommercialAI>();
                        if (commercialAI == null)
                        {
                            throw new Exception("Ploppable RICO Commercial AI not found.");
                        }

                        // Assign basic parameters.
                        commercialAI.m_ricoData = buildingData;
                        commercialAI.m_workplaceCount = buildingData.WorkplaceCount;
                        commercialAI.m_constructionCost = buildingData.ConstructionCost;

                        // Determine AI class string according to subservice.
                        switch (buildingData.subService)
                        {
                            // Organic and Local Produce.
                            case "eco":
                                // Apply eco specialisation if GC installed, otherwise use Level 1 low commercial.
                                if (Util.IsGCinstalled())
                                {
                                    // Eco commercial buildings only import food goods.
                                    commercialAI.m_incomingResource = TransferManager.TransferReason.Food;
                                    aiClass = "Eco Commercial";
                                }
                                else
                                {
                                    aiClass = "Low Commercial - Level1";
                                }
                                break;

                            // Tourism.
                            case "tourist":
                                // Apply tourist specialisation if AD installed, otherwise use Level 1 low commercial.
                                if (Util.IsADinstalled())
                                {
                                    aiClass = "Tourist Commercial - Land";
                                }
                                else
                                {
                                    aiClass = "Low Commercial - Level1";
                                }
                                break;

                            // Leisure.
                            case "leisure":
                                // Apply leisure specialisation if AD installed, otherwise use Level 1 low commercial.
                                if (Util.IsADinstalled())
                                {
                                    aiClass = "Leisure Commercial";
                                }
                                else
                                {
                                    aiClass = "Low Commercial - Level1";
                                }
                                break;

                            // Wall-to-wall - requires Plazas & Promenades.
                            case "wall2wall":
                                if (Util.IsPPinstalled())
                                {
                                    // Need to do W2W manually as the ItemClassCollection may not have loaded the expansion when this is called.
                                    ItemClass itemClass = new ItemClass()
                                    {
                                        m_service = ItemClass.Service.Commercial,
                                        m_subService = ItemClass.SubService.CommercialWallToWall,
                                        m_level = (ItemClass.Level)(buildingData.level - 1),
                                    };

                                    InitializePrefab(prefab, commercialAI, itemClass, buildingData.growable);
                                    return;
                                }

                                // Plazas & Promenades not installed - fall back to high commercial.
                                aiClass = "High Commercial - Level" + buildingData.level;
                                break;

                            // Bog standard high commercial.
                            case "high":
                                aiClass = "High Commercial - Level" + buildingData.level;
                                break;

                            // Fall back to low commercial as default.
                            default:
                                aiClass = "Low Commercial - Level" + buildingData.level;

                                // If invalid subservice, report.
                                if (buildingData.subService != "low")
                                {
                                    Logging.Message("Commercial building ", buildingData.Name, " has invalid subService ", buildingData.subService, "; reverting to low commercial.");
                                }
                                break;
                        }

                        // Initialize the prefab.
                        InitializePrefab(prefab, commercialAI, aiClass, buildingData.growable);

                        break;
                }
            }
        }

        /// <summary>
        /// Applies settings to a BuildingInfo prefab.
        /// </summary>
        /// <param name="prefab">The prefab to modify.</param>
        /// <param name="ai">The building AI to apply.</param>
        /// <param name="aiClass">The AI class string to apply.</param>
        /// <param name="growable">Whether the prefab should be growable.</param>
        private void InitializePrefab(BuildingInfo prefab, BuildingAI ai, string aiClass, bool growable) =>
            InitializePrefab(prefab, ai, ItemClassCollection.FindClass(aiClass), growable);

        /// <summary>
        /// Applies settings to a BuildingInfo prefab.
        /// </summary>
        /// <param name="prefab">The prefab to modify.</param>
        /// <param name="ai">The building AI to apply.</param>
        /// <param name="itemClass">The ItemClass to apply.</param>
        /// <param name="growable">Whether the prefab should be growable.</param>
        private void InitializePrefab(BuildingInfo prefab, BuildingAI ai, ItemClass itemClass, bool growable)
        {
            // Non-zero construction time important for other mods (Real Time, Real Construction) - only for private building AIs.
            if (ai is PrivateBuildingAI privateBuildingAI)
            {
                privateBuildingAI.m_constructionTime = 30;
            }

            // Assign required fields.
            prefab.m_buildingAI = ai;
            prefab.m_buildingAI.m_info = prefab;
            prefab.m_class = itemClass;
            prefab.m_placementStyle = growable ? ItemClass.Placement.Automatic : ItemClass.Placement.Manual;
            prefab.m_autoRemove = growable || !ModSettings.warnBulldoze;
        }

        /// <summary>
        /// Returns and industrial service name given a category.
        /// Service name is 'Forestry' if category is 'forest', otherwise the service name is just the capitalised first letter of the category.
        /// </summary>
        /// <param name="category">Category.</param>
        /// <returns>Service name.</returns>
        private string ServiceName(string category)
        {
            //  "forest" = "Forestry" 
            if (category == "forest")
            {
                return "Forestry";
            }
            else
            {
                // Everything else is just capitalised first letter.
                return category.Substring(0, 1).ToUpper() + category.Substring(1);
            }
        }

        /// <summary>
        /// Checks to see if the given subservice is a valid industrial subservice.
        /// </summary>
        /// <param name="subservice">Subservice to check.</param>
        /// <returns>True if the subservice is a valid industry subservice, false otherwise.</returns>
        private bool IsValidIndSubServ(string subservice)
        {
            // Check against each valid subservice.
            return (subservice == "farming" || subservice == "forest" || subservice == "oil" || subservice == "ore");
        }
    }
}