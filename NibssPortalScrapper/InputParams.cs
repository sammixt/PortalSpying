using ConsoleCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NibssPortalScrapper
{
    class InputParams : ParamsObject
    {
        public InputParams(string[] args)
           : base(args)
        {

        }
        [Switch("U")]
        public string Username { get; set; }
        [Switch("P")]
        public string Password { get; set; }
        [Switch("URL")]
        public string WebUrl { get; set; }
        [Switch("D")]
        public string DownloadPath { get; set; }
        [Switch("CODE")]
        public string BankCode { get; set; }
        [Switch("SD")]
        public string StartDay { get; set; }
        [Switch("SM")]
        public string StartMonth { get; set; }
        [Switch("SY")]
        public string StartYear { get; set; }

        [Switch("ED")]
        public string EndDay { get; set; }
        [Switch("EM")]
        public string EndMonth { get; set; }
        [Switch("EY")]
        public string EndYear { get; set; }
        [Switch("HL")]
        public bool Headless { get; set; }
    }
}
