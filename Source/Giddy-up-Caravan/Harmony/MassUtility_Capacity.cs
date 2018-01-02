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
            ExtendedDataStorage store = GiddyUpCore.Base.Instance.GetExtendedDataStorage();
            if(store == null)//this check is needed because some mods call this function on world load 
            {
                return;
            }

            ExtendedPawnData pawnData = store.GetExtendedDataFor(p);
            if (pawnData == null || pawnData.caravanRider == null)
            {
                return;
            }
            else
            {
                __result -= pawnData.caravanRider.GetStatValue(StatDefOf.Mass);
                __result = Math.Max(__result, 0f);
            }
        }
    }
}
