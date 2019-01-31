using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace OrchardCore.Environment.Shell.Configuration
{
    public class ShellConfigurationSources : IShellConfigurationSources
    {
        private readonly string _container;

        public ShellConfigurationSources(IHostingEnvironment hostingEnvironment, IOptions<ShellOptions> shellOptions)
        {
            // e.g., App_Data/Sites
            _container = Path.Combine(shellOptions.Value.ShellsApplicationDataPath, shellOptions.Value.ShellsContainerName);
            Directory.CreateDirectory(_container);
        }

        public void AddSources(string tenant, IConfigurationBuilder builder)
        {
            builder
                .AddJsonFile(Path.Combine(_container, tenant, "appsettings.json"), optional: true);
        }

        public void Save(string tenant, IDictionary<string, string> data)
        {
            lock (this)
            {
                var tenantFolder = BuildTenantFolderName(tenant);
                var appsettings = Path.Combine(tenantFolder, "appsettings.json");

                var config = !File.Exists(appsettings) ? new JObject()
                    : JObject.Parse(File.ReadAllText(appsettings));

                foreach (var key in data.Keys)
                {
                    if (data[key] != null)
                    {
                        config[key] = data[key];
                    }
                    else
                    {
                        config.Remove(key);
                    }
                }

                Directory.CreateDirectory(tenantFolder);
                File.WriteAllText(appsettings, config.ToString());
            }
        }

        public void Delete(string tenant)
        {
            lock (this)
            {
                var appsettings = Path.Combine(BuildTenantFolderName(tenant), "appsettings.json");

                File.Delete(appsettings);
            }
        }

        private string BuildTenantFolderName(string tenant) =>
            Path.Combine(_container, tenant);
    }
}