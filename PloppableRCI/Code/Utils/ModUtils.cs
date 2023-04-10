// <copyright file="ModUtils.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using AlgernonCommons;
    using ColossalFramework.Packaging;
    using ColossalFramework.Plugins;
    using HarmonyLib;

    /// <summary>
    /// Utilities dealing with other mods, including compatibility and functionality checks.
    /// </summary>
    public static class ModUtils
    {
        /// <summary>
        ///  Gets a value indicating whether or not a realistic population mod is installed and enabled.
        /// </summary>
        internal static bool RealPopEnabled { get; private set; } = false;

        /// <summary>
        /// Gets the list of conflicting mod names.
        /// </summary>
        internal static List<string> ConflictingModNames { get; private set; }

        /// <summary>
        /// Checks for known 'soft' mod conflicts and function extenders.
        /// </summary>
        /// <returns>Whether or not a soft mod conflict was detected.</returns>
        internal static bool CheckMods()
        {
            // Initialise flag and list of conflicting mods.
            bool conflictDetected = false;
            ConflictingModNames = new List<string>();

            // No hard conflicts - check for 'soft' conflicts.
            if (IsPtGInstalled())
            {
                conflictDetected = true;
                Logging.Message("Plop the Growables detected");

                // Add PTG to mod conflict list.
                ConflictingModNames.Add("PTG");
            }

            // Check for realistic population mods.
            RealPopEnabled = AssemblyUtils.GetEnabledAssembly("RealPopRevisited") != null || AssemblyUtils.GetEnabledAssembly("WG_BalancedPopMod") != null;

            // Check for Workshop RICO settings mod.
            if (IsModEnabled(629850626uL))
            {
                Logging.Message("found Workshop RICO settings mod");
                InitializePrefabPatch.Mod1RicoDef = RICOReader.ParseRICODefinition(Path.Combine(RICOUtils.SettingsModPath("629850626"), "WorkshopRICOSettings.xml"), false);
            }

            // Check for Ryuichi Kaminogi's "RICO Settings for Modern Japan CCP"
            Package modernJapanRICO = PackageManager.GetPackage("2035770233");
            if (modernJapanRICO != null)
            {
                Logging.Message("found RICO Settings for Modern Japan CCP");
                InitializePrefabPatch.Mod2RicoDef = RICOReader.ParseRICODefinition(Path.Combine(Path.GetDirectoryName(modernJapanRICO.packagePath), "PloppableRICODefinition.xml"), false);
            }

            return conflictDetected;
        }

        /// <summary>
        /// Uses reflection to find the LockBuildingLevel method of Advanced Building Level Control.
        /// If successful, sets ablcLockBuildingLevel field.
        /// </summary>
        internal static void ABLCReflection()
        {
            Assembly ablcAssembly = AssemblyUtils.GetEnabledAssembly("AdvancedBuildingLevelControl");
            if (ablcAssembly != null)
            {
                Logging.KeyMessage("Found Advanced Building Level Control");

                // Found AdvancedBuildingLevelControl.dll that's part of an enabled plugin; try to get its ExternalCalls class.
                Type ablcExternalCalls = ablcAssembly.GetType("ABLC.ExternalCalls");

                if (ablcExternalCalls != null)
                {
                    // Try to get LockBuildingLevel method.
                    MethodInfo ablcLockBuildingLevel = AccessTools.Method(ablcExternalCalls, "LockBuildingLevel");
                    if (ablcLockBuildingLevel != null)
                    {
                        // Success!
                        Logging.KeyMessage("found LockBuildingLevel");
                        BuildingToolPatches.LockBuildingLevel = AccessTools.MethodDelegate<BuildingToolPatches.LockBuildingLevelDelegate>(ablcLockBuildingLevel);
                    }
                }
            }
            else
            {
                // If we got here, we were unsuccessful.
                Logging.KeyMessage("Advanced Building Level Control not found");
            }
        }

        /// <summary>
        /// Checks to see if another mod is installed and enabled, based on a provided Steam Workshop ID.
        /// </summary>
        /// <param name="id">Steam workshop ID.</param>
        /// <returns>True if the mod is installed and enabled, false otherwise.</returns>
        private static bool IsModEnabled(ulong id)
        {
            return PluginManager.instance.GetPluginsInfo().Any(mod => (mod.publishedFileID.AsUInt64 == id && mod.isEnabled));
        }

        /// <summary>
        ///  Checks for the Plop the Growables mod, as distinct from the PtG converter.
        /// </summary>
        /// <returns>True if the original Plop the Growables mod is installed and active, false otherwise.</returns>
        private static bool IsPtGInstalled()
        {
            // Iterate through the full list of plugins.
            foreach (PluginManager.PluginInfo plugin in PluginManager.instance.GetPluginsInfo())
            {
                foreach (Assembly assembly in plugin.GetAssemblies())
                {
                    // Looking for an assembly named "PlopTheGrowables" that's active.
                    if (assembly.GetName().Name.Equals("PlopTheGrowables") && plugin.isEnabled)
                    {
                        // Found one - is this the converter mod class?
                        if (!plugin.userModInstance.GetType().ToString().Equals("PlopTheGrowables.PtGReaderMod"))
                        {
                            // Not converter mod class - assume it's the original.
                            return true;
                        }
                        else
                        {
                            // Converter mod class - log and continue.
                            Logging.Message("found Plop the Growables converter");
                        }
                    }
                }
            }

            // If we got here, no active PtG was detected.
            return false;
        }
    }
}