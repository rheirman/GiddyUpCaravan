﻿using GiddyUpCaravan.Utilities;
using GiddyUpCore.Utilities;
using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace GiddyUpCaravan.Harmony
{
    [HarmonyPatch(typeof(IncidentWorker_TravelerGroup), "TryExecuteWorker")]
    static class IncidentWorker_TravelerGroup_TryExecuteWorker
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            for (var i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction instruction = instructionsList[i];
                if (instruction.operand == AccessTools.Method(typeof(IncidentWorker_NeutralGroup), "SpawnPawns")) //Identifier for which IL line to inject to

                {
                    yield return new CodeInstruction(OpCodes.Call, typeof(IncidentWorker_TravelerGroup_TryExecuteWorker).GetMethod("MountAnimals"));//Injected code                                                                                                                         //yield return new CodeInstruction(OpCodes.Stloc_2);
                }
                else
                {
                    yield return instruction;
                }

            }
        }
        public static List<Pawn> MountAnimals(IncidentWorker_TravelerGroup instance, IncidentParms parms)
        {
            List<Pawn> pawns = Traverse.Create(instance).Method("SpawnPawns", new object[] { parms }).GetValue<List<Pawn>>();
            if (!pawns.NullOrEmpty())
            {
                NPCMountUtility.generateMounts(ref pawns, parms, Base.inBiomeWeight, Base.outBiomeWeight, Base.nonWildWeight, Base.visitorMountChance, Base.visitorMountChanceTribal);
            }
            return pawns;
        }
    }
}
