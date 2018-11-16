using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Main
{
    class SingleThreadedQueueingHandler
    {
        protected ConcurrentQueue<HttpListenerContext> requests;
        protected Semaphore semQueue;
        protected List<ThreadSemaphore> threadPool;
        protected const int MAX_WORKER_THREADS = 20;
        private static ListenerThreadHandler _handler;

        public SingleThreadedQueueingHandler()
        {
            threadPool = new List<ThreadSemaphore>();
            requests = new ConcurrentQueue<HttpListenerContext>();
            semQueue = new Semaphore(0, Int32.MaxValue);
            StartThreads();
            MonitorQueue();
            _handler = new ListenerThreadHandler();
        }

        private void StartThreads()
        {
            for (int i = 0; i < MAX_WORKER_THREADS; i++)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(ProcessThread)) {IsBackground = true};
                ThreadSemaphore ts = new ThreadSemaphore();
                threadPool.Add(ts);
                thread.Start(ts);
            }
        }
        /// <summary>
        /// As a thread, we wait until there's something to do
        /// </summary>
        /// <param name="state"></param>
        private void ProcessThread(object state)
        {
            ThreadSemaphore ts = (ThreadSemaphore)state;
            while (true)
            {
                ts.WaitOne();
                HttpListenerContext context;
                if (ts.TryDequeue(out context))
                {
                    Program.TimeStamp("Processing on thread " + Thread.CurrentThread.ManagedThreadId);
                    _handler.CommonResponse(context);
                }
            }
        }

        private void MonitorQueue()
        {
            Task.Run(() =>
            {
                int threadIndex = 0;
                //Forever
                while (true)
                {
                    // wait until we have context
                    semQueue.WaitOne();
                    HttpListenerContext context;
                    if (requests.TryDequeue(out context))
                    {
                        // In a round-robin manner, queue up the request on the current thread index then increment the index.
                        threadPool[threadIndex].Enqueue(context);
                        threadIndex = (threadIndex + 1) % MAX_WORKER_THREADS;
                    }
                }
            });
        }
    }
}
