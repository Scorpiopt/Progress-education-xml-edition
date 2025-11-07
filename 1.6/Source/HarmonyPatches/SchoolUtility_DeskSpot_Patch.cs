using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Progresseducationxmledition
{
    [HarmonyPatch]
    public static class SchoolUtility_DeskSpot_Patch
    {
        public static bool IsDesk(ThingDef thing)
        {
            return thing == ThingDefOf.SchoolDesk || thing == PEDefOf.SE_RoughSchoolDesk || thing == PEDefOf.SE_SpacerSchoolDesk;
        }

        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(SchoolUtility), "DeskSpotTeacher");
            yield return AccessTools.Method(typeof(SchoolUtility), "DeskSpotStudent");
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool done = false;
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Beq_S)
                {
                    yield return new CodeInstruction(OpCodes.Brtrue, instruction.operand);
                }
                else if (instruction.opcode == OpCodes.Ldsfld && !done)
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SchoolUtility_DeskSpot_Patch), "IsDesk"));
                    done = true;
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}