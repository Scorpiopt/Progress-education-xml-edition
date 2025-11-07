using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Progresseducationxmledition
{
    [HarmonyPatch(typeof(SchoolUtility), "ClosestSchoolDesk")]
    public static class SchoolUtility_ClosestSchoolDesk_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Thing __result, Pawn child, Pawn teacher)
        {
            if (__result == null && teacher != null)
            {
                __result = GenClosest.ClosestThingReachable(child.Position, child.Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.InteractionCell, TraverseParms.For(child), 9999f, 
                    (Thing d) => (d.def == PEDefOf.SE_RoughSchoolDesk || d.def == PEDefOf.SE_SpacerSchoolDesk) && 
                                 child.CanReserveSittableOrSpot(SchoolUtility.DeskSpotStudent(d)) && 
                                 teacher.CanReserveSittableOrSpot(SchoolUtility.DeskSpotTeacher(d)) && 
                                 !d.IsForbidden(child) && 
                                 !d.IsForbidden(teacher));
            }
        }
    }
}