using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using GiddyUpCore.Zones;

namespace GiddyUpCaravan.Zones
{
    class Designator_GU_DropAnimal_NPC_Expand : Designator_GU
    {

        public Designator_GU_DropAnimal_NPC_Expand() : base(DesignateMode.Add)
        {
            defaultLabel = "GU_Car_Designator_GU_DropAnimal_NPC_Expand_Label".Translate();
            defaultDesc = "GU_Car_Designator_GU_DropAnimal_NPC_Expand_Description".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/GU_Car_Designator_GU_DropAnimal_NPC_Expand", true);
            areaLabel = Base.DropAnimal_NPC_LABEL;
        }



        //public override AcceptanceReport CanDesignateCell(IntVec3 c)
        //{
        //    return c.InBounds(base.Map) && Designator_Stable.SelectedArea != null && Designator_Stable.SelectedArea[c];
        //}
        public override void DesignateSingleCell(IntVec3 c)
        {
            selectedArea[c] = true;
        }
        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            return c.InBounds(base.Map) && selectedArea != null && !selectedArea[c];
        }


    }
}
