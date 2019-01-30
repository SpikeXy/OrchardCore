using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace OrchardCore.Environment.Shell.Configuration
{
    public class ShellsSettingsSources : IShellsSettingsSources
    {
        private readonly string _tenants;

        public ShellsSettingsSources(IOptions<ShellOptions> shellOptions)
        {
            _tenants = Path.Combine(shellOptions.Value.ShellsApplicationDataPath, "tenants.json");
        }

        public void AddSources(IConfigurationBuilder builder)
        {
            builder.AddJsonFile(_tenants, optional: true);
        }

        public void Save(string tenant, IDictionary<string, string> data)
        {
            lock (this)
            {
                var settings = GetSettings();

                var tenantSettings = settings.GetValue(tenant) as JObject ?? new JObject();

                foreach (var key in data.Keys)
                {
                    if (data[key] != null)
                    {
                        tenantSettings[key] = data[key];
                    }
                    else
                    {
                        tenantSettings.Remove(key);
                    }
                }

                settings[tenant] = tenantSettings;
                File.WriteAllText(_tenants, settings.ToString());
            }
        }

        public void Delete(string tenant)
        {
            lock (this)
            {
                var settings = GetSettings();

                if (settings.GetValue(tenant) == null) return;

                settings.Remove(tenant);
                File.WriteAllText(_tenants, settings.ToString());
            }
        }

        private JObject GetSettings() =>
            !File.Exists(_tenants) ? new JObject() : JObject.Parse(File.ReadAllText(_tenants));
    }
}