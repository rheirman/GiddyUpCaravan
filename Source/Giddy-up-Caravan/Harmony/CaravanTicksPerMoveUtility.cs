using GiddyUpCaravan.Utilities;
using GiddyUpCore.Storage;
using Harmony;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;

namespace GiddyUpCaravan.Harmony
{
    [HarmonyPatch(new Type[] { typeof(List<Pawn>) })]
    [HarmonyPatch(typeof(CaravanTicksPerMoveUtility), "GetTicksPerMove")]
    class CaravanTicksPerMoveUtility_GetTicksPerMove
    {

        static void Postfix(List<Pawn> pawns, ref int __result)
        {
            int pawnsWithMount = 0;
            int pawnsWithoutMount = 0;
          
            foreach(Pawn pawn in pawns)
            {
                ExtendedPawnData pawndata = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(pawn);
                if(pawn.IsColonist && pawn.ridingCaravanMount())
                {
                    pawnsWithMount++;
                }
                else if (pawn.IsColonist && !pawn.ridingCaravanMount())
                {
                    pawnsWithoutMount++;
                }
            }
            
            if(pawnsWithoutMount == 0)
            {
                Log.Message("no pawns without mount, speed bonus applied");
                __result = Mathf.RoundToInt(__result / ((100f + Base.completeCaravanBonus.Value) / 100));
            }
            else
            {
                Log.Message("pawnsWithoutMount: " + pawnsWithoutMount);
                float isMountedFraction =  (float) pawnsWithMount / (pawnsWithMount + pawnsWithoutMount - 1);
                Log.Message("result before: " + __result);
                __result = Mathf.RoundToInt(__result / ((100f + isMountedFraction * Base.incompleteCaravanBonusCap.Value) / 100f));
                Log.Message("result after: " + __result);

            }


        }

        /*
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            bool flag = false;
            for (var i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction instruction = instructionsList[i];
                yield return instruction;
                if (instructionsList[i].operand == typeof(Pawn).GetMethod("TicksPerMoveCardinal"))
                {
                    flag = true;
                }
                if(flag && instructionsList[i].opcode == OpCodes.Stloc_2)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_2);//load num2 local variable
                    yield return new CodeInstruction(OpCodes.Call, typeof(CaravanTicksPerMoveUtility_GetTicksPerMove).GetMethod("adjustTicksPerMove"));//Injected code     
                    yield return new CodeInstruction(OpCodes.Stloc_2);//load num2 local variable
                }
            }
        }
        */
        //public int adjustTicksPerMove(Pawn pawn)
        //{
        //    return ticks;
        //}
    }
}
