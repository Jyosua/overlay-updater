using Avalonia.Controls;
using Avalonia.Remote.Protocol.Designer;
using Newtonsoft.Json;
using OverlayUpdater.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OverlayUpdater.Services
{
    public class WebHostService
    {
        private bool shutdown = true;

        public async Task Start(ProgressBarJSON json)
        {
            shutdown = false;
            var url = "http://localhost:8017/";
            using var listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            await HandleRequests(listener, json);

            // Close the listener
            listener.Close();
        }

        private async Task HandleRequests(HttpListener httpListener, ProgressBarJSON json)
        {
            int requestCount = 0;

            while (!shutdown)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await httpListener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Print out some info about the request
                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();

                // Write the response info
                byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(json));
                resp.ContentType = "application/json";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }

        public void Shutdown()
        {
            Console.WriteLine("Shutdown requested");
            shutdown = true;
        }
    }
}
