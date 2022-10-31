using GiddyUpCore;
using GiddyUpCore.Utilities;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace GiddyUpCaravan.Harmony
{

    [HarmonyPatch(typeof(TransferableUtility), "TransferAsOne")]
    static class TransferableUtility_TransferAsOne
    {
        static void Postfix(Thing a, Thing b, ref bool __result)
        {
            if (__result == true && a.def.category == ThingCategory.Pawn && b.def.category == ThingCategory.Pawn)
            {
                Pawn pawnA = (Pawn)a;
                Pawn pawnB = (Pawn)b;
                if(IsMountableUtility.isMountable(pawnA) || IsMountableUtility.isMountable(pawnB)){
                    __result = false;
                }
            }
        }


    }
}
