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
    // todo: Effects should be loaded through the content manager
    public static class EffectLoader
    {
        public static Effect Load(string filePathAndName)
        {
            string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fullPath = Path.Combine(baseDir, filePathAndName);

            if (!File.Exists(fullPath))
            {
                Debugger.Break();
            }

            EffectCompiler compiler = new EffectCompiler();
            var result = compiler.CompileFromFile(fullPath, EffectCompilerFlags.Debug);
            
            if (result.HasErrors)
            {
                var thing = result.Logger.Messages;
                Debugger.Break();
                return null;
            }
            else
                return new Effect(Engine.GraphicsContext.Device, result.EffectData);
        }
    }
}
