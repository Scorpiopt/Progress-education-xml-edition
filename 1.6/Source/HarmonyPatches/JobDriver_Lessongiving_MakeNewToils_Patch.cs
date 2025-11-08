using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Progresseducationxmledition
{
    [HarmonyPatch(typeof(JobDriver_Lessongiving), "MakeNewToils")]
    public static class JobDriver_Lessongiving_MakeNewToils_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(JobDriver_Lessongiving __instance, ref IEnumerable<Toil> __result)
        {
            var driver = __instance;
            var pawn = driver.pawn;
            var existingToils = __result.ToList();
            var newToils = new List<Toil>();

            var startOfOriginalToils = new Toil();
            Toil findBell = Toils_General.Do(() =>
            {
                var electricBell = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(PEDefOf.PE_ElectricSchoolBell), PathEndMode.InteractionCell, TraverseParms.For(pawn));
                if (electricBell != null)
                {
                    var powerComp = electricBell.TryGetComp<CompPowerTrader>();
                    if (powerComp == null || powerComp.PowerOn)
                    {
                        var bellComp = electricBell.TryGetComp<CompBell>();
                        if (bellComp?.Props.soundDef != null)
                        {
                            bellComp.Props.soundDef.PlayOneShot(SoundInfo.InMap(new TargetInfo(electricBell.Position, pawn.Map)));
                            driver.JumpToToil(startOfOriginalToils);
                            return;
                        }
                    }
                }

                var manualBell = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.InteractionCell, TraverseParms.For(pawn), 9999f,
                    t => t.def == PEDefOf.PE_SchoolBell || t.def == PEDefOf.PE_TribalBell);

                if (manualBell != null)
                {
                    driver.job.SetTarget(TargetIndex.C, manualBell);
                }
            });

            newToils.Add(findBell);

            Toil jumpToOriginal = Toils_Jump.JumpIf(startOfOriginalToils, () => driver.job.GetTarget(TargetIndex.C).Thing == null);
            newToils.Add(jumpToOriginal);

            Toil ringBell = new Toil
            {
                initAction = () =>
                {
                    var bell = driver.job.GetTarget(TargetIndex.C).Thing;
                    var bellComp = bell.TryGetComp<CompBell>();
                    bellComp?.Props.soundDef.PlayOneShot(SoundInfo.InMap(new TargetInfo(bell.Position, pawn.Map)));
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };

            newToils.Add(Toils_Goto.GotoThing(TargetIndex.C, PathEndMode.InteractionCell).FailOnSomeonePhysicallyInteracting(TargetIndex.C));
            newToils.Add(ringBell);

            Toil waitToil = new Toil();
            waitToil.initAction = () =>
            {
                var bell = driver.job.GetTarget(TargetIndex.C).Thing;
                driver.pawn.rotationTracker.FaceTarget(bell);
                var bellComp = bell.TryGetComp<CompBell>();
                driver.ticksLeftThisToil = bellComp.Props.ticksToRing;
                waitToil.defaultDuration = bellComp.Props.ticksToRing;
            };
            waitToil.tickAction = () =>
            {
                var bell = driver.job.GetTarget(TargetIndex.C).Thing;
                driver.pawn.rotationTracker.FaceTarget(bell);
                var bellComp = bell.TryGetComp<CompBell>();
                if (driver.ticksLeftThisToil <= 0)
                {
                    driver.ReadyForNextToil();
                }
            };
            waitToil.handlingFacing = true;
            waitToil.defaultCompleteMode = ToilCompleteMode.Never;
            waitToil.WithProgressBarToilDelay(TargetIndex.C);
            newToils.Add(waitToil);

            newToils.Add(startOfOriginalToils);
            newToils.AddRange(existingToils);
            __result = newToils;
        }
    }
}
