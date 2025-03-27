using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt
{
    public class FixedSizeConcurrentQueue<T>
    {
        private readonly ConcurrentQueue<T> queue = new ConcurrentQueue<T>();
        private readonly int maxSize;
        private readonly object syncLock = new object();

        public FixedSizeConcurrentQueue(int maxSize)
        {
            this.maxSize = maxSize;
        }

        public void Enqueue(T item)
        {
            queue.Enqueue(item);

            if (queue.Count > maxSize)
            {
                lock (syncLock)
                {
                    while (queue.Count > maxSize)
                    {
                        queue.TryDequeue(out _);
                    }
                }
            }
        }

        public void Clear()
        {
            while (queue.TryDequeue(out _)) { }
        }

        public int Count => queue.Count;
    }
}
