using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Datafeeds.Processor
{

    public class ThreadQueue<T>
    {
        BlockingCollection<T> queue;
        IEnumerable<T> producent;

        Task t;

        public ThreadQueue(int queueSize, IEnumerable<T> producent)
        {
            this.queue = new BlockingCollection<T>(queueSize);
            this.producent = producent;
        }

        public IEnumerable<T> Consume()
        {
            if(t == null)
            {
                startProducing();
            }

            T entry;
            while (true)
            {
                try
                {
                    entry = queue.Take();
                }
                catch (InvalidOperationException e)
                {
                    break; // on CompleteAdding() called
                }
                yield return entry;
            }
        }

        private void startProducing()
        {
            this.t = Task.Run(() =>
            {
                foreach (var entry in producent)
                {
                    queue.Add(entry);
                }

                queue.CompleteAdding();
            });
        }
    }
}
