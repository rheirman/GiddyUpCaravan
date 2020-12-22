using GiddyUpCore.Jobs;
using GiddyUpCore.Storage;
using GiddyUpCore.Utilities;
using GiddyUpCore.Zones;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace GiddyUpCaravan.Harmony
{


    [HarmonyPatch(typeof(Pawn_JobTracker), "DetermineNextJob")]
    static class Pawn_JobTracker_DetermineNextJob
    {

        static void Postfix(Pawn_JobTracker __instance, ref ThinkResult __result, ref Pawn ___pawn)
        {
            if (___pawn.RaceProps.Animal && ___pawn.Faction != Faction.OfPlayer && ___pawn.Faction != null)
            {

                if (___pawn.GetLord() != null && (___pawn.GetLord().CurLordToil is LordToil_DefendPoint || ___pawn.GetLord().CurLordToil.GetType().Name == "LordToil_DefendTraderCaravan"))
                {

                    if (__result.SourceNode is JobGiver_Wander)
                    {

                        JobGiver_Wander jgWander = (JobGiver_Wander)__result.SourceNode;
                        Traverse.Create(__result.SourceNode).Field("wanderRadius").SetValue(5f);
                    }

                }

            }
            //Check if pawn is enemy and can mount.
            if (___pawn.IsColonistPlayerControlled || ___pawn.IsBorrowedByAnyFaction() || ___pawn.RaceProps.Animal || ___pawn.Faction.HostileTo(Faction.OfPlayer) || !___pawn.RaceProps.Humanlike)
            {            
                return;
            }
            if (___pawn.IsPrisoner)
            {
                return;
            }
            if(__result.Job == null) //shouldn't happen, but may happen with mods.
            {
                return;
            }

            LocalTargetInfo target = DistanceUtility.GetFirstTarget(__result.Job, TargetIndex.A);
            if (!target.IsValid)
            {
                return;
            }

            ExtendedDataStorage store = Base.GetExtendedDataStorage();
            if (store == null)
            {
                return;
            }

            //Log.Message("wrong duty");
            ExtendedPawnData PawnData = store.GetExtendedDataFor(___pawn);
            Lord lord = ___pawn.GetLord();
            if (lord == null)
            {
                return;

            }
            if(__result.Job.def == GUC_JobDefOf.Dismount || __result.Job.def == GUC_JobDefOf.Mount)
            {
                return;
            }

            QueuedJob qJob = ___pawn.jobs.jobQueue.FirstOrFallback(null);
            if(qJob != null && (qJob.job.def == GUC_JobDefOf.Dismount || qJob.job.def == GUC_JobDefOf.Mount))
            {
                return;
            }

            if (lord.CurLordToil is LordToil_ExitMapAndEscortCarriers || lord.CurLordToil is LordToil_Travel || lord.CurLordToil is LordToil_ExitMap || lord.CurLordToil is LordToil_ExitMapTraderFighting)
            {
                if (PawnData.owning != null &&
                    PawnData.owning.Faction == ___pawn.Faction &&
                    PawnData.mount == null && 
                    !PawnData.owning.Downed &&
                    PawnData.owning.Spawned && 
                    !___pawn.IsBurning() &&
                    !___pawn.Downed)
                {
                    mountAnimal(__instance, ___pawn, PawnData, ref __result);

                }
            }
            else if(lord.CurLordToil.GetType().Name == "LordToil_DefendTraderCaravan" || lord.CurLordToil is LordToil_DefendPoint) //first option is internal class, hence this way of accessing. 
            {
                if (PawnData.mount != null)
                {
                    parkAnimal(__instance, ___pawn, PawnData);
                }
            }
        }
        
        private static void mountAnimal(Pawn_JobTracker __instance, Pawn pawn, ExtendedPawnData pawnData, ref ThinkResult __result)
        {
            Job oldJob = __result.Job;
            Job mountJob = new Job(GUC_JobDefOf.Mount, pawnData.owning);
            mountJob.count = 1;
            __result = new ThinkResult(mountJob, __result.SourceNode, __result.Tag, false);
            __instance.jobQueue.EnqueueFirst(oldJob);
        }

        private static void parkAnimal(Pawn_JobTracker __instance, Pawn pawn, ExtendedPawnData pawnData)
        {
            Area_GU areaFound = (Area_GU) pawn.Map.areaManager.GetLabeled(Base.DropAnimal_NPC_LABEL);
            IntVec3 targetLoc = pawn.Position;

            if(areaFound != null && areaFound.ActiveCells.Count() > 0)
            {
                targetLoc = DistanceUtility.getClosestAreaLoc(pawn, areaFound);
            }
            if (pawn.Map.reachability.CanReach(pawn.Position, targetLoc, PathEndMode.OnCell, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false)))
            {
                Job dismountJob = new Job(GUC_JobDefOf.Dismount);
                dismountJob.count = 1;
                __instance.jobQueue.EnqueueFirst(dismountJob);
                __instance.jobQueue.EnqueueFirst(new Job(JobDefOf.Goto, targetLoc));
                PawnDuty animalDuty = pawnData.mount.mindState.duty;
                //if(pawnData.mount.GetLord().CurLordToil is LordToil)

                if(animalDuty != null)
                {
                    animalDuty.focus = new LocalTargetInfo(targetLoc);
                }
            }
            else
            {
                Messages.Message("GU_Car_NotReachable_DropAnimal_NPC_Message".Translate(), new RimWorld.Planet.GlobalTargetInfo(targetLoc, pawn.Map), MessageTypeDefOf.NegativeEvent);
            }
        }


    }
}
