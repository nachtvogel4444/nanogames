// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace NanoGames.Network
{
    /// <summary>
    /// Implements a game server.
    /// </summary>
    public sealed class Server : IDisposable
    {
        /// <summary>
        /// The server port.
        /// </summary>
        public const int Port = 13897;

        private readonly object _mutex = new object();
        private readonly TcpListener _listener;
        private readonly WorkerThread _listenerThread;
        private readonly HashSet<IConnection> _connections = new HashSet<IConnection>();

        private bool _isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
        /// The server is running until disposed.
        /// </summary>
        public Server()
        {
            _listener = CreateListener(Port);
            _listener.Start();
            _listenerThread = new WorkerThread(RunListener);
        }

        /// <summary>
        /// A server connection.
        /// </summary>
        private interface IConnection : IDisposable
        {
            /// <summary>
            /// Receives a packet.
            /// </summary>
            /// <param name="packet">The packet to receive.</param>
            void Receive(byte[] packet);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_mutex)
            {
                if (_isDisposed)
                {
                    return;
                }

                _isDisposed = true;

                try
                {
                    _listener.Stop();
                }
                catch
                {
                }

                foreach (var connection in _connections)
                {
                    connection.Dispose();
                }

                _connections.Clear();
            }

            _listenerThread.Dispose();
        }

        /// <summary>
        /// Gets an endpoint connected to this server.
        /// Disposing the endpoint also disposes the server.
        /// </summary>
        /// <typeparam name="TPacketData">The packet type.</typeparam>
        /// <returns>The new endpoint.</returns>
        public Endpoint<TPacketData> GetLocalEndpoint<TPacketData>()
        {
            lock (_mutex)
            {
                var connection = new LocalConnection<TPacketData>(this);
                _connections.Add(connection);
                return connection;
            }
        }

        private static TcpListener CreateListener(int port)
        {
            /*
             * TcpListener.Create(int port) is only available in Microsoft .NET, not in Mono.
             * However, we'd like to use it if available because it creates a convenient hybrid (IPv4/IPv6) socket.
             * So we attempt to fetch the method via reflection.
             * If it's there, invoke it, if it's not there, fall back to new TcpListener(...), which creates an IPv4 socket.
             */

            var createMethod =
                typeof(TcpListener)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(
                    m => m.Name == "Create"
                        && m.GetParameters().Length == 1
                        && m.GetParameters()[0].ParameterType == typeof(int))
                .SingleOrDefault();

            if (createMethod != null)
            {
                return (TcpListener)createMethod.Invoke(null, new object[] { port });
            }
            else
            {
                return new TcpListener(IPAddress.Any, port);
            }
        }

        private void RunListener(CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                var client = _listener.AcceptTcpClient();
                lock (_mutex)
                {
                    _connections.Add(new TcpConnection(this, client));
                }
            }
        }

        private void Broadcast(IConnection source, byte[] packet)
        {
            lock (_mutex)
            {
                foreach (var connection in _connections)
                {
                    if (connection != source)
                    {
                        connection.Receive(packet);
                    }
                }
            }
        }

        private void Disconnect(IConnection oldConnection)
        {
            lock (_mutex)
            {
                _connections.Remove(oldConnection);
            }
        }

        private class LocalConnection<TPacket> : Endpoint<TPacket>, IConnection
        {
            private readonly Server _server;

            public LocalConnection(Server server)
            {
                _server = server;
            }

            public void Receive(byte[] packet)
            {
                ReceiveBytes(packet);
            }

            protected override bool GetIsConnected() => true;

            protected override void Dispose(bool isDisposing)
            {
                if (isDisposing)
                {
                    _server.Dispose();
                }

                base.Dispose(isDisposing);
            }

            protected override void SendBytes(byte[] packetBytes)
            {
                _server.Broadcast(this, packetBytes);
            }
        }

        private class TcpConnection : IConnection
        {
            private readonly Server _server;
            private readonly TcpClient _tcpClient;
            private readonly Stream _stream;
            private readonly StreamReader _reader;
            private readonly StreamWriter _writer;

            public TcpConnection(Server server, TcpClient tcpClient)
            {
                _server = server;
                _tcpClient = tcpClient;
                _stream = tcpClient.GetStream();
                _reader = new StreamReader(_stream, OnPacket, OnDisconnect);
                _writer = new StreamWriter(_stream);
            }

            public void Receive(byte[] packet)
            {
                _writer.Write(packet);
            }

            public void Dispose()
            {
                _writer.Dispose();
                _reader.Dispose();
                _stream.Dispose();
                _tcpClient.Dispose();
            }

            private void OnPacket(byte[] packet)
            {
                _server.Broadcast(this, packet);
            }

            private void OnDisconnect()
            {
                _server.Disconnect(this);
            }
        }
    }
}
