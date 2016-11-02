using System;
using System.Collections.Generic;

namespace GraphQL.DataLoader
{
    public class FetchQueue
    {
        private static Lazy<FetchQueue> m_Current = new Lazy<FetchQueue>(() => new FetchQueue());
        public static FetchQueue Current => m_Current.Value;

        private Queue<Action> Fetchers = new Queue<Action>();
        public int Level { get; private set; }

        public void Enqueue(Action fetch)
        {
            Fetchers.Enqueue(fetch);
        }

        public bool Next()
        {
            if (Fetchers.Count == 0) return false;
            Fetchers.Dequeue().Invoke();
            return true;
        }

        public void Drain()
        {
            var length = Fetchers.Count;
            var count = 0;
            while (Next()) {
                count++;
                if (count == length) {
                    Level++;
                    length += Fetchers.Count;
                }
            }
        }
    }
}