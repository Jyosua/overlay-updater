using Microsoft.AspNetCore.StaticFiles;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace OverlayUpdater.Services
{
    public class WebHostService
    {
        private bool shutdown = true;
        public bool ServerRunning => !shutdown;

        public async Task Start(string folderPath)
        {
            shutdown = false;
            var url = "http://localhost:8017/";
            using var listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            await HandleRequests(listener, folderPath);

            // Close the listener
            listener.Close();
        }

        private async Task HandleRequests(HttpListener httpListener, string folderPath)
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
                var fullpath = GetLocalpath(folderPath, req.Url.AbsolutePath);
                try {
                    byte[] data = await File.ReadAllBytesAsync(fullpath);
                    resp.ContentType = GetContentType(fullpath);
                    resp.ContentLength64 = data.LongLength;

                    // Write out to the response stream (asynchronously), then close it
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                } catch(Exception)
                {
                    resp.StatusCode = 404;
                }
                
                resp.Close();
            }
        }

        public void Shutdown()
        {
            Console.WriteLine("Shutdown requested");
            shutdown = true;
        }

        private string GetLocalpath(string folderPath, string requestPath)
        {
            var relativePath = requestPath.TrimStart('/').Replace('/', '\\');
            var fullpath = Path.Combine(folderPath,relativePath);
            return fullpath;
        }

        private string GetContentType(string filepath)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filepath, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
