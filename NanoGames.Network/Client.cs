// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System.Net.Sockets;
using System.Threading.Tasks;

namespace NanoGames.Network
{
    /// <summary>
    /// Represents a network client.
    /// </summary>
    /// <typeparam name="TPacketData">The packet type.</typeparam>
    public sealed class Client<TPacketData> : Endpoint<TPacketData>
    {
        private TcpClient _tcpClient;

        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;

        private Client()
        {
            _tcpClient = new TcpClient(AddressFamily.InterNetworkV6);
            _tcpClient.Client.DualMode = true;
        }

        /// <summary>
        /// Connects to a server.
        /// </summary>
        /// <param name="server">The server (hostname or IP) to connect to.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task<Endpoint<TPacketData>> ConnectAsync(string server)
        {
            var client = await Task.Run(() => new Client<TPacketData>());
            await client.ConnectInternalAsync(server);
            return client;
        }

        /// <inheritdoc/>
        protected override bool GetIsConnected()
        {
            return _tcpClient.Connected;
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
            _reader = new StreamReader(_stream, OnPacket, OnDisconnect);
            _writer = new StreamWriter(_stream);
        }

        private void OnPacket(byte[] packet)
        {
            ReceiveBytes(packet);
        }

        private void OnDisconnect()
        {
            Dispose();
        }
    }
}
