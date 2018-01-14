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
            __result = Utilities.CaravanUtility.applySpeedBonus(__result, pawns);
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
            if(store == null)
            {
                return num2;
            }
            ExtendedPawnData pawnData = store.GetExtendedDataFor(pawn);
            if (pawnData != null && pawnData.caravanMount != null && pawn.ridingCaravanMount())
            {
                Log.Message("Returning adjusted ticks per move: " + "TicsPerMove was: " + num2 + ", but now: " + TicksPerMoveUtility.adjustedTicksPerMove(pawn, pawnData.caravanMount, true));
                return TicksPerMoveUtility.adjustedTicksPerMove(pawn, pawnData.caravanMount, true);
            }
            else
            {
                Log.Message("Returning default ticks per move: " + num2);
                return num2;
            }
        }
    }
}
