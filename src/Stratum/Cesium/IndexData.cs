﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.Cesium
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IndexData16
    {
        public uint triangleCount;
        public ushort[] indices;

        public IndexData16(FastBinaryReader reader)
        {
            this.triangleCount = reader.ReadUInt32();
            indices = new ushort[triangleCount * 3];

            ushort highest = 0;
            for (int i = 0; i < indices.Length; i++)
            {
                ushort code = reader.ReadUInt16();
                indices[i] = (ushort)(highest - code);

                if (code == 0)
                    highest++;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IndexData32
    {
        public uint triangleCount;
        public uint[] indices;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct EdgeIndices16
    {
        public uint westVertexCount;
        public ushort[] westIndices;

        public uint southVertexCount;
        public ushort[] southIndices;

        public uint eastVertexCount;
        public ushort[] eastIndices;

        public uint northVertexCount;
        public ushort[] northIndices;

        public EdgeIndices16(FastBinaryReader reader)
        {
            this.westVertexCount = reader.ReadUInt32();
            this.westIndices = new ushort[westVertexCount];

            for (int i = 0; i < westVertexCount; i++)
                westIndices[i] = reader.ReadUInt16();

            this.southVertexCount = reader.ReadUInt32();
            this.southIndices = new ushort[southVertexCount];

            for (int i = 0; i < southVertexCount; i++)
                southIndices[i] = reader.ReadUInt16();

            this.eastVertexCount = reader.ReadUInt32();
            this.eastIndices = new ushort[eastVertexCount];

            for (int i = 0; i < eastVertexCount; i++)
                eastIndices[i] = reader.ReadUInt16();

            this.northVertexCount = reader.ReadUInt32();
            this.northIndices = new ushort[northVertexCount];

            for (int i = 0; i < northVertexCount; i++)
                northIndices[i] = reader.ReadUInt16();
        }
    }
}
