using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NibssPortalScrapper
{
    public static class Extensions
    {
        public static string GetKeyValue(this string Key)
        {
            return ConfigurationManager.AppSettings[Key];
        }
    }
}
