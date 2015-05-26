using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.WindowsAzure.ServiceRuntime;
using XSockets.Core.Common.Configuration;
using XSockets.Core.Common.Socket;
using XSockets.Core.Common.Utility.Logging;
using XSockets.Core.Configuration;
using XSockets.Plugin.Framework;

namespace CC2650.XSocketsWorker
{
    /// <summary>
    /// Configures XSockets to run on all endpoints setup on the WorkerRole
    /// </summary>
    public static class Configuration
    {
        public static void StartOnAzure(this IXSocketServerContainer container)
        {
            //Configurations
            var configs = new List<IConfigurationSetting>();
            var uriStr = RoleEnvironment.GetConfigurationSettingValue("uri");
            var origins = new HashSet<string>(RoleEnvironment.GetConfigurationSettingValue("origin").Split(',').ToList());
            var instanceEndpoints =
                RoleEnvironment.CurrentRoleInstance.InstanceEndpoints.Values.Where(p => p.Protocol.Equals("tcp"));

            //Create endpoints
            foreach (var endpoint in instanceEndpoints)
            {
                var uri = new Uri(uriStr.Replace("port", endpoint.IPEndpoint.Port.ToString(CultureInfo.InvariantCulture)));
                configs.Add(new ConfigurationSetting(uri, origins) { Endpoint = endpoint.IPEndpoint });
                Composable.GetExport<IXLogger>().Information("Endpoint {@endpoint}",endpoint);
            }

            if (!configs.Any())
            {
                Composable.GetExport<IXLogger>().Fatal("Could not find a TCP endpoint, check your configuration");
                return;
            }

            //Start server with endpoints
            container.Start(false, configurationSettings: configs);
        }
    }
}