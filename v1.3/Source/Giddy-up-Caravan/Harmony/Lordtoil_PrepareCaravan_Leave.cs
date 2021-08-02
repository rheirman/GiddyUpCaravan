using GiddyUpCore.Jobs;
using GiddyUpCore.Storage;
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
    [HarmonyPatch(typeof(LordToil_PrepareCaravan_Leave), "UpdateAllDuties")]
    static class Lordtoil_PrepareCaravan_Leave_UpdateAllDuties
    {
        static void Prefix(LordToil_PrepareCaravan_Leave __instance)
        {
            AddMissingPawnsToLord(__instance);
            foreach (Pawn pawn in __instance.lord.ownedPawns)
            {
                ExtendedPawnData pawnData = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(pawn);
                if (pawnData.caravanMount != null)
                {
                    Pawn animal = pawnData.caravanMount;
                    Job jobRider = new Job(GUC_JobDefOf.Mount, animal);
                    jobRider.count = 1;
                    pawn.jobs.TryTakeOrderedJob(jobRider);
                    animal.jobs.StopAll();
                    animal.pather.StopDead();
                    Job jobAnimal = new Job(GUC_JobDefOf.Mounted, pawn);
                    jobAnimal.count = 1;
                    animal.jobs.TryTakeOrderedJob(jobAnimal);
                }
            }
        }

        //For compatibility with other mods (Save our ship 2), add any missing mounts to the lord. 
        private static void AddMissingPawnsToLord(LordToil_PrepareCaravan_Leave __instance)
        {
            List<Pawn> shouldAddOwnedPawns = new List<Pawn>();
            foreach (Pawn pawn in __instance.lord.ownedPawns)
            {
                ExtendedPawnData pawnData = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(pawn);
                bool caravanContainsMount = __instance.lord.ownedPawns.Contains(pawnData.caravanMount);
                if (pawnData.caravanMount != null && !caravanContainsMount)
                {
                    shouldAddOwnedPawns.Add(pawnData.caravanMount);
                }
            }
            var exitSpot = Traverse.Create(__instance).Field("exitSpot").GetValue<IntVec3>();
            foreach (Pawn pawn in shouldAddOwnedPawns)
            {
                __instance.lord.ownedPawns.Add(pawn);
                pawn.mindState.duty = new PawnDuty(DutyDefOf.TravelOrWait, exitSpot);
            }
        }
    }
}
