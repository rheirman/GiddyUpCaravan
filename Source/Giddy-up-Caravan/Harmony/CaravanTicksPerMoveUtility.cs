using GiddyUpCaravan.Utilities;
using GiddyUpCore.Storage;
using GiddyUpCore.Utilities;
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
            ExtendedDataStorage store = GiddyUpCore.Base.Instance.GetExtendedDataStorage();
            if(store == null)
            {
                return;
            }
            
            foreach (Pawn pawn in pawns)
            {
                ExtendedPawnData pawnData = store.GetExtendedDataFor(pawn);
                if(pawnData != null && pawn.IsColonist && pawn.ridingCaravanMount())
                {
                    pawnsWithMount++;
                }
                else if (pawn.IsColonist)
                {
                    pawnsWithoutMount++;
                }
            }
            
            if(pawnsWithoutMount == 0) //no pawns without mount, apply full speed bonus
            {
                __result = Mathf.RoundToInt(__result / ((100f + Base.completeCaravanBonus.Value) / 100));
            }
            else //otherwise apply small per mount bonus
            {
                //Log.Message("pawnsWithoutMount: " + pawnsWithoutMount);

                int total = pawnsWithMount + pawnsWithoutMount;
                int adjustedTotal = total > 1 ? total- 1 : 1; //adjusted total makes sure incompleteCaravanBonusCap is achievable and prevents div/0. 
                float isMountedFraction =  (float) pawnsWithMount / adjustedTotal;
                //Log.Message("isMountedFraction: " + isMountedFraction);
                __result = Mathf.RoundToInt(__result / ((100f + isMountedFraction * Base.incompleteCaravanBonusCap.Value) / 100f));

            }
            

        }

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
                    yield return new CodeInstruction(OpCodes.Ldloc_1);//load i local variable
                    yield return new CodeInstruction(OpCodes.Ldarg_0);//load Pawns argument
                    yield return new CodeInstruction(OpCodes.Call, typeof(CaravanTicksPerMoveUtility_GetTicksPerMove).GetMethod("adjustTicksPerMove"));//Injected code     
                    yield return new CodeInstruction(OpCodes.Stloc_2);//load num2 local variable
                }
            }
        }

        public static int adjustTicksPerMove(int num2, int index, List<Pawn> pawns)
        {
            Pawn pawn = pawns[index];
            ExtendedDataStorage store = GiddyUpCore.Base.Instance.GetExtendedDataStorage();
            if (store == null)
            {
                return num2;
            }
            ExtendedPawnData pawnData = store.GetExtendedDataFor(pawn);
            if (pawnData.caravanMount != null && pawn.ridingCaravanMount())
            {
                return TicksPerMoveUtility.adjustedTicksPerMove(pawn, pawnData.caravanMount, true);
            }
            else
            {
                return num2;
            }
        }
    }
}
