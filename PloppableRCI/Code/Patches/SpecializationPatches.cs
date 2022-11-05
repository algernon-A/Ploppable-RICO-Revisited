// <copyright file="SpecializationPatches.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using AlgernonCommons;
    using AlgernonCommons.Patching;
    using ColossalFramework;
    using HarmonyLib;

    /// <summary>
    /// Harmony transpilers to add checks to see if buildings should be demolished if they're outside of a district with relevant specialization settings.
    /// </summary>
    [HarmonyPatch]
    public static class SpecializationPatches
    {
        /// <summary>
        /// Harmony Transpiler to add checks to see if commercial buildings should be demolished if they're outside of a district with relevant specialization settings.
        /// </summary>
        /// <param name="instructions">Original ILCode.</param>
        /// <param name="original">Method being patched.</param>
        /// <returns>Modified ILCode.</returns>
        [HarmonyPatch(typeof(CommercialBuildingAI), "SimulationStep")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> CommercialBuildingAITranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original) => SpecCheckTranspiler(instructions, original);

        /// <summary>
        /// Harmony Transpiler to add checks to see if residential buildings should be demolished if they're outside of a district with relevant specialization settings.
        /// </summary>
        /// <param name="instructions">Original ILCode.</param>
        /// <param name="original">Method being patched.</param>
        /// <returns>Modified ILCode.</returns>
        [HarmonyPatch(typeof(ResidentialBuildingAI), "SimulationStep")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ResidentialBuildingAITranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original) => SpecCheckTranspiler(instructions, original);

        /// <summary>
        /// Harmony Transpiler to add checks to see if office buildings should be demolished if they're outside of a district with relevant specialization settings.
        /// </summary>
        /// <param name="instructions">Original ILCode.</param>
        /// <param name="original">Method being patched.</param>
        /// <returns>Modified ILCode.</returns>
        [HarmonyPatch(typeof(OfficeBuildingAI), "SimulationStep")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> OfficeBuildingAITranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original) => SpecCheckTranspiler(instructions, original);

        /// <summary>
        /// Harmony Transpiler to add checks to see if industrial buildings should be demolished if they're outside of a district with relevant specialization settings.
        /// </summary>
        /// <param name="instructions">Original ILCode.</param>
        /// <param name="original">Method being patched.</param>
        /// <returns>Modified ILCode.</returns>
        [HarmonyPatch(typeof(IndustrialBuildingAI), "SimulationStep")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> IndustrialBuildingAITranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original) => SpecCheckTranspiler(instructions, original);

        /// <summary>
        /// Harmony Transpiler to add checks to see if extractor buildings should be demolished if they're outside of a district with relevant specialization settings.
        /// </summary>
        /// <param name="instructions">Original ILCode.</param>
        /// <param name="original">Method being patched.</param>
        /// <returns>Modified ILCode.</returns>
        [HarmonyPatch(typeof(IndustrialExtractorAI), "SimulationStep")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> IndustrialExtractorAITranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original) => SpecCheckTranspiler(instructions, original);

        /// <summary>
        /// Harmony transpiler that replaces specific code blocks with a call to a custom replacement method.
        /// The code blocks are where buildings are normally assigned the 'Demolishing' flag because they're not within a district with the relevant specialisation.
        /// </summary>
        /// <param name="instructions">Original ILCode.</param>
        /// <param name="original">Method being patched.</param>
        /// <returns>Modified ILCode.</returns>
        private static IEnumerable<CodeInstruction> SpecCheckTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            // Replacing this code block with a call to our custom replacement method:

            /* buildingData.m_flags |= Building.Flags.Demolishing;
             *
             * ldarg.2
             * dup
             * ldfld valuetype Building / Flags Building::m_flags
             * ldc.i4 524288
             * or
             * stfld valuetype Building / Flags Building::m_flags
             *
             * instance.m_currentBuildIndex++;
             *
             * ldloc.0
             * dup
             * ldfld uint32 SimulationManager::m_currentBuildIndex
             * ldc.i4.1
             * add
             * stfld uint32 SimulationManager::m_currentBuildIndex
            */

            // Offset of the last instruction relative to the first (i.e. first + this = last).
            const int endOffset = 11;

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            Logging.Message("transpiler patching specialized building checks in ", PatcherBase.PrintMethod(original));

            // Iterate through each opcode in the ILCode, looking for an ldarg.2.
            // Technically a for-next loop that we reset, but there's a bit of code smell about doing it that way, so we use a while loop instead to make what we're doing more explicit.
            int i = 0;

            // Don't bother counting within endOffset of the end, as there's no point.
            // Ending early also eliminates chance of overflow without having to add extra checks within the loop.
            while (i < codes.Count - endOffset)
            {
                // Look for an ldarg.2 to signal a potential candidate.
                if (codes[i].opcode == OpCodes.Ldarg_2)
                {
                    // Found ldarg.2; check ahead for Building.Flags.Demolishing.
                    CodeInstruction checkOp = codes[i + 3];

                    // Look for a clear guide to confirm that this is our code block - loading Building.Flags.Demolishing.
                    if (checkOp.opcode == OpCodes.Ldc_I4 && checkOp.operand as int? == (int)Building.Flags.Demolishing)
                    {
                        // Found ldc.i4 Building.Flags.Demolishing; check ahead for m_currentBuildIndex.
                        checkOp = codes[i + endOffset];

                        if (checkOp.opcode == OpCodes.Stfld && checkOp.operand == AccessTools.Field(typeof(SimulationManager), "m_currentBuildIndex"))
                        {
                            // Found stfld SimulationManager::m_currentBuildIndex.  This is confirmation that we're where we need to be.
                            // Now remove the ILCode from the ldarg.2 to the stfld (inclusive).
                            // +1 to avoid fencepost error (need to include original instruction as well).
                            codes.RemoveRange(i, endOffset + 1);

                            // Add new call to our custom method instead.
                            CodeInstruction callOp = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SpecializationPatches), nameof(CheckSpecial)));
                            codes.Insert(i, callOp);

                            // Make sure we put our 'ref Building' argument on the stack first; insert this immediately ahead of the call instruction.
                            callOp = new CodeInstruction(OpCodes.Ldarg_2);
                            codes.Insert(i, callOp);

                            // Reset i to zero to restart.
                            // We want to replace *all* instances of this code block within the target method, so keep going until the loop naturally reaches the end (no replacement).
                            i = 0;
                        }
                    }
                }

                // Increment counter to next instruction.
                ++i;
            }

            return codes.AsEnumerable();
        }

        /// <summary>
        /// Checks to see if a building should be demolished if it's not in a district with the relevant specialisation.
        /// Called via Harmony Transpiler patches.
        /// </summary>
        /// <param name="buildingData">Building data.</param>
        private static void CheckSpecial(ref Building buildingData)
        {
            bool isRICO = RICOUtils.IsRICOAI(buildingData.Info.GetAI() as PrivateBuildingAI);

            // Check if the relevant 'ignore specialisation' setting is set.
            // If it is, we just don't do anything.  Otherwise, we mimic the base game's code for this ocurrence.
            if (!(ModSettings.noSpecOther && !isRICO) && !(ModSettings.noSpecRico && isRICO))
            {
                buildingData.m_flags |= Building.Flags.Demolishing;
                Singleton<SimulationManager>.instance.m_currentBuildIndex++;
            }
        }
    }
}
