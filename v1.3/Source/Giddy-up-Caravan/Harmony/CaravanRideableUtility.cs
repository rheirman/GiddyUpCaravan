using GiddyUpCaravan.Utilities;
using GiddyUpCore.Storage;
using GiddyUpCore.Utilities;
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
    [HarmonyPatch(typeof(CaravanRideableUtility), "IsCaravanRideable", new Type[] {typeof(Pawn)})]
    class CaravanRideableUtility_IsCaravanRideable
    {
        static bool Prefix(Pawn pawn, ref bool __result)
        {
            if(IsMountableUtility.isMountable(pawn))
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(CaravanRideableUtility), "IsCaravanRideable", new Type[] {typeof(ThingDef)})]
    class CaravanRideableUtility_IsCaravanRideable2
    {
        static bool Prefix(ThingDef def, ref bool __result)
        {
            if (IsMountableUtility.IsMountable(def))
            {
                __result = true;
                return false;
            }
            return true;
        }
    }

}
