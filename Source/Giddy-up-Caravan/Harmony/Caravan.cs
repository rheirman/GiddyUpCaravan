using Harmony;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GiddyUpCaravan.Harmony
{
    /*
    [HarmonyPatch(typeof(Caravan), "GetInspectString")]
    static class Caravan_GetInspectString
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            for (var i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction instruction = instructionsList[i];
                if (instructionsList[i].operand == typeof(CaravanArrivalTimeEstimator).GetMethod("EstimatedTicksToArrive"))
                {

                }
                else
                {
                    yield return instruction;
                }

            }
        }
    }
    */
}
