﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using ICities;
using ColossalFramework.Plugins;
using ColossalFramework.Packaging;


namespace PloppableRICO
{
    /// <summary>
    /// Utilities dealing with other mods, including compatibility and functionality checks.
    /// </summary>
    public static class ModUtils
    {
        // ABLC methods.
        internal static MethodInfo ablcLockBuildingLevel;

        // List of conflcting mod names.
        internal static List<string> conflictingModNames;


        /// <summary>
        ///  Flag to determine whether or not a realistic population mod is installed and enabled.
        /// </summary>
        internal static bool realPopEnabled = false;


        /// <summary>
        /// Checks for any known fatal mod conflicts.
        /// </summary>
        /// <returns>True if a mod conflict was detected, false otherwise</returns>
        internal static bool IsModConflict()
        {
            // Initialise flag and list of conflicting mods.
            bool conflictDetected = false;
            conflictingModNames = new List<string>();

            // Iterate through the full list of plugins.
            foreach (PluginManager.PluginInfo plugin in PluginManager.instance.GetPluginsInfo())
            {
                foreach (Assembly assembly in plugin.GetAssemblies())
                {
                    switch (assembly.GetName().Name)
                    {
                        case "PloppableRICO":
                            // Original Ploppable RICO mod.
                            conflictDetected = true;
                            conflictingModNames.Add("Ploppable RICO (old version)");
                            break;
                        case "EnhancedBuildingCapacity":
                            // Enhanced building capacity.
                            conflictDetected = true;
                            conflictingModNames.Add("Enhanced Building Capacity");
                            break;
                        case "VanillaGarbageBinBlocker":
                            // Garbage Bin Controller
                            conflictDetected = true;
                            conflictingModNames.Add("Garbage Bin Controller");
                            break;
                        case "Painter":
                            // Painter - this one is trickier because both Painter and Repaint use Painter.dll (thanks to CO savegame serialization...)
                            if (plugin.userModInstance.GetType().ToString().Equals("Painter.UserMod"))
                            {
                                conflictDetected = true;
                                conflictingModNames.Add("Painter");
                            }
                            break;
                    }
                }
            }

            // Was a conflict detected?
            if (conflictDetected)
            {
                // Yes - log each conflict.
                foreach (string conflictingMod in conflictingModNames)
                {
                    Logging.Error("Conflicting mod found: ", conflictingMod);
                }
                Logging.Error("exiting due to mod conflict");
            }

            return conflictDetected;
        }



        /// <summary>
        /// Checks for known 'soft' mod conflicts and function extenders.
        /// </summary>
        /// <returns>Whether or not a soft mod conflict was detected</returns>
        internal static bool CheckMods()
        {
            // Initialise flag and list of conflicting mods.
            bool conflictDetected = false;
            conflictingModNames = new List<string>();

            // No hard conflicts - check for 'soft' conflicts.
            if (IsPtGInstalled())
            {
                conflictDetected = true;
                Logging.Message("Plop the Growables detected");

                // Add PTG to mod conflict list.
                conflictingModNames.Add("PTG");
            }

            // Check for realistic population mods.
            realPopEnabled = (IsModInstalled("RealPopRevisited", true) || IsModInstalled("WG_BalancedPopMod", true));

            // Check for Workshop RICO settings mod.
            if (IsModEnabled(629850626uL))
            {
                Logging.Message("found Workshop RICO settings mod");
                Loading.mod1RicoDef = RICOReader.ParseRICODefinition(Path.Combine(Util.SettingsModPath("629850626"), "WorkshopRICOSettings.xml"), false);
            }

            // Check for Ryuichi Kaminogi's "RICO Settings for Modern Japan CCP"
            Package modernJapanRICO = PackageManager.GetPackage("2035770233");
            if (modernJapanRICO != null)
            {
                Logging.Message("found RICO Settings for Modern Japan CCP");
                Loading.mod2RicoDef = RICOReader.ParseRICODefinition(Path.Combine(Path.GetDirectoryName(modernJapanRICO.packagePath), "PloppableRICODefinition.xml"), false);
            }

            return conflictDetected;
        }


        /// <summary>
        /// Returns the filepath of the mod assembly.
        /// </summary>
        /// <returns>Mod assembly filepath</returns>
        internal static string GetAssemblyPath()
        {
            // Get list of currently active plugins.
            IEnumerable<PluginManager.PluginInfo> plugins = PluginManager.instance.GetPluginsInfo();

            // Iterate through list.
            foreach (PluginManager.PluginInfo plugin in plugins)
            {
                try
                {
                    // Get all (if any) mod instances from this plugin.
                    IUserMod[] mods = plugin.GetInstances<IUserMod>();

                    // Check to see if the primary instance is this mod.
                    if (mods.FirstOrDefault() is PloppableRICOMod)
                    {
                        // Found it! Return path.
                        return plugin.modPath;
                    }
                }
                catch
                {
                    // Don't care.
                }
            }

            // If we got here, then we didn't find the assembly.
            Logging.Error("assembly path not found");
            throw new FileNotFoundException(PloppableRICOMod.ModName + ": assembly path not found!");
        }


        /// <summary>
        /// Uses reflection to find the LockBuildingLevel method of Advanced Building Level Control.
        /// If successful, sets ablcLockBuildingLevel field.
        /// </summary>
        internal static void ABLCReflection()
        {
            Logging.KeyMessage("Attempting to find Advanced Building Level Control");

            // Iterate through each loaded plugin assembly.
            foreach (PluginManager.PluginInfo plugin in PluginManager.instance.GetPluginsInfo())
            {
                foreach (Assembly assembly in plugin.GetAssemblies())
                {
                    if (assembly.GetName().Name.Equals("AdvancedBuildingLevelControl") && plugin.isEnabled)
                    {
                        Logging.KeyMessage("Found Advanced Building Level Control");

                        // Found AdvancedBuildingLevelControl.dll that's part of an enabled plugin; try to get its ExternalCalls class.
                        Type ablcExternalCalls = assembly.GetType("ABLC.ExternalCalls");

                        if (ablcExternalCalls != null)
                        {
                            // Try to get LockBuildingLevel method.
                            ablcLockBuildingLevel = ablcExternalCalls.GetMethod("LockBuildingLevel", BindingFlags.Public | BindingFlags.Static);
                            if (ablcLockBuildingLevel != null)
                            {
                                // Success!
                                Logging.KeyMessage("found LockBuildingLevel");
                            }
                        }

                        // At this point, we're done; return.
                        return;
                    }
                }
            }

            // If we got here, we were unsuccessful.
            Logging.KeyMessage("Advanced Building Level Control not found");
        }


        /// <summary>
        /// Checks to see if another mod is installed and enabled, based on a provided Steam Workshop ID.
        /// </summary>
        /// <param name="id">Steam workshop ID</param>
        /// <returns>True if the mod is installed and enabled, false otherwise</returns>
        private static bool IsModEnabled(UInt64 id)
        {
            return PluginManager.instance.GetPluginsInfo().Any(mod => (mod.publishedFileID.AsUInt64 == id && mod.isEnabled));
        }


        /// <summary>
        /// Checks to see if another mod is installed, based on a provided assembly name.
        /// </summary>
        /// <param name="assemblyName">Name of the mod assembly</param>
        /// <param name="enabledOnly">True if the mod needs to be enabled for the purposes of this check; false if it doesn't matter</param>
        /// <returns>True if the mod is installed (and, if enabledOnly is true, is also enabled), false otherwise</returns>
        private static bool IsModInstalled(string assemblyName, bool enabledOnly = false)
        {
            // Convert assembly name to lower case.
            string assemblyNameLower = assemblyName.ToLower();

            // Iterate through the full list of plugins.
            foreach (PluginManager.PluginInfo plugin in PluginManager.instance.GetPluginsInfo())
            {
                foreach (Assembly assembly in plugin.GetAssemblies())
                {
                    if (assembly.GetName().Name.ToLower().Equals(assemblyNameLower))
                    {
                        Logging.Message("found mod assembly ", assemblyName);
                        if (enabledOnly)
                        {
                            return plugin.isEnabled;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }

            // If we've made it here, then we haven't found a matching assembly.
            return false;
        }


        /// <summary>
        ///  Checks for the Plop the Growables mod, as distinct from the PtG converter.
        /// </summary>
        /// <returns>True if the original Plop the Growables mod is installed and active, false otherwise</returns>
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