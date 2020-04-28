using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Onion.WebApp.Utils
{
    public class ConfigUtil
    {
        public static string GetConfigValue(string key)
        {
            return ConfigurationManager.AppSettings.Get(key);
        }
    }
}