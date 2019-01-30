using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OrchardCore.Environment.Shell;

namespace OrchardCore.Tenants.Events
{
    public interface ITenantEventHandler
    {
        Task CreatingAsync(ShellSettings shellSettings);
    }
}
