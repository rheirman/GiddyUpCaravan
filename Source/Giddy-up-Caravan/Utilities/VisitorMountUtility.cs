using GiddyUpCore.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace GiddyUpCaravan.Utilities
{
    class VisitorMountUtility
    {
        public static void mountAnimals(ref List<Pawn> list, IncidentParms parms)
        {
            if (!list.NullOrEmpty())
            {
                NPCMountUtility.generateMounts(ref list, parms, Base.inBiomeWeight, Base.outBiomeWeight, Base.nonWildWeight, Base.visitorMountChance, Base.visitorMountChanceTribal);
            }
        }
    }
}
