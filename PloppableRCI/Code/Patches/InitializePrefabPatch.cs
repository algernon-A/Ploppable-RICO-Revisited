// <copyright file="InitializePrefabPatch.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System.IO;
    using AlgernonCommons;
    using ColossalFramework.Packaging;
    using HarmonyLib;

    /// <summary>
    /// Patch for BuildingInfo.InitializePrefab, to read and apply RICO settings prior to prefab initialization.
    /// </summary>
    [HarmonyPatch(typeof(BuildingInfo), "InitializePrefab")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony")]
    public static class InitializePrefabPatch
    {
        /// <summary>
        /// Harmony prefix patch for BuildingInfo.InitializePrefab.
        /// Reads and applies RICO settings prior to prefab initialization.
        /// Harmony priority is High, and before boformer's Prefab Hook, so that RICO assets are converted to their 'final' RICO state before initialization.
        /// </summary>
        /// <param name="__instance">Original method instance reference.</param>
        /// <returns>Whether to continue InitializePrefab (always true).</returns>
        [HarmonyPriority(Priority.High)]
        [HarmonyBefore(new string[] { "github.com/boformer/PrefabHook" })]
        public static bool Prefix(BuildingInfo __instance)
        {
            // Basic sanity check before proceeding; if failed, don't do anything here - just continue on to game method.
            if (__instance.name == null)
            {
                return true;
            }

            // Create a new building record for this prefab and add it to our lists.
            BuildingData buildingData = new BuildingData(__instance);
            PrefabManager.PrefabDictionary[__instance] = buildingData;

            // Add to broken prefabs list (will be removed later if it's not broken).
            Loading.s_brokenPrefabs.Add(__instance);

            // Search for PloppableRICODefinition.xml files with this asset.
            // Need to use FindAssetByName(string, AssetType) because FindAssetByName(string) doesn't catch all assets at this stage of initialisation
            // (those two methods are more different than you might think - discovered that the hard way).
            Package.Asset asset = PackageManager.FindAssetByName(__instance.name, Package.AssetType.Object);

            // Get custom asset filesystem location (if CRP pacakge).
            string crpPath = asset?.package?.packagePath;

            if (!string.IsNullOrEmpty(crpPath))
            {
                // Look for RICO settings file.
                string ricoDefPath = Path.Combine(Path.GetDirectoryName(crpPath), "PloppableRICODefinition.xml");

                if (File.Exists(ricoDefPath))
                {
                    // Parse the file.
                    PloppableRICODefinition tempRicoDef = RICOReader.ParseRICODefinition(ricoDefPath);

                    if (tempRicoDef != null)
                    {
                        foreach (RICOBuilding buildingDef in tempRicoDef.Buildings)
                        {
                            // Go through each building parsed and check to see if we've got a match for this prefab.
                            if (MatchRICOName(buildingDef.Name, __instance.name, asset.package.packageName))
                            {
                                // Match!  Add these author settings to our prefab dictionary.
                                Logging.Message("found author settings for ", buildingDef.Name);
                                PrefabManager.PrefabDictionary[__instance].Author = buildingDef;
                                PrefabManager.PrefabDictionary[__instance].HasAuthor = true;
                            }
                        }
                    }
                }
            }

            // Check for and add any local settings for this prefab to our list.
            if (Loading.s_localRicoDef != null)
            {
                // Step through our previously loaded local settings and see if we've got a match.
                foreach (RICOBuilding buildingDef in Loading.s_localRicoDef.Buildings)
                {
                    if (buildingDef.Name.Equals(__instance.name))
                    {
                        // Match!  Add these author settings to our prefab dictionary.
                        PrefabManager.PrefabDictionary[__instance].Local = buildingDef;
                        PrefabManager.PrefabDictionary[__instance].HasLocal = true;
                    }
                }
            }

            // Check for any Workshop RICO mod settings for this prefab.
            if (Loading.s_mod1RicoDef != null)
            {
                // Step through our previously loaded local settings and see if we've got a match.
                foreach (RICOBuilding buildingDef in Loading.s_mod1RicoDef.Buildings)
                {
                    if (buildingDef.Name.Equals(__instance.name))
                    {
                        // Match!  Add these author settings to our prefab dictionary.
                        PrefabManager.PrefabDictionary[__instance].Mod = buildingDef;
                        PrefabManager.PrefabDictionary[__instance].HasMod = true;
                    }
                }
            }

            // Check for Modern Japan CCP mod settings for this prefab.
            if (Loading.s_mod2RicoDef != null)
            {
                // Step through our previously loaded local settings and see if we've got a match.
                foreach (RICOBuilding buildingDef in Loading.s_mod2RicoDef.Buildings)
                {
                    if (buildingDef.Name.Equals(__instance.name))
                    {
                        // Match!  Add these author settings to our prefab dictionary.
                        PrefabManager.PrefabDictionary[__instance].Mod = buildingDef;
                        PrefabManager.PrefabDictionary[__instance].HasMod = true;
                    }
                }
            }

            // Apply appropriate RICO settings to prefab.
            if (Loading.s_convertPrefabs != null)
            {
                // Start with local settings.
                if (PrefabManager.PrefabDictionary[__instance].HasLocal)
                {
                    // If local settings disable RICO, dont convert.
                    if (PrefabManager.PrefabDictionary[__instance].Local.m_ricoEnabled)
                    {
                        Loading.s_convertPrefabs.ConvertPrefab(PrefabManager.PrefabDictionary[__instance].Local, __instance);
                    }
                }
                else if (PrefabManager.PrefabDictionary[__instance].HasAuthor)
                {
                    // If no local settings, apply author settings.
                    // If author settings disable RICO, dont convert.
                    if (PrefabManager.PrefabDictionary[__instance].Author.m_ricoEnabled)
                    {
                        Loading.s_convertPrefabs.ConvertPrefab(PrefabManager.PrefabDictionary[__instance].Author, __instance);
                    }
                }
                else if (PrefabManager.PrefabDictionary[__instance].HasMod)
                {
                    // If none of the above, apply mod settings.
                    // If mod settings disable RICO, dont convert.
                    if (PrefabManager.PrefabDictionary[__instance].Mod.m_ricoEnabled)
                    {
                        Loading.s_convertPrefabs.ConvertPrefab(PrefabManager.PrefabDictionary[__instance].Mod, __instance);
                    }
                }
                else
                {
                    // No RICO settings; replicate game InitializePrefab checks overridden by transpiler.
                    int privateServiceIndex = ItemClass.GetPrivateServiceIndex(__instance.m_class.m_service);
                    if (privateServiceIndex != -1)
                    {
                        if (__instance.m_placementStyle == ItemClass.Placement.Manual)
                        {
                            throw new PrefabException(__instance, "Private building cannot have manual placement style");
                        }

                        if (__instance.m_paths != null && __instance.m_paths.Length != 0)
                        {
                            throw new PrefabException(__instance, "Private building cannot include roads or other net types");
                        }
                    }
                }
            }
            else
            {
                // This means that there's been a significant failure.  Ploppable RICO settings can't be applied.
                Logging.Error("convertPrefabs not initialised");
            }

            // Continue on to execute game InitializePrefab.
            return true;
        }

        /// <summary>
        /// Harmony prefix patch for BuildingInfo.InitializePrefab.
        /// Confirms that the prefab made it through initialisation.
        /// </summary>
        /// <param name="__instance">Building prefab instance.</param>
        public static void Postfix(BuildingInfo __instance)
        {
            // If we've made it here, the asset has initialised correctly (no PrefabExceptions thrown); remove it from broken prefabs list.
            Loading.s_brokenPrefabs.Remove(__instance);
        }

        /// <summary>
        /// Attempts to match a given RICO name with a prefab name.
        /// </summary>
        /// <param name="ricoName">Name parsed from Ploppable RICO definition file.</param>
        /// <param name="prefabName">BuildingInfo prefab name to match against.</param>
        /// <param name="packageName">Prefab package name.</param>
        /// <returns>True if a match was found, false otherwise.</returns>
        private static bool MatchRICOName(string ricoName, string prefabName, string packageName)
        {
            // Get common string combinations.
            string ricoNameData = ricoName + "_Data";
            string packagePrefix = packageName + ".";

            // Ordered in order of assumed probability.
            // Standard full workshop asset name - all local settings for workshop assets should match against this, as well as many author settings files.
            if (prefabName.Equals(packagePrefix + ricoNameData))
            {
                return true;
            }

            // Asset creators that append "_Data" after their name.
            if (prefabName.Equals(packagePrefix + ricoName))
            {
                return true;
            }

            if (prefabName.Equals(ricoNameData))
            {
                // The workshop package ID isn't included in the RICO settings file - common amongst workshop assets.
                return true;
            }
            else if (prefabName.Equals(ricoName))
            {
                // Direct match - mostly applies to game assets, but some workshop assets may also match here.
                return true;
            }

            // No match.
            return false;
        }
    }
}
