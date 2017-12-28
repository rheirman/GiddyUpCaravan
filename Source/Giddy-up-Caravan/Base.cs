using GiddyUpCore.Storage;
using HugsLib;
using HugsLib.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace GiddyUpCaravan
{
    public class Base : ModBase
    {
        internal static SettingHandle<int> completeCaravanBonus;
        internal static SettingHandle<int> incompleteCaravanBonusCap;

        public override string ModIdentifier
        {
            get { return "GiddyUpCaravan"; }
        }

        public static ExtendedDataStorage GetExtendedDataStorage()
        {
            return GiddyUpCore.Base.Instance.GetExtendedDataStorage();
        }
        public override void DefsLoaded()
        {
            completeCaravanBonus = Settings.GetHandle<int>("completeCaravanBonus", "BM_CompleteCaravanBonus_Title".Translate(), "BM_CompleteCaravanBonus_Description".Translate(), 80, Validators.IntRangeValidator(0, 200));
            incompleteCaravanBonusCap = Settings.GetHandle<int>("incompleteCaravanBonusCap", "BM_incompleteCaravanBonusCap_Title".Translate(), "BM_incompleteCaravanBonusCap_Description".Translate(), 25, Validators.IntRangeValidator(0, 200));
        }
    }


}
