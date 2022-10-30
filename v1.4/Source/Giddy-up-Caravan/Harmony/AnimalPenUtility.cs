using GiddyUpCore.Storage;
using HarmonyLib;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace GiddyUpCaravan.Harmony
{
    [HarmonyPatch(typeof(AnimalPenUtility), "NeedsToBeManagedByRope")]
    class AnimalPenUtility_NeedsToBeManagedByRope
    {
        static void Postfix(Pawn pawn, ref bool __result)
        {
            if (__result && pawn.IsFormingCaravan())
            {
                ExtendedDataStorage store = GiddyUpCore.Base.Instance.GetExtendedDataStorage();
                var caravanRider = store.GetExtendedDataFor(pawn).caravanRider;
                __result = caravanRider == null;
                return;
            }
        }
    }
}
