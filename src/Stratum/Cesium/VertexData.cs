using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.Cesium
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct VertexData
    {
        public uint vertexCount;
        public ushort[] u;
        public ushort[] v;
        public ushort[] height;

        public VertexData(FastBinaryReader reader)
        {
            vertexCount = reader.ReadUInt32();
            u = new ushort[vertexCount];
            v = new ushort[vertexCount];
            height = new ushort[vertexCount];

            if (vertexCount > 64 * 1024)
                throw new NotSupportedException("32 bit indices not supported yet");

            //fixed (ushort* uptr = u)
            //{
            //    SharpDX.Utilities.CopyMemory(new IntPtr(uptr), new IntPtr(reader.FixedPtr), (int)vertexCount * sizeof(ushort));
            //    reader.AdvanceBytes(vertexCount * sizeof(ushort));
            //}
            for (int i = 0; i < vertexCount; i++)
                u[i] = reader.ReadUInt16();

            //fixed (ushort* vptr = v)
            //{
            //    SharpDX.Utilities.CopyMemory(new IntPtr(vptr), new IntPtr(reader.FixedPtr), (int)vertexCount * sizeof(ushort));
            //    reader.AdvanceBytes(vertexCount * sizeof(ushort));
            //}
            for (int i = 0; i < vertexCount; i++)
                v[i] = reader.ReadUInt16();

            //fixed (ushort* hptr = height)
            //{
            //    SharpDX.Utilities.CopyMemory(new IntPtr(hptr), new IntPtr(reader.FixedPtr), (int)vertexCount * sizeof(ushort));
            //    reader.AdvanceBytes(vertexCount * sizeof(ushort));
            //}
            for (int i = 0; i < vertexCount; i++)
                height[i] = reader.ReadUInt16();

            ushort _u = 0;
            ushort _v = 0;
            ushort _height = 0;

            for (int i = 0; i < vertexCount; i++)
            {
                _u += zigZagDecode(u[i]);
                _v += zigZagDecode(v[i]);
                _height += zigZagDecode(height[i]);

                u[i] = _u;
                v[i] = _v;
                height[i] = _height;
            }
        }

        private ushort zigZagDecode(ushort val)
        {
            return (ushort)((val >> 1) ^ (-(val & 1)));
        }
    }
}
