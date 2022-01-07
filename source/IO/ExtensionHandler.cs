using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ChartTools.SystemExtensions;

namespace ChartTools.IO
{
    public delegate T Read<T>(string path, ReadingConfiguration? config);
    public delegate Task<T> AsyncRead<T>(string path, CancellationToken cancellationToken, ReadingConfiguration? config);
    public delegate void Write<T>(string path, T content, WritingConfiguration? config);
    public delegate Task AsyncWrite<T>(string path, T content, CancellationToken cancellationToken, WritingConfiguration? config);

    /// <summary>
    /// Provides methods for reading and writing files based on the extension
    /// </summary>
    internal static class ExtensionHandler
    {
        #region Reading
        /// <summary>
        /// Reads a file using the method that matches the extension.
        /// </summary>
        /// <param name="path">Path of the file to read</param>
        /// <param name="readers">Array of tuples representing the supported extensions</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FileNotFoundException"/>
        public static void Read(string path, params (string extension, Action<string> readMetod)[] readers)
        {
            string extension = Path.GetExtension(path);
            (string extension, Action<string> readMethod) reader = readers.FirstOrDefault(r => r.extension == extension);

            if (reader == default)
                throw GetException(extension, readers.Select(r => r.extension));

            reader.readMethod(path);
        }

        /// <inheritdoc cref="Read{T}(string, ValueTuple{string, Func{string, T}}[])"/>
        public static T Read<T>(string path, ReadingConfiguration? config, params (string extension, Read<T> readMethod)[] readers)
        {
            string extension = Path.GetExtension(path);
            (string extension, Read<T> readMethod) reader = readers.FirstOrDefault(r => r.extension == extension);

            return reader == default ? throw GetException(extension, readers.Select(r => r.extension)) : reader.readMethod(path, config);
        }
        public static async Task<T> ReadAsync<T>(string path, CancellationToken cancellationToken, ReadingConfiguration? config, params (string extension, AsyncRead<T> readMethod)[] readers)
        {
            string extension = Path.GetExtension(path);
            (string extension, AsyncRead<T> readMethod) reader = readers.FirstOrDefault(r => r.extension == extension);

            return reader == default ? throw GetException(extension, readers.Select(r => r.extension)) : await reader.readMethod(path, cancellationToken, config);
        }
        #endregion

        #region Writing
        public static void Write(string path, params (string extension, Action<string> writeMethod)[] writers)
        {
            string extension = Path.GetExtension(path);
            (string extension, Action<string> writeMethod) writer = writers.FirstOrDefault(w => w.extension == extension);

            if (writer == default)
                throw GetException(extension, writers.Select(w => w.extension));

            writer.writeMethod(path);
        }
        /// <summary>
        /// Writes an object to a file using the method that matches the extension.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="content">Item to write</param>
        /// <param name="writers">Array of tupples representing the supported extensions</param>
        /// <exception cref="ArgumentNullException"/>
        public static void Write<T>(string path, T content, WritingConfiguration? config, params (string extension, Write<T> writeMethod)[] writers)
        {
            string extension = Path.GetExtension(path);
            (string extension, Write<T> writeMethod) writer = writers.FirstOrDefault(w => w.extension == extension);

            if (writer == default)
                throw GetException(extension, writers.Select(w => w.extension));

            writer.writeMethod(path, content, config);
        }
        public static async Task WriteAsync<T>(string path, T content, CancellationToken cancellationToken, WritingConfiguration? config, params (string extension, AsyncWrite<T> writeMethod)[] writers)
        {
            string extension = Path.GetExtension(path);
            (string extension, AsyncWrite<T> writeMethod) writer = writers.FirstOrDefault(w => w.extension == extension);

            if (writer == default)
                throw GetException(extension, writers.Select(w => w.extension));

            await writer.writeMethod(path, content, cancellationToken, config);
        }
        #endregion

        /// <summary>
        /// Gets the exception to throw if the extension has no method that handles it.
        /// </summary>
        /// <returns>Instance of <see cref="Exception"/> to throw</returns>
        private static Exception GetException(string extension, IEnumerable<string> supportedExtensions) => new ArgumentException($"\"{extension}\" is not a supported extension. File must be {supportedExtensions.VerbalEnumerate("or")}.");
    }
}
