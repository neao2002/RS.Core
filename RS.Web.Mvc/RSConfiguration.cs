using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using RS.Core;

namespace RS.Web.Mvc
{
    internal class RSConfiguration : IRSConfiguration
    {
        public IConfiguration configuration;
        public RSConfiguration(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public string GetAppSetting(string Key)
        {
            if (configuration == null)
                return "";
            else
                return configuration[Key].ToStringValue();

        }

        public string GetConnectionString(string Name)
        {
            if (configuration == null)
                return "";
            else
                return configuration.GetConnectionString(Name).ToStringValue();
        }

        public static IRSConfiguration ToRSConfiguration(IConfiguration config)
        {
            return new RSConfiguration(config);
        }
        
    }
}