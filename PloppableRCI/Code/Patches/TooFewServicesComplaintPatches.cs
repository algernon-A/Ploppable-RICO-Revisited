// <copyright file="TooFewServicesComplaintPatches.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System.Collections.Generic;
    using System.Reflection;
    using HarmonyLib;

    /// <summary>
    /// Harmony patches to override OnCalculateOfficeLevelUp and OnCalculateIndustrialLevelUp to force disabling of 'too few services' complaints for all buildings.
    /// </summary>
    [HarmonyPatch]
    public static class TooFewServicesComplaintPatches
    {
        /// <summary>
        /// Determines list of target methods to patch - in this case, LevelUpWrapper methods OnCalculateOfficeLevelUp and OnCalculateIndustrialLevelUp.
        /// </summary>
        /// <returns>List of target methods to patch.</returns>
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(LevelUpWrapper), nameof(LevelUpWrapper.OnCalculateOfficeLevelUp));
            yield return AccessTools.Method(typeof(LevelUpWrapper), nameof(LevelUpWrapper.OnCalculateIndustrialLevelUp));
        }

        /// <summary>
        /// Harmony Postfix patch to force 'too few services' complaints off for all buildings.
        /// </summary>
        /// <param name="tooFewServices">If a 'too few services' complaint has been calculated.</param>
        /// <param name="buildingID">Building instance ID.</param>
        public static void Postfix(ref bool tooFewServices, ushort buildingID)
        {
            // Check if this building is RICO or not.
            bool isRICO = RICOUtils.IsRICOBuilding(buildingID);

            // Check if the relevant 'ignore too few services complaint' setting is set.
            if ((ModSettings.noServicesOther && !isRICO) || (ModSettings.noServicesRicoGrow && isRICO) || (ModSettings.noServicesRicoPlop && RICOUtils.IsRICOPloppable(buildingID)))
            {
                // It is - force too few services complaint off.
                tooFewServices = false;
            }
        }
    }
}
