using Spectre.Pop3;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Spectre
{
    /// <summary>
    /// Provides some methods to extends a variety of classes functionality.
    /// </summary>
    public static class SpectreExtensions
    {
        #region String

        /// <summary>
        /// When overridden in a derived class, encodes all the characters in the specified character array into a sequence of bytes.
        /// </summary>
        /// <param name="value">The string containing the characters to encode.</param>
        /// <returns>A byte array containing the results of encoding the specified set of characters.</returns>
        public static byte[] GetBytes(this string value)
        {
            return Encoding.ASCII.GetBytes(value.ToCharArray());
        }

        #endregion

        #region Byte

        /// <summary>
        /// When overridden in a derived class, decodes all the bytes in the specified byte array into a string.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
        /// <returns>A string that contains the results of decoding the specified sequence of bytes.</returns>
        public static string GetString(this byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        #endregion

        #region Sockets

        /// <summary>
        /// Sends all message to <see cref="Socket"/>.
        /// </summary>
        /// <param name="socket">The <see cref="Socket"/> to send message.</param>
        /// <param name="message">The message.</param>
        /// <returns>The number of bytes sent to the <see cref="Socket"/>.</returns>
        public static int SendAll(this Socket socket, string message)
        {
            var buffer = message.GetBytes();
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

        /// <summary>
        /// Read a line from socket data.
        /// </summary>
        /// <param name="socket">An instance of <see cref="Socket"/>.</param>
        /// <param name="bufferMaxLength">Max length of read buffer.</param>
        /// <param name="idleTimeout">the timeout for idle.</param>
        /// <returns>A line readed from socket data.</returns>
        public static string ReadLine(this Socket socket, int bufferMaxLength, int idleTimeout)
        {
            try
            {
                var lastDataTime = DateTime.Now.Ticks;
                var buffer = new List<byte>();
                var prevByte = new byte();

                while (ServerConstants.Forever)
                {
                    if (socket.HasData())
                    {
                        var currByte = new byte[1];
                        var receivedCounter = socket.Receive(currByte, 1, SocketFlags.None);

                        if (receivedCounter.Equals(1))
                        {
                            buffer.Add(currByte[0]);

                            if ((prevByte.Equals(ServerConstants.CarrigeReturn) &&
                                currByte[0].Equals(ServerConstants.LineFeed)))
                            {
                                // remove (CRLF)
                                var lineBytes = new byte[(buffer.Count - 2)];
                                buffer.CopyTo(0, lineBytes, 0, (buffer.Count - 2));

                                return lineBytes.GetString();
                            }

                            prevByte = currByte[0];

                            if (buffer.Count > bufferMaxLength)
                            {
                                throw new ReadException(ReadReplyCode.LengthExceeded, "Maximum line length exceeded");
                            }

                            lastDataTime.Update();
                        }
                    }
                    else
                    {
                        //----- Session timeout stuff ------------------------------------------------//

                        if (lastDataTime.ReachTimeOut(idleTimeout))
                        {
                            throw new ReadException(ReadReplyCode.TimeOut, "Read timeout");
                        }

                        // Wait a moment (100ms) to save CPU, otherwise while loop may take 100% CPU. 
                        Thread.Sleep(ServerConstants.ForAMoment);

                        //----- Session timeout stuff ------------------------------------------------//
                    }
                }
            }
            catch (ReadException rEx)
            {
                throw rEx;
            }
            catch (Exception ex)
            {
                throw new ReadException(ReadReplyCode.UnKnownError, ex.Message, ex);
            }
        }

        /// <summary>
        /// Read a line from socket data, with a buffer of <see cref="ServerConstants.BufferMaxLength"/> size.
        /// </summary>
        /// <param name="socket">An instance of <see cref="Socket"/>.</param>
        /// <returns>A line readed from socket data.</returns>
        public static string ReadLine(this Socket socket)
        {
            return socket.ReadLine(ServerConstants.BufferMaxLength, ServerConstants.CommandIdleTimeOut);
        }

        /// <summary>
        /// Get the pop3 command from socket.
        /// </summary>
        /// <param name="socket">An instance of <see cref="Socket"/>.</param>
        /// <returns>An instance of <see cref="Command"/> with it's arguments.</returns>
        public static Command GetCommand(this Socket socket)
        {
            return new Command(
                    socket.ReadLine(ServerConstants.BufferMaxLength, ServerConstants.CommandIdleTimeOut)
                );
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
        /// Check if measure if a datetime ticks reach time out. 
        /// </summary>
        /// <param name="measureDatetimeTicks">The datetime ticks time.</param>
        /// <returns>A <see cref="System.Boolean"/> representing if a datetime ticks time reach time out.</returns>
        public static bool ReachTimeOut(this long measureDatetimeTicks, int? idleTimeout = null)
        {
            var timeout = (idleTimeout != null) ? idleTimeout : ServerConstants.SessionIdleTimeOut;
            return (DateTime.Now.Ticks > measureDatetimeTicks + ((long)(timeout)) * 10000);
        }

        #endregion
    }
}
