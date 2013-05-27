using System;
using System.IO;

namespace dff.Extensions
{
    public static class FileInfoExtensions
    {
        /// <summary>
        /// Umbenennen einer Datei (incl. Dateityp).
        /// </summary>
        public static FileInfo Rename(this FileInfo file, string newName)
        {
            var filePath = Path.Combine(Path.GetDirectoryName(file.FullName), newName);
            file.MoveTo(filePath);
            return file;
        }

        /// <summary>
        /// Umbenennen einer Datei (ohne Dateityp).
        /// </summary>
        public static FileInfo RenameFileWithoutExtension(this FileInfo file, string newName)
        {
            var fileName = string.Concat(newName, file.Extension);
            file.Rename(fileName);
            return file;
        }

        /// <summary>
        /// Ändert den Dateitypen
        /// </summary>
        public static FileInfo ChangeExtension(this FileInfo file, string newExtension)
        {
            if (newExtension.StartsWith("."))
            {
                var fileName = string.Concat(Path.GetFileNameWithoutExtension(file.FullName), newExtension);
                file.Rename(fileName);
                return file;
            }
            throw new ArgumentException("The new Extension must start with '.'");
        }
    }
}