// <copyright file="RefreshMonumentsPanelPatch.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System.Runtime.CompilerServices;
    using AlgernonCommons;
    using HarmonyLib;

    /// <summary>
    /// Harmony patch and reverse patch to catch exceptions when no valid monuments are avaliable (presumably because they've all been converted to Ploppable RICO buildings and/or skipped by LSM prefab skipping).
    /// </summary>
    [HarmonyPatch(typeof(UnlockingPanel), "RefreshMonumentsPanel")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony")]
    public static class RefreshMonumentsPanelPatch
    {
        /// <summary>
        /// Simple Prefix patch to catch Monuments panel setup exceptions.
        /// All we do is call (via reverse patch) the original method and painlessly catch any exceptions.
        /// </summary>
        /// <param name="__instance">Harmony original instance reference.</param>
        /// <returns>Always false (never execute original method).</returns>
        public static bool Prefix(UnlockingPanel __instance)
        {
            try
            {
                RefreshMonumentsPanelRev(__instance);
            }
            catch
            {
                Logging.Message("caught monuments panel exception");
            }

            // Don't call base method after this.
            return false;
        }

        /// <summary>
        /// Harmony reverse patch to access original private method.
        /// </summary>
        /// <param name="instance">Harmony original instance reference.</param>
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(UnlockingPanel), "RefreshMonumentsPanel")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void RefreshMonumentsPanelRev(object instance)
        {
            Logging.Error("RefreshMonumentsPanel reverse Harmony patch wasn't applied ", instance);
        }
    }
}