// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System.Net.Sockets;
using System.Threading.Tasks;

namespace NanoGames.Network
{
    /// <summary>
    /// Represents a network client.
    /// </summary>
    /// <typeparam name="TPacket">The packet type.</typeparam>
    public sealed class Client<TPacket> : Endpoint<TPacket>
    {
        private readonly TcpClient _tcpClient;

        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;

        private Client()
        {
            _tcpClient = new TcpClient();
        }

        /// <inheritdoc/>
        public override bool IsConnected
        {
            get
            {
                return _tcpClient.Connected;
            }
        }

        /// <summary>
        /// Connects to a server.
        /// </summary>
        /// <param name="server">The server (hostname or IP) to connect to.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task<Endpoint<TPacket>> ConnectAsync(string server)
        {
            var client = await Task.Run(() => new Client<TPacket>());
            await client.ConnectInternalAsync(server);
            return client;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _writer.Dispose();
                _reader.Dispose();
                _stream.Dispose();
                _tcpClient.Dispose();
            }

            base.Dispose(isDisposing);
        }

        /// <inheritdoc/>
        protected override void SendBytes(byte[] bytes)
        {
            _writer.Write(bytes);
        }

        private async Task ConnectInternalAsync(string server)
        {
            await _tcpClient.ConnectAsync(server, Server.Port);
            _stream = _tcpClient.GetStream();
            _reader = new StreamReader(_stream, OnPacket, null);
            _writer = new StreamWriter(_stream);
        }

        private void OnPacket(byte[] packet)
        {
            ReceiveBytes(packet);
        }
    }
}
