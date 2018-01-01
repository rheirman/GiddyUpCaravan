using GiddyUpCaravan.Utilities;
using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace GiddyUpCaravan.Harmony
{
    [HarmonyPatch(typeof(IncidentWorker_TraderCaravanArrival), "TryExecuteWorker")]
    static class IncidentWorker_TraderCaravanArrival_TryExecuteWorker
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            for (var i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction instruction = instructionsList[i];
                yield return instruction;

                if (instructionsList[i].operand == AccessTools.Method(typeof(PawnRelationUtility), "Notify_PawnsSeenByPlayer_Letter")) //Identifier for which IL line to inject to

                {
                    yield return new CodeInstruction(OpCodes.Ldloc_1);//load generated pawns as parameter
                    yield return new CodeInstruction(OpCodes.Ldarg_1);//load incidentparms as parameter
                    yield return new CodeInstruction(OpCodes.Call, typeof(VisitorMountUtility).GetMethod("mountAnimals"));//Injected code                                                                                                                         //yield return new CodeInstruction(OpCodes.Stloc_2);
                }

            }

        }
    }
}
