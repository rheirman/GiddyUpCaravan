using GiddyUpCore.Storage;
using HugsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GiddyUpCaravan
{
    public class Base : ModBase
    {
        public override string ModIdentifier
        {
            get { return "GiddyUpCaravan"; }
        }
        public static ExtendedDataStorage GetExtendedDataStorage()
        {
            return GiddyUpCore.Base.Instance.GetExtendedDataStorage();
        }

    }


}
