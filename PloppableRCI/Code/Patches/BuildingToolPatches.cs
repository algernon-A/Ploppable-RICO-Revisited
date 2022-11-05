﻿// <copyright file="BuildingToolPatches.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace PloppableRICO
{
    using System;
    using AlgernonCommons;
    using ColossalFramework;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Harmony patches for the game's builidng tool to implement demolition confirmation and construction time settings.
    /// Based on boformers Larger Footprints mod.  Many thanks to him for his work.
    /// </summary>
    [HarmonyPatch(typeof(BuildingTool))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony")]
    internal static class BuildingToolPatches
    {
        // Delegate to CommonBuildingAI.BuildingCompleted (protected method).
        private static BuildingCompletedDelegate s_buildingCompleted;

        /// <summary>
        /// Delegate to CommonBuildingAI.BuildingCompleted (open delegate).
        /// </summary>
        private delegate void BuildingCompletedDelegate(CommonBuildingAI instance, ushort buildingID, ref Building buildingData);

        /// <summary>
        /// Called by Harmony when patching to peform pre-patch actions.
        /// </summary>
        public static void Prepare()
        {
            // Set the BuildingCompleted delegate if we haven't already.
            if (s_buildingCompleted == null)
            {
                s_buildingCompleted = AccessTools.MethodDelegate<BuildingCompletedDelegate>(AccessTools.Method(typeof(CommonBuildingAI), "BuildingCompleted"));
                if (s_buildingCompleted == null)
                {
                    Logging.Error("unable to get delegate for CommonBuildingAI.BuildingCompleted");
                }
            }
        }

        /// <summary>
        /// Harmony Prefix patch to ensure that ploppable RICO buildings are classified as "important buildings".  This saves them from auto-demolition.
        /// Based on boformers Larger Footprints mod.  Many thanks to him for his work.
        /// </summary>
        /// <param name="__result">Original method result.</param>
        /// <param name="info">Building prefab.</param>
        /// <returns>False (don't continue execution chain) if this is a RICO building (original return value changed to true), true (continue exection chain) otherwise.</returns>
        [HarmonyPatch(
            "IsImportantBuilding",
            new Type[] { typeof(BuildingInfo), typeof(Building) },
            new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref })]
        [HarmonyPrefix]
        public static bool IsImportantBuildingPrefix(ref bool __result, BuildingInfo info)
        {
            // Only do this if our settings are set to ensure RICO buildings are important.
            if (!ModSettings.autoDemolish)
            {
                // All we want to do here is ensure that ploppable RICO buildings are classified as "Important Buildings" (to "spare them from the wrath of the BuildingTool"...)
                if (info.m_buildingAI is PloppableOfficeAI || info.m_buildingAI is PloppableExtractorAI || info.m_buildingAI is PloppableResidentialAI || info.m_buildingAI is PloppableCommercialAI || info.m_buildingAI is PloppableIndustrialAI)
                {
                    // Found a ploppable RICO building - set original method return value.
                    __result = true;

                    // Don't execute base method after this.
                    return false;
                }
            }

            // Didn't find a ploppable RICO building (or the RICO important setting isn't set) - go onto running the original game method.
            return true;
        }

        /// <summary>
        /// Harmony Postfix patch to skip 'gradual construction' for plopped RICO growables, and/or to apply the 'Make Historical' and/or 'Lock level' settings on building creation, accoriding to settings.
        /// </summary>
        /// <param name="__result">Original method result (unchanged).</param>
        /// <param name="info">BuildingInfo prefab for this building (unchanged).</param>
        [HarmonyPatch(
            "CreateBuilding",
            new Type[] { typeof(BuildingInfo), typeof(Vector3), typeof(float), typeof(int), typeof(bool), typeof(bool) },
            new ArgumentType[] { ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal })]

        [HarmonyPostfix]
        internal static void CreateBuildingPostfix(ref ushort __result, ref BuildingInfo info)
        {
            // Check that we have a valid building ID.
            if (__result == 0)
            {
                return;
            }

            // Get building AI.
            PrivateBuildingAI buildingAI = info.GetAI() as PrivateBuildingAI;

            // Only interested in private building AI.
            if (buildingAI != null)
            {
                // Check if AI is a RICO custom AI type.
                bool isRICO = RICOUtils.IsRICOAI(buildingAI);

                // Check if it's a RICO custom AI type.
                // Enable 'ploppable growables' if option is set.
                if ((ModSettings.plopOther && !isRICO) || (ModSettings.plopRico && isRICO))
                {
                    // Check to see if construction time is greater than zero.
                    if (buildingAI.m_constructionTime > 0)
                    {
                        // Complete construction.
                        Singleton<BuildingManager>.instance.m_buildings.m_buffer[__result].m_frame0.m_constructState = byte.MaxValue;
                        s_buildingCompleted.Invoke(buildingAI, __result, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[__result]);

                        // Have to do this manually as CommonBuildingAI.BuildingCompleted won't if construction time isn't zero.
                        Singleton<BuildingManager>.instance.UpdateBuildingRenderer(__result, updateGroup: true);
                    }
                }

                // Enable 'Make Historical' if option is set.
                if ((ModSettings.historicalOther && !isRICO) || (ModSettings.historicalRico && isRICO))
                {
                    info.m_buildingAI.SetHistorical(__result, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[__result], historical: true);
                }

                // Enable ABLC level lock if option is set and ABLC is running.
                if (ModUtils.ablcLockBuildingLevel != null && ((ModSettings.lockLevelOther && !isRICO) || (ModSettings.lockLevelRico && isRICO)))
                {
                    ModUtils.ablcLockBuildingLevel.Invoke(null, new object[] { __result, Singleton<BuildingManager>.instance.m_buildings.m_buffer[__result].m_level });
                }
            }
        }
    }
}