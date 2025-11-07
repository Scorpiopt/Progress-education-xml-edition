using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace Progresseducationxmledition
{
    [HarmonyPatch(typeof(LearningUtility), "ConnectedBlackboards")]
    public static class LearningUtility_ConnectedBlackboards_Patch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int popped = 0;
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldfld || (popped > 0 && popped < 3))
                {
                    popped++;
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}