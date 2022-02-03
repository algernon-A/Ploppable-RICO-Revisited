using HarmonyLib;


namespace PloppableRICO
{
    /// <summary>
    /// Harmony patch to stop RICO buildings being destroyed by disasters.
    /// </summary>
    [HarmonyPatch(typeof(CommonBuildingAI), nameof(CommonBuildingAI.CollapseBuilding))]
    public static class CollapseBuildingPatch
    {
        /// <summary>
        /// Simple Prefix patch to toggle excecution of game method based on current settings.
        /// </summary>
        /// <param name="__instance">Harmony original instance reference</param>
        /// <returns>False if the base method shouldn't be called (collapse has been prevented), true otherwise</returns>
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