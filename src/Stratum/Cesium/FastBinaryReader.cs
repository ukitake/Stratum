﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.Cesium
{
    public unsafe class FastBinaryReader : IDisposable
    {
        private GZipStream compressedStream;
        private MemoryStream memStream;
        private byte[] data;
        private long position;

        private GCHandle handle;
        private byte* fixedPtr;

        public FastBinaryReader(GZipStream cStream)
        {
            this.compressedStream = cStream;
            this.memStream = new MemoryStream();

            this.compressedStream.CopyTo(memStream, 8192);
            data = memStream.ToArray();
            memStream.Dispose();

            handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            fixedPtr = (byte*)handle.AddrOfPinnedObject().ToPointer();
        }

        public byte* FixedPtr { get { return fixedPtr; } }

        public void AdvanceBytes(uint numBytes)
        {
            fixedPtr = fixedPtr + numBytes;
        }

        public bool HasMore()
        {
            return position < data.Length;
        }

        public byte ReadByte()
        {
            byte ret = *fixedPtr;
            position++;
            fixedPtr = fixedPtr + 1;
            return ret;
        }

        // ***** portability issue endianess *****

        public float ReadSingle()
        {
            float ret = *(float*)fixedPtr;
            fixedPtr = fixedPtr + 4;
            position = position + 4;
            return ret;
        }

        public double ReadDouble()
        {
            double ret = *(double*) fixedPtr;
            fixedPtr = fixedPtr + 8;
            position = position + 8;
            return ret;
        }

        public ushort ReadUInt16()
        {
            ushort ret = *(ushort*) fixedPtr;
            fixedPtr = fixedPtr + 2;
            position = position + 2;
            return ret;
        }

        public uint ReadUInt32()
        {
            uint ret = *(uint*) fixedPtr;
            fixedPtr = fixedPtr + 4;
            position = position + 4;
            return ret;
        }

        public void Dispose()
        {
            handle.Free();
            fixedPtr = null;

            data = null;
        }
    }
}
