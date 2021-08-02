using GiddyUpCore.Jobs;
using GiddyUpCore.Storage;
using HarmonyLib;
using Multiplayer.API;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace GiddyUpCaravan.Harmony
{
    [HarmonyPatch(typeof(CaravanEnterMapUtility), "Enter")]
    [HarmonyPatch(new Type[] { typeof(Caravan), typeof(Map), typeof(Func<Pawn, IntVec3>), typeof(CaravanDropInventoryMode), typeof(bool) })]
    class CaravanEnterMapUtility_Enter
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            foreach (CodeInstruction instruction in instructionsList)
            {
                yield return instruction;
                if (instruction.operand == typeof(Caravan).GetMethod("RemoveAllPawns"))
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(CaravanEnterMapUtility), "tmpPawns"));
                    yield return new CodeInstruction(OpCodes.Call, typeof(CaravanEnterMapUtility_Enter).GetMethod("MountCaravanMounts"));
                }
            }

        }
        [SyncMethod]
        public static void MountCaravanMounts(List<Pawn> pawns)
        {

            foreach (Pawn pawn in pawns)
            {
                if (pawn.IsColonist && Base.Instance.GetExtendedDataStorage() is ExtendedDataStorage store && pawn.Spawned)
                {
                    ExtendedPawnData pawnData = store.GetExtendedDataFor(pawn);
                    if (pawnData.caravanMount is Pawn animal)
                    {
                        ExtendedPawnData animalData = store.GetExtendedDataFor(animal);
                        pawnData.mount = animal;
                        Job jobAnimal = new Job(GUC_JobDefOf.Mounted, pawn);
                        jobAnimal.count = 1;
                        animal.jobs.TryTakeOrderedJob(jobAnimal);
                    }
                }

            }
        }
    }
}
