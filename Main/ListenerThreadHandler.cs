using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Main
{
    public class ListenerThreadHandler
    {
        public void Process(HttpListenerContext context)
        {
            Program.TimeStamp("Process Thread ID: " + Thread.CurrentThread.ManagedThreadId);
            CommonResponse(context);
        }
        public void CommonResponse(HttpListenerContext context)
        {
            Thread.Sleep(1000);
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