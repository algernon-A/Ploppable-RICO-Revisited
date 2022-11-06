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
        // Ignore complaint settings.
        private static bool s_noValueRicoPlop = true;
        private static bool s_noValueRicoGrow = true;
        private static bool s_noValueOther = false;

        /// <summary>
        /// Gets or sets a value indicating whether land value complaints are disabled for Ploppable RICO ploppables (true means disabled).
        /// </summary>
        internal static bool NoValueRicoPlop { get => s_noValueRicoPlop; set => s_noValueRicoPlop = value; }

        /// <summary>
        /// Gets or sets a value indicating whether land value complaints are disabled for Ploppable RICO growables (true means disabled).
        /// </summary>
        internal static bool NoValueRicoGrow { get => s_noValueRicoGrow; set => s_noValueRicoGrow = value; }

        /// <summary>
        /// Gets or sets a value indicating whether land value complaints are disabled for generic growables (true means disabled).
        /// </summary>
        internal static bool NoValueOther { get => s_noValueOther; set => s_noValueOther = value; }

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
            if ((s_noValueOther & !isRICO) || (s_noValueRicoGrow & isRICO) || (s_noValueRicoPlop && RICOUtils.IsRICOPloppable(buildingID)))
            {
                // It is - force land value complaint off.
                landValueTooLow = false;
            }
        }
    }
}
