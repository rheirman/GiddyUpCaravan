using Harmony;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace GiddyUpCaravan.Harmony
{
    [HarmonyPatch(typeof(WorldRoutePlanner), "RecreatePaths")]
    static class WorldRoutePlanner_RecreatePaths
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            for (var i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction instruction = instructionsList[i];

                if(instructionsList[i].opcode == OpCodes.Ldloc_3)
                {
                    Log.Message("replacing ldloc_3");
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(WorldRoutePlanner), "get_CaravanPawns")); //this replaces get_CaravanTicksPerMove with a reference to the Caravan, which is needed in the modified EstimatedTicksToArrive
                    continue;
                }
                

                if (instructionsList[i].operand == typeof(CaravanArrivalTimeEstimator).GetMethod("EstimatedTicksToArrive", new Type[] { typeof(int), typeof(int), typeof(WorldPath), typeof(float), typeof(int), typeof(int) }))
                {
                  
                    yield return new CodeInstruction(OpCodes.Call, typeof(Utilities.CaravanUtility).GetMethod("EstimatedTicksToArrive", new Type[] { typeof(int), typeof(int), typeof(WorldPath), typeof(float), typeof(List<Pawn>), typeof(int) }));//Injected code     
                    continue;
                    //yield return instruction;
                }
                yield return instruction;
            }
        }
    }
}
