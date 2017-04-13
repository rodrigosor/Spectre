using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace Spectre.Pop3
{
    /// <summary>
    /// Handles a POP3 session.
    /// </summary>
    public class Session
    {
        /// <summary>
        /// Start time of this <see cref="Session"/> instance. 
        /// </summary>
        public DateTime SessionStartTime { get; private set; }

        /// <summary>
        /// The pop3 Server.
        /// </summary>
        public Server Server { get; private set; }

        /// <summary>
        /// Instance of <see cref="Socket"/> to client. 
        /// </summary>
        public Socket Client { get; private set; }

        /// <summary>
        /// Handles session ID.
        /// </summary>
        public string SessionID { get; private set; }

        /// <summary>
        /// Ticks representing the last command execution time.
        /// </summary>
        public long LatCommandTime { get; private set; }

        /// <summary>
        /// Current pop3 command sended by client.
        /// </summary>
        public Command CurrentCommand { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="Session"/>.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="sessionID"></param>
        public Session(Server server, Socket client, string sessionID)
        {
            SessionStartTime = DateTime.Now;
            Server = server;
            Client = client;
            SessionID = sessionID;
            LatCommandTime = 0;
        }

        /// <summary>
        /// Start processing commands and replys
        /// </summary>
        internal void StartProcessing()
        {
            try
            {
                Client.SendOKMessage(ServerMessages.Handshake);

                //------ Create command loop --------------------------------//
                // Loop while QUIT cmd or Session TimeOut.

                LatCommandTime.Update();

                while (ServerConstants.Forever)
                {
                    if (Client.HasData())
                    {
                        try
                        {
                            CurrentCommand = Client.GetCommand();
                        }
                        catch (Exception ex)
                        {
                            // connection lost.
                            if (!Client.Connected)
                            {
                                break;
                            }

                            Client.SendErrMessage(ServerMessages.UnknownError);
                            Server.TriggerOnError(ex, new StackTrace());
                        }

                        LatCommandTime.Update();
                    }
                    else
                    {
                        //----- Session timeout stuff ------------------------------------------------//

                        if (LatCommandTime.ReachTimeOut())
                        {
                            Client.SendErrMessage(ServerMessages.LastCommandTimeReachTimeout);
                            break;
                        }

                        // Wait a moment (100ms) to save CPU, otherwise while loop may take 100% CPU. 
                        Thread.Sleep(ServerConstants.ForAMoment);

                        //----- Session timeout stuff ------------------------------------------------//
                    }
                }

                //------ Create command loop --------------------------------//
            }
            catch (Exception ex)
            {
                Server.TriggerOnError(ex, new StackTrace());
            }
            finally
            {
                Server.RemoveSession(this);
                Client.SafeDisconnect();
            }
        }
    }
}
