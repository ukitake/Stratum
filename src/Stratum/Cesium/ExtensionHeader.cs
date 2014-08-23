using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.Cesium
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ExtensionHeader
    {
        public byte extensionId;
        public uint extensionLength;

        public ExtensionHeader(FastBinaryReader reader)
        {
            this.extensionId = reader.ReadByte();
            this.extensionLength = reader.ReadUInt32();
        }
    }
}
