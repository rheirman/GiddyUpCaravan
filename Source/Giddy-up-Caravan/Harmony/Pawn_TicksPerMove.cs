using GiddyUpCaravan.Utilities;
using GiddyUpCore.Storage;
using GiddyUpCore.Utilities;
using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace GiddyUpCaravan.Harmony
{
    [HarmonyPatch(typeof(Pawn), "TicksPerMove")]
    static class Pawn_TicksPerMove
    {
        [HarmonyPriority(Priority.Low)]
        static void Postfix(Pawn __instance, ref bool diagonal, ref int __result)
        {

            if (GiddyUpCore.Base.Instance.GetExtendedDataStorage() == null)
            {
                return;
            }

            ExtendedPawnData pawnData = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(__instance);
            if (pawnData.caravanMount != null && __instance.ridingCaravanMount() && !__instance.Spawned)
            {
                __result = TicksPerMoveUtility.adjustedTicksPerMove(__instance, pawnData.caravanMount, diagonal);
            }
        }
    }
}
