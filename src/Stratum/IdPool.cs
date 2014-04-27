using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum
{
    public class IdPool
    {
        private ConcurrentQueue<ulong> idPool;
        private ulong idPoolCapacity = 4000;

        public IdPool()
        {
            idPool = new ConcurrentQueue<ulong>();
            for (ulong k = 1; k <= idPoolCapacity; k++)
                idPool.Enqueue(k);
        }

        public ulong Get()
        {
            if (idPool.Count == 0)
            {
                for (ulong k = idPoolCapacity + 1; k <= idPoolCapacity * 2; k++)
                    idPool.Enqueue(k);

                idPoolCapacity *= 2;
            }

            ulong o;
            if (idPool.TryDequeue(out o))
                return o;
            else throw new Exception("Out of Ids");
        }

        public void Free(ulong id)
        {
            idPool.Enqueue(id);
        }
    }
}
