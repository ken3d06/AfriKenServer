using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Main
{
    class Program
    {
        private static Semaphore _semaphore;
        private static DateTime _startTime;
        private static ListenerThreadHandler _handler;

        public static void StartTime()
        {
            _startTime = DateTime.UtcNow;
        }

        public static void TimeStamp(string message)
        {
            long elapsedTime = (long)(DateTime.UtcNow - _startTime).TotalMilliseconds;
            Console.WriteLine($"{0} : {1}", elapsedTime, message);
        }

        static void Main(string[] args)
        {
            _semaphore = new Semaphore(20, 50);
            _handler = new ListenerThreadHandler();
            HttpListener listener = new HttpListener();
            string url = $"http://localhost/";
            listener.Prefixes.Add(url);
            listener.Start();
            Task.Run(function: () =>
            {
                while (true)
                {
                    _semaphore.WaitOne();
                    BeginConnListener(listener);
                }
            });
            Console.WriteLine("Press any key to exit AfriKen Server");
            Console.ReadLine();
        }



        private static async void BeginConnListener(HttpListener listener)
        {
            TimeStamp("BeginConnListener Thread ID: " + Thread.CurrentThread.ManagedThreadId);
            HttpListenerContext context = await listener.GetContextAsync();
            _semaphore.Release();
            _handler.Process(context);
        }
    }
}
