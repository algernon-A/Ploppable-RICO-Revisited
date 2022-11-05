// <copyright file="CommonBuildingAIPatches.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using HarmonyLib;

    /// <summary>
    /// Harmony patch to stop RICO buildings being destroyed by disasters.
    /// </summary>
    [HarmonyPatch(typeof(CommonBuildingAI), nameof(CommonBuildingAI.CollapseBuilding))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony")]
    public static class CommonBuildingAIPatches
    {
        /// <summary>
        /// Simple Prefix patch to toggle excecution of game method based on current settings.
        /// </summary>
        /// <param name="__result">Original method result.</param>
        /// <param name="__instance">Harmony original instance reference.</param>
        /// <returns>False if the base method shouldn't be called (collapse has been prevented), true otherwise.</returns>
        public static bool Prefix(ref bool __result, CommonBuildingAI __instance)
        {
            if (ModSettings.noCollapse && RICOUtils.IsRICOPloppableAI(__instance as PrivateBuildingAI))
            {
                __result = false;

                // Don't call base method after this.
                return false;
            }

            // Continue on to base method
            return true;
        }
    }
}