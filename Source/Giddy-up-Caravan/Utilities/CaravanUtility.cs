using GiddyUpCore.Storage;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace GiddyUpCaravan.Utilities
{
    static class CaravanUtility
    {
        public static bool ridingCaravanMount(this Pawn pawn)
        {
            ExtendedPawnData pawndata = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(pawn);
            if (pawn.IsColonist && pawndata != null && pawndata.caravanMount != null)
            {
                return true;

            }
            return false;
        }
    }
}
