﻿using ConsoleCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIbssFileFilteringV2
{
    public class InputParams:ParamsObject
    {
        public InputParams(string[] args)
            : base(args)
        {

        }

        [Switch("FP")]
        public string FullPath { get; set; }
        [Switch("ST")]
        public string SettlementType { get; set; }
        [Switch("SS")]
        public string Session { get; set; }
    }
}
