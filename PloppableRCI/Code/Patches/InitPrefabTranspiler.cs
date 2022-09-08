namespace PloppableRICO
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using AlgernonCommons;
    using HarmonyLib;

    /// <summary>
    /// Harmony transpiler to remove two checks from BuildingInfo.InitializePrefab which throw exceptions when private building types have manual (ploppable) placement styles or include net assets.
    /// Removing these checks enables Ploppable RICO buildings to be initialised as RICO assets on load (instead of doing funky things to prefabs already initialised in their 'native' state).
    /// This approach fixes a whole bunch of issues, enables code simplifications, and is required to properly enable 'Growable RICO'.
    /// </summary>
    [HarmonyPatch(typeof(BuildingInfo), "InitializePrefab")]
    public static class InitPrefabTranspiler
    {
        /// <summary>
        /// Harmony transpiler removing two checks from BuildingInfo.InitializePrefab.
        /// </summary>
        /// <param name="instructions">CIL code to alter.</param>
        /// <returns></returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // The checks we're targeting for removal are fortunately clearly defined by their exception operands.
            // We're only going to remove the exception throw itself (including stack loading), and not the preceeding conditional check.
            // This minimises our footprint and reduces the chance of conflict with other transpilers,
            // and also means we don't have to worry about dealing with any branches to the start of the check.
            string[] targetOperands =
            {
                "Private building cannot have manual placement style",
                "Private building cannot include roads or other net types"
            };

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            // Deal with each of the operands consecutively and independently to avoid risk of error.
            foreach (string targetOperand in targetOperands)
            {
                // Stores the number of operands to cut.
                int cutCount = 0;

                // Iterate through each opcode in the CIL, looking for an ldarg.0 immediately followed by an ldstr.
                for (int i = 0; i < codes.Count; i++)
                {
                    if ((codes[i].opcode == OpCodes.Ldarg_0) && (codes[i + 1].opcode == OpCodes.Ldstr))
                    {
                        // Found a matching combo - now check the ldstr operand aginst our target.
                        if (codes[i + 1].operand.Equals(targetOperand))
                        {
                            // Operand match - now count forward from this operand until we encounter an exception throw.
                            while (codes[i + cutCount].opcode != OpCodes.Throw)
                            {
                                cutCount++;
                            }

                            Logging.Message("InitPrefab transpiler removing CIL (offset ", cutCount, ") from ", i, " (", codes[i].opcode, " to ", codes[i + cutCount].opcode, ") - ", targetOperand);

                            // Remove the CIL from the ldarg.0 to the throw (inclusive).
                            // +1 to avoid fencepost error (need to include original instruction as well).
                            codes.RemoveRange(i, cutCount + 1);

                            // We're done with this one - no point in continuing the loop.
                            break;
                        }
                    }
                }
            }

            return codes.AsEnumerable();
        }
    }
}