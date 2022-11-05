// <copyright file="BuildingPatches.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System;
    using HarmonyLib;

    /// <summary>
    /// Harmony patch to implement 'no zoning checks' functionality.
    /// </summary>
    [HarmonyPatch(typeof(Building), nameof(Building.CheckZoning), new Type[] { typeof(ItemClass.Zone), typeof(ItemClass.Zone), typeof(bool) })]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony")]
    public static class BuildingPatches
    {
        /// <summary>
        /// Harmony prefix patch to Building.CheckZoning to implement 'no zoning checks' functionality.
        /// </summary>
        /// <param name="__result">Original method result.</param>
        /// <param name="__instance">Building data instance.</param>
        /// <returns>False (don't execute original method) if this is a building covered by 'no zoning checks', otherwise true (execute original method).</returns>
        public static bool Prefix(ref bool __result, ref Building __instance)
        {
            // Check if this building is RICO or not.
            bool isRICO = RICOUtils.IsRICOAI(__instance.Info.GetAI() as PrivateBuildingAI);

            // Check if the relevant 'ignore zoning' setting is set.
            if ((ModSettings.noZonesOther && !isRICO) || (ModSettings.noZonesRico && isRICO))
            {
                // It is - set the result to true (tell the game we're in a valid zone).
                __result = true;

                // Don't execute original method.
                return false;
            }

            // If we got here, this isn't a building covered by our settings: continue to original method .
            return true;
        }
    }
}