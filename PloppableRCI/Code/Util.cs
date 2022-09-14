// <copyright file="Util.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System;
    using System.Collections.Generic;
    using ColossalFramework.Plugins;

    /// <summary>
    /// Various RICO-related utilities.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Gets a value indicating whether the After Dark expansion is installed.
        /// </summary>
        /// <returns>True if After Dark is installed, false otherwise.</returns>
        public static bool IsADinstalled() => SteamHelper.IsDLCOwned(SteamHelper.DLC.AfterDarkDLC);

        /// <summary>
        /// Gets a value indicating whether the Green Cities expansion is installed.
        /// </summary>
        /// <returns>True if Green Cities is installed, false otherwise.</returns>
        public static bool IsGCinstalled() => SteamHelper.IsDLCOwned(SteamHelper.DLC.GreenCitiesDLC);


        /// <summary>
        /// Gets a value indicating whether the Plazas and Promenades expansion is installed.
        /// </summary>
        /// <returns>True if Plazas and Promenades is installed, false otherwise.</returns>
        public static bool IsPPinstalled() => SteamHelper.IsDLCOwned(SteamHelper.DLC.PlazasAndPromenadesDLC);

        /// <summary>
        /// Gets the default workplace distribution for the given service, subservice, and level.
        /// </summary>
        /// <param name="service">Service.</param>
        /// <param name="subservice">Sub-service.</param>
        /// <param name="level">Level.</param>
        /// <returns>Workplace distribution array.</returns>
        public static int[] WorkplaceDistributionOf(string service, string subservice, string level)
        {
            // Workplace distributions by building category, subservice, and level.
            Dictionary<string, int[]> distributions = new Dictionary<string, int[]>()
            {
                { "IndustrialIndustrialFarming", new int[] { 100, 100, 0, 0, 0 } },
                { "IndustrialIndustrialForestry", new int[] { 100, 100, 0, 0, 0 } },
                { "IndustrialIndustrialOre", new int[] { 100, 20, 60, 20, 0 } },
                { "IndustrialIndustrialOil", new int[] { 100, 20, 60, 20, 0 } },
                { "IndustrialIndustrialGenericLevel1", new int[] { 100, 100, 0, 0, 0 } },
                { "IndustrialIndustrialGenericLevel2", new int[] { 100, 20, 50, 20, 0 } },
                { "IndustrialIndustrialGenericLevel3", new int[] { 100, 15, 55, 25, 5 } },
                { "OfficeNoneLevel1", new int[] { 100, 0, 40, 50, 10 } },
                { "OfficeNoneLevel2", new int[] { 100, 0, 20, 50, 30 } },
                { "OfficeNoneLevel3", new int[] { 100, 0, 0, 40, 60 } },
                { "OfficeOfficeWallToWallLevel1", new int[] { 100, 0, 40, 50, 10 } },
                { "OfficeOfficeWallToWallLevel2", new int[] { 100, 0, 20, 50, 30 } },
                { "OfficeOfficeWallToWallLevel3", new int[] { 100, 0, 0, 40, 60 } },
                { "ExtractorIndustrialFarming", new int[] { 100, 100, 0, 0, 0 } },
                { "ExtractorIndustrialForestry", new int[] { 100, 100, 0, 0, 0 } },
                { "ExtractorIndustrialOre", new int[] { 100, 20, 60, 20, 0 } },
                { "ExtractorIndustrialOil", new int[] { 100, 20, 60, 20, 0 } },
                { "CommercialCommercialTourist", new int[] { 100, 20, 20, 30, 30 } },
                { "CommercialCommercialLeisure", new int[] { 100, 30, 30, 20, 20 } },
                { "CommercialCommercialLowLevel1", new int[] { 100, 100, 0, 0, 0 } },
                { "CommercialCommercialLowLevel2", new int[] { 100, 20, 60, 20, 0 } },
                { "CommercialCommercialLowLevel3", new int[] { 100, 5, 15, 30, 50 } },
                { "CommercialCommercialHighLevel1", new int[] { 100, 0, 40, 50, 10 } },
                { "CommercialCommercialHighLevel2", new int[] { 100, 0, 20, 50, 30 } },
                { "CommercialCommercialHighLevel3", new int[] { 100, 0, 0, 40, 60 } },
                { "CommercialCommercialWallToWallLevel1", new int[] { 100, 0, 40, 50, 10 } },
                { "CommercialCommercialWallToWallLevel2", new int[] { 100, 0, 20, 50, 30 } },
                { "CommercialCommercialWallToWallLevel3", new int[] { 100, 0, 0, 40, 60 } },
                { "CommercialCommercialEco", new int[] { 100, 50, 50, 0, 0 } },
                { "OfficeOfficeHighTech", new int[] { 100, 0, 10, 40, 50 } },
            };


            distributions.Add("industrialfarming", distributions["IndustrialIndustrialFarming"]);
            distributions.Add("industrialforestry", distributions["IndustrialIndustrialForestry"]);
            distributions.Add("industrialore", distributions["IndustrialIndustrialOre"]);
            distributions.Add("industrialoil", distributions["IndustrialIndustrialOil"]);
            distributions.Add("industrialgenericLevel1", distributions["IndustrialIndustrialGenericLevel1"]);
            distributions.Add("industrialgenericLevel2", distributions["IndustrialIndustrialGenericLevel2"]);
            distributions.Add("industrialgenericLevel3", distributions["IndustrialIndustrialGenericLevel3"]);
            distributions.Add("officenoneLevel1", distributions["OfficeNoneLevel1"]);
            distributions.Add("officenoneLevel2", distributions["OfficeNoneLevel2"]);
            distributions.Add("officenoneLevel3", distributions["OfficeNoneLevel3"]);
            distributions.Add("officewall2wallLevel1", distributions["OfficeOfficeWallToWallLevel1"]);
            distributions.Add("officewall2wallLevel2", distributions["OfficeOfficeWallToWallLevel2"]);
            distributions.Add("officewall2wallLevel3", distributions["OfficeOfficeWallToWallLevel3"]);
            distributions.Add("extractorfarming", distributions["ExtractorIndustrialFarming"]);
            distributions.Add("extractorforestry", distributions["ExtractorIndustrialForestry"]);
            distributions.Add("extractorore", distributions["ExtractorIndustrialOre"]);
            distributions.Add("extractoroil", distributions["ExtractorIndustrialOil"]);
            distributions.Add("commercialtourist", distributions["CommercialCommercialTourist"]);
            distributions.Add("commercialleisure", distributions["CommercialCommercialLeisure"]);
            distributions.Add("commerciallowLevel1", distributions["CommercialCommercialLowLevel1"]);
            distributions.Add("commerciallowLevel2", distributions["CommercialCommercialLowLevel2"]);
            distributions.Add("commerciallowLevel3", distributions["CommercialCommercialLowLevel3"]);
            distributions.Add("commercialhighLevel1", distributions["CommercialCommercialHighLevel1"]);
            distributions.Add("commercialhighLevel2", distributions["CommercialCommercialHighLevel2"]);
            distributions.Add("commercialhighLevel3", distributions["CommercialCommercialHighLevel3"]);
            distributions.Add("commercialwall2wallLevel1", distributions["CommercialCommercialWallToWallLevel1"]);
            distributions.Add("commercialwall2wallLevel2", distributions["CommercialCommercialWallToWallLevel2"]);
            distributions.Add("commercialwall2wallLevel3", distributions["CommercialCommercialWallToWallLevel3"]);
            distributions.Add("commercialeco", distributions["CommercialCommercialEco"]);
            distributions.Add("officehigh tech", distributions["OfficeOfficeHighTech"]);

            int[] workplaceDistribution = null;

            if (distributions.ContainsKey(service + subservice))
            {
                // First try basic (level-less) sevice + subservice match.
                workplaceDistribution = distributions[service + subservice];
            }
            else if (distributions.ContainsKey(service + subservice + level))
            {
                // If not, try adding level.
                workplaceDistribution = distributions[service + subservice + level];
            }
            else if (distributions.ContainsKey(service + "none" + level))
            {
                // If not, try using "none" for subservice.
                workplaceDistribution = distributions[service + "none" + level];
            }
            else if (distributions.ContainsKey(service + "none" + level))
            {
                // If not, try using "generic" for subservice.
                workplaceDistribution = distributions[service + "generic" + level];
            }

            if (workplaceDistribution != null)
            {
                // We've got a distribution; return it.
                return workplaceDistribution;
            }
            else
            {
                // Fallback - no distribtion found - evenly assign jobs across all education levels.
                return new int[] { 100, 25, 25, 25, 25 };
            }
        }

        /// <summary>
        /// Returns the maximum permitted level for the given sub-service.
        /// </summary>
        /// <param name="subService">Subservice.</param>
        /// <returns>Maximum permitted level (1-based).</returns>
        public static int MaxLevelOf(ItemClass.SubService subService)
        {
            switch (subService)
            {
                case ItemClass.SubService.ResidentialLow:
                case ItemClass.SubService.ResidentialHigh:
                case ItemClass.SubService.ResidentialLowEco:
                case ItemClass.SubService.ResidentialHighEco:
                case ItemClass.SubService.ResidentialWallToWall:
                    return 5;
                case ItemClass.SubService.CommercialLow:
                case ItemClass.SubService.CommercialHigh:
                case ItemClass.SubService.CommercialWallToWall:
                case ItemClass.SubService.OfficeGeneric:
                case ItemClass.SubService.OfficeWallToWall:
                case ItemClass.SubService.IndustrialGeneric:
                    return 3;
                default:
                    return 1;
            }
        }

        /// <summary>
        /// Gets the proper UI category name based on category and subservice combination.
        /// </summary>
        /// <param name="service">Service.</param>
        /// <param name="subservice">Sub-service.</param>
        /// <returns>UI category name.</returns>
        public static string UICategoryOf(string service, string subservice)
        {
            string category = "";

            // Ensure vald service and subservice.
            if (!string.IsNullOrEmpty(service) && !string.IsNullOrEmpty(subservice))
            {
                switch (service)
                {
                    case "residential":
                        category = subservice.Equals("high") || subservice.Equals("wall2wall") ? "reshigh" : "reslow";
                        break;
                    case "commercial":
                        category = subservice.Equals("high") || subservice.Equals("wall2wall") ? "comhigh" : "comlow";
                        break;
                    case "office":
                        category = "office";
                        break;
                    case "industrial":
                        category = subservice.Equals("generic") ? "industrial" : subservice;
                        break;
                    case "extractor":
                        category = subservice;
                        break;
                    case "none":
                        category = "none";
                        break;
                }
            }

            return category;
        }

        /// <summary>
        /// Gets the settings mod filepath.
        /// </summary>
        /// <param name="name">Settings mod name.</param>
        /// <returns>Mod filepath.</returns>
        public static string SettingsModPath(string name)
        {
            IEnumerable<PluginManager.PluginInfo> modList = PluginManager.instance.GetPluginsInfo();
            string modPath = "null";

            foreach (PluginManager.PluginInfo modInfo in modList)
            {
                if (modInfo.name == name)
                {
                    modPath = modInfo.modPath;
                }
            }
            return modPath;
        }
    }
}