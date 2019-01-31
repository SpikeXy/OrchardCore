using OrchardCore.Environment.Shell.Descriptor.Models;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell
{
    /// <summary>
    /// 
    /// </summary>
    public interface IShellEventHandler
    {
        /// <summary>
        /// 
        /// </summary>
        Task Removing(ShellSettings shellSettings);
    }
}
