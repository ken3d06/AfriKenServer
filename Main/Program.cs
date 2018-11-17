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
        private static SingleThreadedQueueingHandler _sHandler;

        public static void StartTime()
        {
            _startTime = DateTime.UtcNow;
        }

        public static void TimeStamp(string message)
        {
            long elapsedTime = (long)(DateTime.UtcNow - _startTime).TotalMilliseconds;
            Console.WriteLine("{0} : {1}", elapsedTime, message);
        }

        static void Main(string[] args)
        {
            //_semaphore = new Semaphore(20, 50);
            //_handler = new ListenerThreadHandler();
            _sHandler = new SingleThreadedQueueingHandler();
            HttpListener listener = new HttpListener();
            string url = "http://localhost/";
            listener.Prefixes.Add(url);
            listener.Start();
            //Task.Run(function: () =>
            //{
            //    while (true)
            //    {
            //        _semaphore.WaitOne();
            //        BeginConnListener(listener);
            //    }
            //});
            //Allocate own threads instead
            for (int i = 0; i < 20; i++)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(WaitForConnection));
                thread.IsBackground = true;
                thread.Start(listener);
            }
            StartTime();
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Request # {i}");
                MakeRequest(i);
            }
            Console.WriteLine("Press any key to exit AfriKen Server");
            Console.ReadLine();
        }

        private static void WaitForConnection(object objListener)
        {
            HttpListener listener = (HttpListener)objListener;
            while (true)
            {
                TimeStamp("StartConnectionListener Thread ID: " + Thread.CurrentThread.ManagedThreadId);
                HttpListenerContext context = listener.GetContext();
                //_handler.Process(context);
                _sHandler.Process(context);
            }
        }


        private static async void BeginConnListener(HttpListener listener)
        {
            TimeStamp("BeginConnListener Thread ID: " + Thread.CurrentThread.ManagedThreadId);
            HttpListenerContext context = await listener.GetContextAsync();
            _semaphore.Release();
            _handler.Process(context);
        }

        static async void MakeRequest(int i)
        {
            TimeStamp("MakeRequest " + i + " start, Thread ID: " + Thread.CurrentThread.ManagedThreadId);
            string ret = await RequestIssuer.HttpGet("http://localhost/firstpage.html");
            TimeStamp("MakeRequest " + i + " end, Thread ID: " + Thread.CurrentThread.ManagedThreadId);
        }
    }
}
