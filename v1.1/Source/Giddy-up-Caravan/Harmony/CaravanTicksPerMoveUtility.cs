using GiddyUpCaravan.Utilities;
using GiddyUpCore.Storage;
using GiddyUpCore.Utilities;
using HarmonyLib;
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
    
    [HarmonyPatch(new Type[] { typeof(List<Pawn>), typeof(float), typeof(float), typeof(StringBuilder) })]
    [HarmonyPatch(typeof(CaravanTicksPerMoveUtility), "GetTicksPerMove")]
    class CaravanTicksPerMoveUtility_GetTicksPerMove
    {
        //We need to save the caravan pawns here, they are needed when Caravan_PathFollower.CostToMove is called, but the list is unreachable there. Hence we make it reachable by saving it in Base. 
        static void Prefix(List<Pawn> pawns, ref int __result)
        {
            //Base.Instance.curCaravanPawns = pawns;
            Base.Instance.curCaravanPawns = new List<Pawn>();
            foreach(Pawn pawn in pawns)
            {
                Base.Instance.curCaravanPawns.Add(pawn);
            }
            //__result = Mathf.RoundToInt(Utilities.CaravanUtility.applySpeedBonus(__result, pawns)); //apply static speed bonus as defined in the options. 
        }

        // Ensures adjustTicksPermove is called, which makes sure the ticks per move of the animals being mounted are used, and not the ticks per move of the riders themselves 
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            bool flag = false;
            for (var i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction instruction = instructionsList[i];
                yield return instruction;
                if (instructionsList[i].operand == typeof(Pawn).GetMethod("get_TicksPerMoveCardinal"))
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
                    flag = false;
                }
            }
        }

        //makes sure the ticks per move of the animals being mounted are used, and not the ticks per move of the riders themselves 
        public static float adjustTicksPerMove(float num2, int index, List<Pawn> pawns)
        {
            ExtendedDataStorage store = GiddyUpCore.Base.Instance.GetExtendedDataStorage();
            if(store == null || pawns == null || pawns.Count == 0 )
            {
                return num2;
            }
            Pawn pawn = pawns[index];

            ExtendedPawnData pawnData = store.GetExtendedDataFor(pawn);
            if (pawnData != null && pawnData.caravanMount != null && pawn.ridingCaravanMount())
            {
                //Log.Message("Returning adjusted ticks per move: " + "TicsPerMove was: " + num2 + ", but now: " + TicksPerMoveUtility.adjustedTicksPerMove(pawn, pawnData.caravanMount, true));
                return TicksPerMoveUtility.adjustedTicksPerMove(pawn, pawnData.caravanMount, true);
            }
            else
            {
                //Log.Message("Returning default ticks per move: " + num2);
                return num2;
            }
        }
    }
    
}
