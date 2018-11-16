using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Main
{
   class Program
   {
      private static Semaphore _semaphore;
      private static DateTime _startTime;

      public static void StartTime()
      {
         _startTime = DateTime.UtcNow;
      }

      public static void TimeStamp(string message)
      {
         long elapsedTime = (long) (DateTime.UtcNow - _startTime).TotalMilliseconds;
         Console.WriteLine($"{0} : {1}", elapsedTime, message);
      }
      static void Main(string[] args)
      {
         _semaphore = new Semaphore(20, 50);
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

         HttpListenerRequest request = context.Request;
         HttpListenerResponse response = context.Response;

         string path = request.RawUrl.AllToTheLeftOfStringOrFull("?").AllTotheRightOfString("/");
         Console.WriteLine(path);

         try
         {
            string text = File.ReadAllText(path);
            byte[] data = Encoding.UTF8.GetBytes(text);
            response.ContentType = "text/html";
            response.ContentLength64 = data.Length;
            response.OutputStream.Write(data, 0, data.Length);
            response.ContentEncoding = Encoding.UTF8;
            response.StatusCode = 200;
            response.OutputStream.Close();
         }
         catch (Exception e)
         {
            Console.WriteLine(e.Message);
            throw;
         }
      }
   }
}
