using HarmonyLib;
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
    ///Removed this patch with Rimworld 1.3 since that adds caravan mounting speed bonus by default. 

    /*
    [HarmonyPatch(new Type[] {typeof(int), typeof(int), typeof(int), typeof(int), typeof(bool), typeof(StringBuilder), typeof(String) , typeof(bool)})]
    [HarmonyPatch(typeof(Caravan_PathFollower), "CostToMove")]
    static class Caravan_PathFollower_CostToMove
    {
        static void Postfix(ref int __result, int caravanTicksPerMove, StringBuilder explanation)
        {
            int oldResult = __result;
            __result = Utilities.CaravanUtility.applySpeedBonus(__result, Base.Instance.curCaravanPawns, explanation);
            if(explanation != null && oldResult != __result)
            {
                explanation.AppendLine("  "+ "GU_Car_Explanation_FinalSpeed".Translate() + ": " + (60000f / __result).ToString("n1") + " " + "TilesPerDay".Translate());
            }
        }
    }
    */


    /*
    [HarmonyPatch(new Type[] { typeof(Caravan), typeof(int), typeof(int), typeof(float), typeof(bool), typeof(StringBuilder),typeof(String) })]
    [HarmonyPatch(typeof(Caravan_PathFollower), "CostToMove")]
    class Caravan_PathFollower_CostToMove
    {

        static void Postfix(ref Caravan caravan, int start, int end, float yearPercent, ref int __result)
        {

            if(caravan != null)
            {
                __result = Utilities.CaravanUtility.CostToMove(caravan, start, end, yearPercent);
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
    */

}
