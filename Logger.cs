
using Swan.Logging;

namespace EmbedIO_Websocket_Example
{
    /// <summary>
    /// Not really useful, just easier to copy existing code using a different logger 
    /// </summary>
    public class Logger
    {
        public static void Error(string msg, params object[] args)
        {
            string.Format(msg, args).Error();
        }
        
        public static void Info(string msg, params object[] args)
        {
            string.Format(msg, args).Info();
        }
        
        public static void Debug(string msg, params object[] args)
        {
            string.Format(msg, args).Debug();
        }
        
        public static void Trace(string msg, params object[] args)
        {
            string.Format(msg, args).Trace();
        }
    }
}