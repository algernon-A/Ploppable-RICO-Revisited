// <copyright file="PrivateBuildingAIPatches.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using AlgernonCommons;
    using HarmonyLib;

    /// <summary>
    /// Harmony patches to disable despawning due to district style mismatch.
    /// </summary>
    [HarmonyPatch(typeof(PrivateBuildingAI), nameof(PrivateBuildingAI.SimulationStep))]
    public static class PrivateBuildingAIPatches
    {
        private static bool s_disableStyleDespawn = false;

        /// <summary>
        /// Gets or sets a value indicating whether building style forced despawning is disabled (true means disabled).
        /// </summary>
        internal static bool DisableStyleDespawn { get => s_disableStyleDespawn; set => s_disableStyleDespawn = value; }

        /// <summary>
        /// Harmony transpiler to PrivateBuildingAI.SimulationStep to disable despawning due to district style mismatch if relevant mod option is set.
        /// </summary>
        /// <param name="instructions">Original ILCode.</param>
        /// <returns>Modified ILCode.</returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Instruction parsing.
            IEnumerator<CodeInstruction> instructionsEnumerator = instructions.GetEnumerator();
            CodeInstruction instruction;
            bool inserted = false;

            Logging.KeyMessage("transpiling PrivateBuildingAI.SimulationStep");

            // Iterate through all instructions in original method.
            while (instructionsEnumerator.MoveNext())
            {
                // Get next instruction and add it to output.
                instruction = instructionsEnumerator.Current;
                yield return instruction;

                // Look for first (and only) ble in code.
                if (!inserted && instruction.opcode == OpCodes.Ble)
                {
                    // Found it - reflect disableStyleDespawn field.
                    FieldInfo disableField = AccessTools.Field(typeof(PrivateBuildingAIPatches), nameof(s_disableStyleDespawn));
                    if (disableField == null)
                    {
                        Logging.Error("couldn't reflect disableStyleDespawn");
                    }

                    // Insert check to our status override (if disableStyleDespawn...).
                    yield return new CodeInstruction(OpCodes.Ldsfld, disableField);
                    yield return new CodeInstruction(OpCodes.Brtrue, instruction.operand);

                    // Set flag.
                    inserted = true;
                }
            }
        }
    }
}