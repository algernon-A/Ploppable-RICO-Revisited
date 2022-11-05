// <copyright file="LandValueComplaintPatches.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System.Collections.Generic;
    using System.Reflection;
    using HarmonyLib;

    /// <summary>
    /// Harmony patches to override OnCalculateResidentialLevelUp and OnCommercialResidentialUp to force disabling of 'land value too low' complaints for all buildings.
    /// </summary>
    [HarmonyPatch]
    public static class LandValueComplaintPatches
    {
        /// <summary>
        /// Determines list of target methods to patch - in this case, LevelUpWrapper methods OnCalculateResidentialLevelUp and OnCommercialResidentialUp.
        /// </summary>
        /// <returns>List of target methods to patch.</returns>
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(LevelUpWrapper), nameof(LevelUpWrapper.OnCalculateResidentialLevelUp));
            yield return AccessTools.Method(typeof(LevelUpWrapper), nameof(LevelUpWrapper.OnCalculateCommercialLevelUp));
        }

        /// <summary>
        /// Harmony Postfix patch to force 'land value too low' complaints off for all buildings.
        /// </summary>
        /// <param name="landValueTooLow">If a 'land value too low' complaint has been calculated.</param>
        /// <param name="buildingID">Building instance ID.</param>
        public static void Postfix(ref bool landValueTooLow, ushort buildingID)
        {
            // Check if this building is RICO or not.
            bool isRICO = RICOUtils.IsRICOBuilding(buildingID);

            // Check if the relevant 'ignore low land value complaint' setting is set.
            if ((ModSettings.noValueOther && !isRICO) || (ModSettings.noValueRicoGrow && isRICO) || (ModSettings.noValueRicoPlop && RICOUtils.IsRICOPloppable(buildingID)))
            {
                // It is - force land value complaint off.
                landValueTooLow = false;
            }
        }
    }
}
