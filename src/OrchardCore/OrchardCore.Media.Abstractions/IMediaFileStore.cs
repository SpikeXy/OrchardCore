using System.IO;
using System.Threading.Tasks;
using OrchardCore.FileStorage;

namespace OrchardCore.Media
{
    /// <summary>
    /// Represents an abstraction over a specialized file store for storing media and service it to clients.
    /// </summary>
    public interface IMediaFileStore : IFileStore
    {
        /// <summary>
        /// Maps a path within the file store to a publicly accessible URL.
        /// </summary>
        /// <param name="path">The path within the file store.</param>
        /// <returns>A string containing the mapped public URL of the given path.</returns>
        string MapPathToPublicUrl(string path);

        /// <summary>
        /// Maps a public URL to a path within the file store.
        /// </summary>
        /// <param name="publicUrl">The public URL to map.</param>
        /// <returns>The mapped path of the given public URL within the file store.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the specified <paramref name="publicUrl"/> value is not within the publicly accessible URL space of the file store.</exception>
        string MapPublicUrlToPath(string publicUrl);

        /// <summary>
        /// Creates a stream to read the contents of a file in the file store.
        /// </summary>
        /// <param name="path">The path of the file to be read.</param>
        /// <returns>An instance of <see cref="System.IO.Stream"/> that can be used to read the contents of the file. The caller must close the stream when finished.</returns>
        /// <exception cref="FileStoreException">Thrown if the specified file does not exist.</exception>
        Task<Stream> RotateImageAsync(string path, int angle);
    }
}
