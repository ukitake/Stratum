using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.Cesium
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NormalExtensionData
    {
        public uint vertexCount;
        public byte[] xy;

        public NormalExtensionData(FastBinaryReader reader, uint vertCount)
        {
            this.vertexCount = vertCount;
            this.xy = new byte[vertexCount * 2];

            for (int i = 0; i < vertexCount*2; i++)
                xy[i] = reader.ReadByte();
        }
    }
}
