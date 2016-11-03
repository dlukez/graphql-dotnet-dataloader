using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQL.DataLoader
{
    public delegate ILookup<int, T> FetchDelegate<T>(IEnumerable<int> ids);

    public class DataLoader<T>
    {
        private HashSet<int> m_Keys = new HashSet<int>();
        private readonly FetchDelegate<T> m_Fetch;
        private Task<ILookup<int, T>> m_Future;

        public DataLoader(FetchDelegate<T> fetch)
        {
            m_Fetch = fetch;
        }

        public async Task<IEnumerable<T>> LoadAsync(int key)
        {
            if (m_Keys.Count == 0)
                m_Future = FetchQueue.Enqueue(() => m_Fetch(Interlocked.Exchange(ref m_Keys, new HashSet<int>())));

            m_Keys.Add(key);
            var batchResult = await m_Future.ConfigureAwait(false);
            return batchResult[key];
        }
    }
}