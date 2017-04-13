using Spectre.EventHandlers;
using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Spectre.Pop3
{
    /// <summary>
    /// A pop3 Server component.
    /// </summary>
    public class Server
    {
        #region Event Handlers

        /// <summary>
		/// Occurs when server has system error(Unknown error).
		/// </summary>
		public event ErrorEventHandler Error;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        private TcpListener _listener;

        /// <summary>
        /// 
        /// </summary>
        private Hashtable _sessionContainer;

        /// <summary>
        /// Gets or sets which IP address to listen.
        /// </summary>
        public IPAddress IPAddress { get; set; }

        /// <summary>
        /// Gets or sets which port to listen.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Indicates if server is listening.
        /// </summary>
        public bool IsListening { get; private set; }

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="Server"/>.
        /// </summary>
        public Server() : this(null, 110)
        { }

        /// <summary>
        /// Creates an instance of <see cref="Server"/>.
        /// </summary>
        /// <param name="ipAddress">Which IP address to listen.</param>
        public Server(string ipAddress) : this(ipAddress, 110)
        { }

        /// <summary>
        /// Creates an instance of <see cref="Server"/>.
        /// </summary>
        /// <param name="ipAddress">Which IP address to listen.</param>
        /// <param name="port">Which port to listen.</param>
        public Server(string ipAddress, int port)
        {
            IPAddress = (!string.IsNullOrWhiteSpace(ipAddress)) ? IPAddress.Parse(ipAddress) : IPAddress.Any;
            Port = port;
            _listener = new TcpListener(IPAddress, Port);
        }

        #endregion

        #region Event Triggers

        /// <summary>
        /// Trigger the OnError event handler.
        /// </summary>
        /// <param name="exception">Occured error's Exception.</param>
        /// <param name="stackTrace">Occured error's Stack Trace.</param>
        internal void TriggerOnError(Exception exception, StackTrace stackTrace)
        {
            Error?.Invoke(this, new ErrorEventArgs(exception, stackTrace));
        }

        #endregion

        /// <summary>
        /// Store a client session.
        /// </summary>
        /// <param name="sessionID">Session ID</param>
        /// <param name="session">An instance of <see cref="Session"/> class to store.</param>
        internal void AddSession(string sessionID, Session session)
        {
            _sessionContainer.Add(sessionID, session);
        }

        /// <summary>
        /// Deletes a client session.
        /// </summary>
        /// <param name="session">An instance of <see cref="Session"/> class to remove.</param>
        internal void RemoveSession(Session session)
        {
            lock (_sessionContainer)
            {
                if (!_sessionContainer.Contains(session))
                {
                    TriggerOnError(
                            new Exception(string.Format("Session ", session)),
                            new StackTrace()
                        );

                    _sessionContainer.Remove(session);

                    // Maybe trigger an event here?
                }
            }
        }

        /// <summary>
        /// Start listening on Ip/port (starts server message loop.).
        /// </summary>
        private void Listen()
        {
            try
            {
                _listener.Start();

                while (ServerConstants.Forever)
                {
                    if (_sessionContainer.Count < ServerConstants.MaxSessionsAllowed)
                    {
                        var clientSocket = _listener.AcceptSocket();
                        var sessionID = clientSocket.GetHashCode().ToString();
                        var session = new Session(this, clientSocket, sessionID);
                        var clientThread = new Thread(new ThreadStart(session.StartProcessing));

                        AddSession(sessionID, session);
                        clientThread.Start();
                    }
                    else
                    {
                        Thread.Sleep(ServerConstants.ForAMoment);
                    }
                }
            }
            catch (ThreadInterruptedException)
            {
                Thread.CurrentThread.Abort();
            }
            catch (Exception ex)
            {
                TriggerOnError(ex, new StackTrace());
            }
        }

        /// <summary>
        /// Starts pop3 server.
        /// </summary>
        public void Start()
        {
            try
            {
                if (IsListening)
                {
                    _sessionContainer = new Hashtable();
                    var serverThread = new Thread(new ThreadStart(Listen));

                    serverThread.Start();
                }
            }
            catch (Exception ex)
            {
                TriggerOnError(ex, new StackTrace());
            }
        }

        /// <summary>
        /// Stops POP3 Server.
        /// </summary>
        public void Stop()
        {
            try
            {
                if (_listener != null)
                {
                    _listener.Stop();
                }
            }
            catch (Exception ex)
            {
                TriggerOnError(ex, new StackTrace());
            }
        }
    }
}
