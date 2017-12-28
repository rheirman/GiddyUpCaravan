using GiddyUpCore.Jobs;
using GiddyUpCore.Storage;
using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace GiddyUpCaravan.Harmony
{
    [HarmonyPatch(typeof(LordToil_PrepareCaravan_Leave), "UpdateAllDuties")]
    static class Lordtoil_PrepareCaravan_Leave_UpdateAllDuties
    {
        static void Prefix(LordToil_PrepareCaravan_Leave __instance)
        {
            foreach (Pawn pawn in __instance.lord.ownedPawns)
            {
                Log.Message("UpdateAllDuties called");
                ExtendedPawnData pawnData = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(pawn);
                if(pawnData.caravanMount != null && __instance.lord.ownedPawns.Contains(pawnData.caravanMount))
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
    }
}
