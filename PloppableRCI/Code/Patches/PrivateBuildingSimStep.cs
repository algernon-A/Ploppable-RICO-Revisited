namespace PloppableRICO
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using AlgernonCommons;
    using HarmonyLib;

    [HarmonyPatch(typeof(PrivateBuildingAI), nameof(PrivateBuildingAI.SimulationStep))]
    public static class PrivateBuildingSimStep
    {
        public static bool disableStyleDespawn = false;

        /// <summary>
        /// Harmony transpiler to disable despawning due to district style mismatch if relevant mod option is set.
        /// </summary>
        /// <param name="instructions">CIL code to alter.</param>
        /// <returns></returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Instruction parsing.
            IEnumerator<CodeInstruction> instructionsEnumerator = instructions.GetEnumerator();
            CodeInstruction instruction;
            //Label targetLabel = generator.DefineLabel();
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
                    FieldInfo disableField = typeof(PrivateBuildingSimStep).GetField(nameof(disableStyleDespawn), BindingFlags.Static | BindingFlags.Public);
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