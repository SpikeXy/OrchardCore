using System.Threading.Tasks;
using OrchardCore.Environment.Shell;

namespace OrchardCore.Media
{
    public class DeleteMediaShellEventHandler : IShellEventHandler
    {
        private readonly IMediaFileStore _mediaFileStore;

        public DeleteMediaShellEventHandler(IMediaFileStore mediaFileStore)
        {
            _mediaFileStore = mediaFileStore;
        }

        public async Task Removing(ShellSettings shellSettings)
        {
            var success = await _mediaFileStore.TryDeleteDirectoryAsync("/");
        }
    }
}