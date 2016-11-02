using System; 
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading; 
using System.Threading.Tasks;

namespace GraphQL.Execution
{
    public static class AsyncPump 
    { 
        public static void Run(Func<Task> func) 
        { 
            if (func == null) throw new ArgumentNullException(nameof(func)); 

            var prevCtx = SynchronizationContext.Current; 
            try 
            { 
                var syncCtx = new SingleThreadedSynchronizationContext(); 
                SynchronizationContext.SetSynchronizationContext(syncCtx); 

                var t = func(); 
                if (t == null) throw new InvalidOperationException();
                t.ContinueWith(delegate { syncCtx.Complete(); }, TaskScheduler.Default);

                syncCtx.RunOnCurrentThread();
                t.GetAwaiter().GetResult();
            } 
            finally 
            {  
                SynchronizationContext.SetSynchronizationContext(prevCtx); 
            } 
        }

        sealed class SingleThreadedSynchronizationContext : SynchronizationContext
        {
            private BlockingCollection<KeyValuePair<SendOrPostCallback,object>> m_Queue =
                new BlockingCollection<KeyValuePair<SendOrPostCallback,object>>(); 

            public override void Post(SendOrPostCallback d, object state)
            {
                m_Queue.Add(new KeyValuePair<SendOrPostCallback,object>(d, state));
            }
        
            public void RunOnCurrentThread()
            {
                KeyValuePair<SendOrPostCallback, object> workItem;
                while(m_Queue.TryTake(out workItem, Timeout.Infinite))
                    workItem.Key(workItem.Value);
            }
        
            public void Complete() { m_Queue.CompleteAdding(); }
        }
    }
}