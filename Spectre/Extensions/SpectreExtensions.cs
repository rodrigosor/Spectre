using System;
using System.Net.Sockets;
using System.Text;

namespace Spectre
{
    /// <summary>
    /// Provides some methods to extends a variety of classes functionality.
    /// </summary>
    public static class SpectreExtensions
    {
        #region Sockets

        /// <summary>
        /// Sends all message to <see cref="Socket"/>.
        /// </summary>
        /// <param name="socket">The <see cref="Socket"/> to send message.</param>
        /// <param name="message">The message.</param>
        /// <returns>The number of bytes sent to the <see cref="Socket"/>.</returns>
        public static int SendAll(this Socket socket, string message)
        {
            var buffer = Encoding.ASCII.GetBytes(message.ToCharArray());
            return socket.Send(buffer, buffer.Length, SocketFlags.None);
        }

        /// <summary>
        /// Sends an +OK message to socket.
        /// </summary>
        /// <param name="socket">The <see cref="Socket"/> to send message.</param>
        /// <param name="message">The message.</param>
        /// <returns>The number of bytes sent to the <see cref="Socket"/>.</returns>
        public static int SendOKMessage(this Socket socket, string message)
        {
            return socket.SendAll(string.Concat("+OK ", message));
        }

        /// <summary>
        /// Sends an -ERR message to socket.
        /// </summary>
        /// <param name="socket">The <see cref="Socket"/> to send message.</param>
        /// <param name="message">The message.</param>
        /// <returns>The number of bytes sent to the <see cref="Socket"/>.</returns>
        public static int SendErrMessage(this Socket socket, string message)
        {
            return socket.SendAll(string.Concat("-ERR ", message));
        }

        /// <summary>
        /// Disconnects a socket.
        /// </summary>
        /// <param name="socket">The <see cref="Socket"/> to send message.</param>
        public static void SafeDisconnect(this Socket socket)
        {
            if (socket.Connected)
            {
                socket.Close();
            }
        }

        /// <summary>
        /// Check if there is any data available.
        /// </summary>
        /// <param name="socket">The <see cref="Socket"/> to send message.</param>
        /// <returns>A <see cref="System.Boolean"/> representing if there is any data available.</returns>
        public static bool HasData(this Socket socket)
        {
            return (socket.Available > 0);
        }

        public static string ReadAll(this Socket socket, int maxLength, int idleTimeout)
        {

        }

        #endregion

        #region Long

        /// <summary>
        /// Update this last command time with current ticks.
        /// </summary>
        /// <param name="lastCommandTime">The last command execution time.</param>
        public static void Update(this long lastCommandTime)
        {
            lastCommandTime = DateTime.Now.Ticks;
        }

        /// <summary>
        /// Check if last command execution time reach time out. 
        /// </summary>
        /// <param name="lastCommandTime">The last command execution time.</param>
        /// <returns>A <see cref="System.Boolean"/> representing if last command execution time reach time out.</returns>
        public static bool ReachTimeOut(this long lastCommandTime)
        {
            return (DateTime.Now.Ticks > lastCommandTime + ((long)(ServerConstants.SessionIdleTimeOut)) * 10000);
        }

        #endregion
    }
}
