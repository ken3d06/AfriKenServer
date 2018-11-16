using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace Main
{
    /// <summary>
    /// Track the semaphore and context queue associated with a worker thread.
    /// </summary>
    class ThreadSemaphore
    {
        protected Semaphore semaphore;
        protected ConcurrentQueue<HttpListenerContext> requests;
        public int QueueCount => requests.Count;

        public ThreadSemaphore()
        {
            semaphore = new Semaphore(0, Int32.MaxValue);
            requests = new ConcurrentQueue<HttpListenerContext>();
        }
        /// <summary>
        /// Enqueue request context and release semaphore that a thread is waiting on
        /// </summary>
        /// <param name="context"></param>
        public void Enqueue(HttpListenerContext context)
        {
            requests.Enqueue(context);
            semaphore.Release();
        }
        /// <summary>
        /// Wait for the semaphore to be released
        /// </summary>
        public void WaitOne()
        {
            semaphore.WaitOne();
        }

        public bool TryDequeue(out HttpListenerContext context)
        {
            return requests.TryDequeue(out context);
        }
    }
}
