using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace FluksoCore
{
    public class FluksoServer
    {
        public string FluksoIP { get; set; }
    }

    public class FluksoConfig
    {
        public static IConfiguration _config =  new ConfigurationBuilder()
            #if DEBUG
            .AddJsonFile($"appsettings.json", false, true)
            #endif
            .AddEnvironmentVariables()
            .Build();
        public static string getConfig(string configName)
        {
            return _config[configName];
        }

        /*
        public static List<string> getConfigArray(string configName)
        {
            List<string> configItems = new List<string>();
            IConfigurationSection configArray = _config.GetSection(configName);
            foreach (var c in configArray.GetChildren())
            {
                configItems.Add(c.Value);
            }

            return configItems;
        }
        */
    }
}