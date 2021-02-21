using Swan;
using System;
using System.Threading;

namespace EmbedIO_Websocket_Example
{
    class Program
    {
        private static readonly string ServiceName = "EmbedIOWebsocketService";
        public static int Main(string[] args)
        {
            var mutex = new Mutex(false, ServiceName);

            try
            {
                // Makes sure only one instance of the Tray app is started
                if (mutex.WaitOne(0, false))
                {
                    var mainProgram = new Program();
                    mainProgram.RunServer();


                    Terminal.WriteLine("Service has closed");
                }
                else
                {
                    Terminal.WriteLine($@"{ServiceName} already running");
                }
            }
            catch (Exception e)
            {
                Terminal.WriteLine("Error in Service " + e.Message);
            }
            finally
            {
                mutex.Close();
            }

            return 0;
        }

        private void RunServer()
        {
            ServerService service = new ServerService("http://localhost:8080");
            service.Start();
            
            // Wait for any key to be pressed before disposing of our web server.
            // In a service, we'd manage the lifecycle of our web server using
            // something like a BackgroundWorker or a ManualResetEvent.
            Console.ReadKey(true);
        }
    }
}
