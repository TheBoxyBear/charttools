using System;
using System.IO;
using System.Linq;

namespace ChartTools.IO
{
    /// <summary>
    /// Provides methods for rading and writing files based on the extension
    /// </summary>
    internal static class ExtensionHandler
    {
        /// <summary>
        /// Reads a file using the method that matches the extension.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        internal static void Read(string path, params (string extension, Action<string> readMethod)[] readers)
        {
            string extension = Path.GetExtension(path);
            (string extension, Action<string> readMethod) reader = readers.FirstOrDefault(r => r.extension == extension);

            if (reader == default)
                throw GetException();

            reader.readMethod(path);
        }
        /// <summary>
        /// Reads a file using the method that matches the extension.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        internal static T Read<T>(string path, params (string extension, Func<string, T> readMethod)[] readers)
        {
            string extension = Path.GetExtension(path);
            (string extension, Func<string, T> readMethod) reader = readers.FirstOrDefault(r => r.extension == extension);

            return reader == default ? throw GetException() : reader.readMethod(path);
        }

        /// <summary>
        /// Writes an object to a file using the method that matches the extension.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        internal static void Write<T>(string path, T item, params (string extension, Action<string, T> writeMethod)[] writers)
        {
            string extension = Path.GetExtension(path);
            (string extension, Action<string, T> writeMethod) writer = writers.FirstOrDefault(w => w.extension == extension);

            if (writer == default)
                throw GetException();

            writer.writeMethod(path, item);
        }

        /// <summary>
        /// Gets the exception to throw if the extension has no method that handles it.
        /// </summary>
        /// <returns>Instance of <see cref="Exception"/> to throw</returns>
        private static Exception GetException() => new ArgumentException("File format not supported.");
    }
}
