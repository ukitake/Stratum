using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using SharpDX.Toolkit.Graphics;

namespace Stratum.Graphics
{
    public class FontLoader
    {
        public static SpriteFont Load(string filePathAndName)
        {
            string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fullPath = Path.Combine(baseDir, filePathAndName);

            if (!File.Exists(fullPath))
            {
                Debugger.Break();
            }

            string compiledfile = string.Format("{0}comp", Path.GetFileNameWithoutExtension(fullPath));
            var result = FontCompiler.CompileAndSave(fullPath, compiledfile);

            if (result.HasErrors)
            {
                Debugger.Break();
            }

            return SpriteFont.New(Engine.GraphicsContext.Device, SpriteFontData.Load(compiledfile));
        }
    }
}
