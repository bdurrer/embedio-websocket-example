using Swan;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Net;
using System.IO;

namespace EmbedIO_Websocket_Example
{
    class ServerService
    {
        private WebServer WebServer;
        private readonly string BaseUrl;

        public ServerService(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public EventSocketModule EventSocketModule { get; private set; }

        public readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        public bool IsRunning => !CancellationTokenSource.Token.IsCancellationRequested;

        public void Start()
        {
            try
            {
                if (CancellationTokenSource.IsCancellationRequested)
                {
                    Terminal.WriteLine("Abort server start, service was cancelled");
                    return;
                }

                Terminal.WriteLine($"Starting web server at {BaseUrl}");
                EndPointManager.UseIpv6 = false;

                CreateWebServer();
                Task.Factory.StartNew(
                        () => WebServer.RunAsync(CancellationTokenSource.Token), TaskCreationOptions.LongRunning)
                    .ConfigureAwait(false);
                Terminal.WriteLine($"Web server running at {BaseUrl}");

            }
            catch (Exception e)
            {
                Terminal.WriteLine("Failed to start web server" + e.Message);
            }
        }

        private void CreateWebServer()
        {
            EventSocketModule = new EventSocketModule();

            string httpRoot = HtmlRoot();
            Logger.Info("Serving files from {0}", httpRoot);
            
            WebServer = new WebServer(
                    o => o.WithUrlPrefix(BaseUrl).WithMode(HttpListenerMode.EmbedIO))
                .WithLocalSessionManager()
                .WithCors()
                .WithModule(EventSocketModule)
                .WithStaticFolder("/", httpRoot, true)
                .WithModule(new ActionModule("/", HttpVerbs.Any,
                    ctx =>
                    {
                        Terminal.WriteLine($"Attempted to request unknown endpoint {ctx.Request.HttpMethod}:{ctx.Request.Url}");
                        return ctx.SendDataAsync(new { Message = "Error" });
                    }));

            // Listen for state changes.
            WebServer.StateChanged += WebServerStateChangedEventHandler;
        }

        private void WebServerStateChangedEventHandler(object sender, WebServerStateChangedEventArgs e)
        {
            Terminal.WriteLine($"WebServer New State - {e.NewState}");
        }

        private string HtmlRoot()
        {
            // This will get the current WORKING directory (i.e. \bin\Debug)
            string workingDirectory = Environment.CurrentDirectory;
            // or: Directory.GetCurrentDirectory() gives the same result

            // This will get the current PROJECT directory
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;

            return Path.Combine(projectDirectory, "html-root");
        }
    }
}
