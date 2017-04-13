using System;

namespace Spectre
{
    internal class ServerMessages
    {
        /// <summary>
        /// 
        /// </summary>
        public static string Handshake
        {
            get
            {
                return string.Format("pop3 server ready <{0}>\r\n", Guid.NewGuid().ToString().ToLower());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string LastCommandTimeReachTimeout
        {
            get
            {
                return "Session timeout, OK pop server signing off\r\n";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string UnknownError
        {
            get
            {
                return "Unkown temp error\r\n";
            }
        }
    }
}
