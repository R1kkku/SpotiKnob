using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace SpotiKnob
{
    internal static class AssetStore
    {
        private static readonly Assembly Assembly = typeof(AssetStore).Assembly;
        private static readonly Dictionary<string, string> ExtractedFiles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static readonly object SyncRoot = new object();

        public static byte[] GetBytes(string relativePath)
        {
            using (Stream stream = OpenStream(relativePath))
            {
                if (stream == null)
                {
                    return null;
                }

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }

        public static Bitmap LoadBitmap(string relativePath)
        {
            byte[] bytes = GetBytes(relativePath);
            if (bytes == null)
            {
                return null;
            }

            using (MemoryStream memoryStream = new MemoryStream(bytes))
            using (Image image = Image.FromStream(memoryStream))
            {
                return new Bitmap(image);
            }
        }

        public static string ExtractTempFile(string relativePath)
        {
            lock (SyncRoot)
            {
                string existingPath;
                if (ExtractedFiles.TryGetValue(relativePath, out existingPath) && File.Exists(existingPath))
                {
                    return existingPath;
                }

                byte[] bytes = GetBytes(relativePath);
                if (bytes == null)
                {
                    return null;
                }

                string extension = Path.GetExtension(relativePath);
                string filePath = Path.Combine(
                    Path.GetTempPath(),
                    "SpotiKnob",
                    "assets",
                    relativePath.Replace('\\', '_').Replace('/', '_'));

                string directoryPath = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                if (string.IsNullOrEmpty(extension))
                {
                    filePath += ".bin";
                }

                File.WriteAllBytes(filePath, bytes);
                ExtractedFiles[relativePath] = filePath;
                return filePath;
            }
        }

        private static Stream OpenStream(string relativePath)
        {
            string resourceName = "SpotiKnob." + relativePath.Replace('\\', '.').Replace('/', '.');
            return Assembly.GetManifestResourceStream(resourceName);
        }
    }
}
