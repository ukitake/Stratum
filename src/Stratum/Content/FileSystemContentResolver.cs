using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using SharpDX.Toolkit.Content;

namespace Stratum.Content
{
    public class FileSystemContentResolver : IContentResolver
    {
        public bool Exists(string assetName)
        {
            string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fullPath = Path.Combine(baseDir, assetName);

            return File.Exists(fullPath);
        }

        public Stream Resolve(string assetName)
        {
            string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fullPath = Path.Combine(baseDir, assetName);

            FileStream stream = new FileStream(fullPath, FileMode.Open);
            return stream;
        }
    }
}
