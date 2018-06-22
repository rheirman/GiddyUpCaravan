using Harmony;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace GiddyUpCaravan.Harmony
{

    /*
    [HarmonyPatch(typeof(CaravanArrivalTimeEstimator), "EstimatedTicksToArrive")]
    [HarmonyPatch(new Type[] { typeof(Caravan), typeof(bool)})]
    static class CaravanArrivalTimeEstimator_EstimatedTicksToArrive
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            for (var i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction instruction = instructionsList[i];
                if (instructionsList[i].operand == typeof(Caravan).GetMethod("get_TicksPerMove"))
                {
                    continue;
                }
                if (instructionsList[i].operand == typeof(CaravanArrivalTimeEstimator).GetMethod("EstimatedTicksToArrive", new Type[] { typeof(int), typeof(int), typeof(WorldPath), typeof(float), typeof(int), typeof(int)}))
                {
                    yield return new CodeInstruction(OpCodes.Call, typeof(Utilities.CaravanUtility).GetMethod("EstimatedTicksToArrive", new Type[] { typeof(int), typeof(int), typeof(WorldPath), typeof(float), typeof(Caravan), typeof(int) }));//Injected code     
                }
                else
                {
                    yield return instruction;
                }
            }


        }
    }
    [HarmonyPatch(typeof(CaravanArrivalTimeEstimator), "EstimatedTicksToArrive")]
    [HarmonyPatch(new Type[] { typeof(int), typeof(int), typeof(Caravan) })]
    static class CaravanArrivalTimeEstimator_EstimatedTicksToArrive2
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            for (var i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction instruction = instructionsList[i];
                if (instructionsList[i].operand == typeof(CaravanTicksPerMoveUtility).GetMethod("GetTicksPerMove", new Type[] {typeof(Caravan)}))
                {
                    continue;
                }
                if (instructionsList[i].operand == typeof(CaravanArrivalTimeEstimator).GetMethod("EstimatedTicksToArrive", new Type[] { typeof(int), typeof(int), typeof(WorldPath), typeof(float), typeof(int), typeof(int) }))
                {
                    yield return new CodeInstruction(OpCodes.Call, typeof(Utilities.CaravanUtility).GetMethod("EstimatedTicksToArrive", new Type[] { typeof(int), typeof(int), typeof(WorldPath), typeof(float), typeof(Caravan), typeof(int) }));//Injected code     
                }
                else
                {
                    yield return instruction;
                }
            }


        }
    }
    */
}
