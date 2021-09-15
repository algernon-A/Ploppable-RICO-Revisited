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
        /// Simple Prefix patch to catch Monuments panel setup exceptions.
        /// All we do is call (via reverse patch) the original method and painlessly catch any exceptions.
        /// </summary>
        /// <param name="__instance">Harmony original instance reference</param>
        /// <returns></returns>
        public static bool Prefix(ref bool __result, CommonBuildingAI __instance)
        {
            if (RICOUtils.IsRICOPloppableAI(__instance as PrivateBuildingAI))
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