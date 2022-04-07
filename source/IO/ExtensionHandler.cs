using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ChartTools.IO.Configuration;
using ChartTools.SystemExtensions;

namespace ChartTools.IO
{
    public delegate void VoidRead(string path);
    /// <summary>
    /// Read method that generates an object of the target type
    /// </summary>
    /// <typeparam name="T">Output type</typeparam>
    /// <param name="path">File path</param>
    /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
    public delegate T Read<T>(string path);
    /// <summary>
    /// Asynchronous read method that generates an object of the target type
    /// </summary>
    /// <typeparam name="T">Output type</typeparam>
    /// <param name="path">File path</param>
    /// <param name="cancellationToken"><inheritdoc cref="CancellationToken" path="/summary"/></param>
    /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
    public delegate Task<T> AsyncRead<T>(string path);
    /// <summary>
    /// Write method hat takes an object of a target type
    /// </summary>
    /// <typeparam name="T">Target type</typeparam>
    /// <param name="path">File path</param>
    /// <param name="content">Object to write</param>
    /// <param name="config"><inheritdoc cref="WritingConfiguration" path="/summary"/></param>
    public delegate void Write<T>(string path, T content);
    /// <summary>
    /// Write method hat takes an object of a target type
    /// </summary>
    /// <typeparam name="T">Target type</typeparam>
    /// <param name="path">File path</param>
    /// <param name="content">Object to write</param>
    /// <param name="cancellationToken"><inheritdoc cref="CancellationToken" path="/summary"/></param>
    /// <param name="config"><inheritdoc cref="WritingConfiguration" path="/summary"/></param>
    public delegate Task AsyncWrite<T>(string path, T content);

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
        public static void Read(string path, params (string extension, VoidRead readMetod)[] readers)
        {
            string extension = Path.GetExtension(path);
            (string extension, VoidRead readMethod) reader = readers.FirstOrDefault(r => r.extension == extension);

            if (reader == default)
                throw GetException(extension, readers.Select(r => r.extension));

            reader.readMethod(path);
        }
        /// <summary>
        /// Reads a file using the method that matches the extension and generates an output object.
        /// </summary>
        /// <typeparam name="T">Type of the generated object</typeparam>
        /// <param name="path">File path</param>
        /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
        /// <param name="readers">set of tuples containing the supported extensions and the matching read method</param>
        public static T Read<T>(string path, params (string extension, Read<T> readMethod)[] readers)
        {
            string extension = Path.GetExtension(path);
            (string extension, Read<T> readMethod) reader = readers.FirstOrDefault(r => r.extension == extension);

            return reader == default ? throw GetException(extension, readers.Select(r => r.extension)) : reader.readMethod(path);
        }
        public static async Task<T> ReadAsync<T>(string path, params (string extension, AsyncRead<T> readMethod)[] readers)
        {
            string extension = Path.GetExtension(path);
            (string extension, AsyncRead<T> readMethod) reader = readers.FirstOrDefault(r => r.extension == extension);

            return reader == default ? throw GetException(extension, readers.Select(r => r.extension)) : await reader.readMethod(path);
        }
        #endregion

        #region Writing
        /// <summary>
        /// Writes an object to a file using the method that matches the extension.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="content">Item to write</param>
        /// <param name="writers">Array of tupples representing the supported extensions</param>
        /// <exception cref="ArgumentNullException"/>
        public static void Write<T>(string path, T content, params (string extension, Write<T> writeMethod)[] writers)
        {
            string extension = Path.GetExtension(path);
            (string extension, Write<T> writeMethod) writer = writers.FirstOrDefault(w => w.extension == extension);

            if (writer == default)
                throw GetException(extension, writers.Select(w => w.extension));

            writer.writeMethod(path, content);
        }
        public static async Task WriteAsync<T>(string path, T content, params (string extension, AsyncWrite<T> writeMethod)[] writers)
        {
            string extension = Path.GetExtension(path);
            (string extension, AsyncWrite<T> writeMethod) writer = writers.FirstOrDefault(w => w.extension == extension);

            if (writer == default)
                throw GetException(extension, writers.Select(w => w.extension));

            await writer.writeMethod(path, content);
        }
        #endregion

        /// <summary>
        /// Gets the exception to throw if the extension has no method that handles it.
        /// </summary>
        /// <returns>Instance of <see cref="Exception"/> to throw</returns>
        private static Exception GetException(string extension, IEnumerable<string> supportedExtensions) => new ArgumentException($"\"{extension}\" is not a supported extension. File must be {supportedExtensions.VerbalEnumerate("or")}.");
    }
}
