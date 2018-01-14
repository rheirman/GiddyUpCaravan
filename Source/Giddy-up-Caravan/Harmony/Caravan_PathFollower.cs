using Harmony;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using GiddyUpCore.Utilities;
using UnityEngine;
using Verse;

namespace GiddyUpCaravan.Harmony
{
    [HarmonyPatch(new Type[] { typeof(Caravan), typeof(int), typeof(int), typeof(float) })]
    [HarmonyPatch(typeof(Caravan_PathFollower), "CostToMove")]
    class Caravan_PathFollower_CostToMove
    {

        static void Postfix(ref Caravan caravan, int start, int end, float yearPercent, ref int __result)
        {

            Log.Message("called Caravan_PathFollower_CostToMove");
            if(caravan != null)
            {
                __result = Utilities.CaravanUtility.CostToMove(caravan.PawnsListForReading, start, end, yearPercent);
            }
        }

    }
    [HarmonyPatch(typeof(Caravan_PathFollower), "CostToDisplay")]
    [HarmonyPatch(new Type[] { typeof(Caravan), typeof(int), typeof(int), typeof(float) })]
    class Caravan_PathFollower_CostToDisplay
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            bool flag = false;
            for (var i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction instruction = instructionsList[i];

                if (!flag && instructionsList[i].operand == typeof(Caravan).GetMethod("get_TicksPerMove")) {
                    continue;
                }
                if (instructionsList[i].operand == typeof(Caravan_PathFollower).GetMethod("CostToMove", new Type[] { typeof(int), typeof(int), typeof(int), typeof(float) }))
                {
                    yield return new CodeInstruction(OpCodes.Call, typeof(Caravan_PathFollower).GetMethod("CostToMove", new Type[] { typeof(Caravan), typeof(int), typeof(int), typeof(float) }));//Injected code     
                    flag = true;
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
    
}
