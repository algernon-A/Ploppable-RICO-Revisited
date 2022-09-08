namespace PloppableRICO
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Linq;
    using AlgernonCommons;
    using HarmonyLib;

    /// <summary>
    /// Harmony transpiler to patch PrivateBuildingAI.SimulationStep calls to Building.CheckZoning, to implement 'no zoning checks' functionality.
    /// Done this way (instead of e.g. patching Building.CheckZoning directly) for compatibility with 81 tiles mod, which detours (!) Building.CheckZoning.
    /// </summary>
    [HarmonyPatch(typeof(PrivateBuildingAI), "SimulationStep")]
    public static class SimulationStepPatch
    {
        /// <summary>
        /// Harmony Transpiler - finds calls to Building.CheckZoning in PrivateBuildingAI.SimulationStep and replaces them with calls to our own 'detour-compatible-prefix'.
        /// </summary>
        /// <param name="instructions">Original ILCode</param>
        /// <returns>Replacement (patched) ILCode</returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Logging.Message("transpiler patching CheckZoning calls in PrivateBuildingAI.SimulationStep");
            return CheckZoningTranspiler.Transpiler(instructions);
        }
    }


    /// <summary>
    /// Harmony transpiler to patch PrivateBuildingAI.CheckNearbyBuildingZones calls to Building.CheckZoning, to implement 'no zoning checks' functionality.
    /// Done this way (instead of e.g. patching Building.CheckZoning directly) for compatibility with 81 tiles mod, which detours (!) Building.CheckZoning.
    /// </summary>
    [HarmonyPatch(typeof(PrivateBuildingAI), "CheckNearbyBuildingZones")]
    public static class CheckNearbyBuildingZonesPatch
    {
        /// <summary>
        /// Harmony Transpiler - finds calls to Building.CheckZoning in PrivateBuildingAI.CheckNearbyBuildingZones and replaces them with calls to our own 'detour-compatible-prefix'.
        /// </summary>
        /// <param name="instructions">Original ILCode</param>
        /// <returns>Replacement (patched) ILCode</returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Logging.Message("transpiler patching CheckZoning calls in PrivateBuildingAI.CheckNearbyBuildingZones");
            return CheckZoningTranspiler.Transpiler(instructions);
        }
    }


    /// <summary>
    /// Harmony transpiler to patch calls to Building.CheckZoning with a custom 'detour-compatible-prefix', to implement 'no zoning checks' functionality.
    /// </summary>
    public static class CheckZoningTranspiler
    {
        /// <summary>
        /// Simple transpiler - just finds calls to Building.CheckZoning in PrivateBuildingAI.CheckNearbyBuildingZones and replaces them with calls to our own 'detour-compatible-prefix'.
        /// No other changes are needed, and all relevant calls need to be patched, so there's no special start/end detection required.
        /// </summary>
        /// <param name="instructions">Original ILCode to patch</param>
        /// <returns>Replacement (patched) ILCode</returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Get list of original opcodes and create our replacement list.
            List<CodeInstruction> code = new List<CodeInstruction>(instructions);

            // Iterate through each opcode in the original CIL.
            for (int i = 0; i < code.Count; i++)
            {
                // Get local reference.
                CodeInstruction thisInstruction = code[i];

                // Look for call to Building.CheckZoning.
                if (thisInstruction.opcode == OpCodes.Call && thisInstruction.operand == AccessTools.Method(typeof(Building), "CheckZoning", parameters: new Type[] { typeof(ItemClass.Zone), typeof(ItemClass.Zone), typeof(bool) }))
                {
                    // Found one - just change the operand to our 'detour-compatibile-prefix'.
                    // The Building struct reference for the original method call should be left on the stack and will be picked up as the first argument of our custom method.
                    thisInstruction.operand = AccessTools.Method(typeof(CheckZoningTranspiler), "NewZoneCheck");
                }
            }

            // Return patched code.
            return code.AsEnumerable();
        }


        /// <summary>
        /// A 'detour-compatible-prefix' replacement for Building.CheckZoning calls.
        /// Our transpiler inserts calls to this in place of original calls to BuildingData.CheckZoning.
        /// </summary>
        /// <param name="buildingData">Original Building reference (left on stack as the original reference to the Building instance for the original method call)</param>
        /// <param name="zone1">Original method argument (untouched)</param>
        /// <param name="zone2">Original method argument (untouched)</param>
        /// <param name="allowCollapse">Original method (untouched)</param>
        /// <returns>True if this is a building covered by 'no zoning checks', otherwise will return whatever the base method result is</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool NewZoneCheck(ref Building buildingData, ItemClass.Zone zone1, ItemClass.Zone zone2, bool allowCollapse)
        {
            // Check if this building is RICO or not.
            bool isRICO = RICOUtils.IsRICOAI(buildingData.Info.GetAI() as PrivateBuildingAI);

            // Check if the relevant 'ignore zoning' setting is set.
            if ((ModSettings.noZonesOther && !isRICO) || (ModSettings.noZonesRico && isRICO))
            {
                // It is - return true (tell the game we're in a valid zone).
                return true;
            }

            // If we got here, this isn't a building covered by our settings: call original method and return its result.
            return buildingData.CheckZoning(zone1, zone2, allowCollapse);
        }
    }
}