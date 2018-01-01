using GiddyUpCore.Storage;
using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace GiddyUpCaravan.Harmony
{
    [HarmonyPatch(typeof(MassUtility), "Capacity")]
    static class MassUtility_Capacity
    {
        static void Postfix(ref Pawn p, ref float __result)
        {
            ExtendedPawnData pawnData = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(p);
            if (pawnData.caravanRider == null)
            {
                return;
            }
            else
            {
                Log.Message("caravanRider not null, change result, before: " + __result);
                __result -= pawnData.caravanRider.GetStatValue(StatDefOf.Mass);
                __result = Math.Max(__result, 0f);
                Log.Message("caravanRider not null, change result, after: " + __result);

            }
        }
    }
}
