using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQL.DataLoader
{
    public delegate ILookup<int, T> FetchDelegate<T>(IEnumerable<int> ids);

    public class DataLoader<T>
    {
        private HashSet<int> _keys = new HashSet<int>();
        private FetchDelegate<T> _fetch;
        private Task<ILookup<int, T>> _future;

        public DataLoader(FetchDelegate<T> fetch)
        {
            _fetch = fetch;
        }

        public async Task<IEnumerable<T>> LoadAsync(int key)
        {
            if (_keys.Count == 0)
            {
                // queue the loader when required
                var tcs = new TaskCompletionSource<ILookup<int, T>>();
                FetchQueue.Current.Enqueue(() =>
                {
                    var keys = Interlocked.Exchange(ref _keys, new HashSet<int>());
                    var result = _fetch(keys);
                    tcs.SetResult(result);
                });
                _future = tcs.Task;
            }

            _keys.Add(key);
            var batchResult = await _future.ConfigureAwait(false);
            return batchResult[key];
        }
    }
}