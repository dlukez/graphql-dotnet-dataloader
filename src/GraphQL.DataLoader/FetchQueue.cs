using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GraphQL.DataLoader
{
    public class FetchQueue
    {
        private static readonly Queue<Action> m_Queue = new Queue<Action>();

        public static Task<T> Enqueue<T>(Func<T> fetch)
        {
            var tcs = new TaskCompletionSource<T>();
            m_Queue.Enqueue(() => tcs.SetResult(fetch()));
            return tcs.Task;
        }

        public static void Execute()
        {
            while (m_Queue.Count > 0)
                m_Queue.Dequeue().Invoke();
        }
    }
}
