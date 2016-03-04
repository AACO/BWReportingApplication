using log4net;

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

using BWServerLogger.Exceptions;
using BWServerLogger.Model;

namespace BWServerLogger.Service {
    /// <summary>
    /// Service to get info from the provided A3 server
    /// </summary>
    class ServerInfoService {
        private const byte FILLER_BYTE = 0xFF;
        private const byte PLAYER_BYTE = 0x55;
        private const byte RULE_BYTE = 0x56;

        private static readonly ILog _logger = LogManager.GetLogger(typeof(ServerInfoService));
        private static readonly byte[] REQUEST_INFO = { 0xFF, 0xFF, 0xFF, 0xFF, 0x54, 0x53, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x20, 0x45, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x20, 0x51, 0x75, 0x65, 0x72, 0x79, 0x00 };

        /// <summary>
        /// Default constructor
        /// </summary>
        public ServerInfoService() {
        }

        /// <summary>
        /// Method to pull data from an A3 server
        /// </summary>
        /// <param name="host">Host name/IP of the A3 server</param>
        /// <param name="port">Port of the A3 server steam query point (+1 game port)</param>
        /// <returns>Filled in <see cref="ServerInfo"/> object</returns>
        public ServerInfo GetServerInfo(string host, int port) {
            Stopwatch retry = new Stopwatch();
            retry.Start();
            while (retry.ElapsedMilliseconds < Properties.Settings.Default.retryTimeLimit) {
                try {
                    using (UdpClient client = new UdpClient(56800)) {
                        IPAddress[] hostEntry = Dns.GetHostAddresses(host);

                        IPEndPoint remoteIpEndpoint = null;
                        if (hostEntry.Length > 0) {
                            remoteIpEndpoint = new IPEndPoint(hostEntry[0], port);
                        }
                        else {
                            remoteIpEndpoint = new IPEndPoint(IPAddress.Parse(host), port);
                        }
                        
                        client.Client.ReceiveTimeout = 20000;
                        client.Connect(remoteIpEndpoint);

                        //Server Info
                        //request general info
                        client.Send(REQUEST_INFO, REQUEST_INFO.Length);
                        Stopwatch ping = new Stopwatch();
                        ping.Start();
                        byte[] response = client.Receive(ref remoteIpEndpoint);
                        ping.Stop();
                        ServerInfo serverInfo = new ServerInfo(response);
                        serverInfo.Ping = ping.ElapsedMilliseconds;

                        //get player info
                        serverInfo.AddPlayers(GetChallengeResponse(PLAYER_BYTE, client, remoteIpEndpoint));

                        return serverInfo;
                    }
                } catch (Exception e) {
                    _logger.Warn("Trouble connecting to the server: ", e);
                    if (retry.ElapsedMilliseconds > Properties.Settings.Default.retryTimeLimit) {
                        throw new NoServerInfoException("Trouble connecting to the server");
                    } else {
                        System.Threading.Thread.Sleep(Properties.Settings.Default.pollRate);
                    }

                }
            }
            return null;
        }

        /// <summary>
        /// Get byte array that requires a challenge
        /// </summary>
        /// <param name="requestByte">Request byte array that requires the challenge</param>
        /// <param name="client">Client to request the info from</param>
        /// <param name="remoteIpEndpoint">IP endpoint to recieve the response byte array from</param>
        /// <returns>byte array</returns>
        private byte[] GetChallengeResponse(byte requestByte, UdpClient client, IPEndPoint remoteIpEndpoint) {
            byte[] challenge = new byte[] { FILLER_BYTE, FILLER_BYTE, FILLER_BYTE, FILLER_BYTE, requestByte, FILLER_BYTE, FILLER_BYTE, FILLER_BYTE, FILLER_BYTE };
            client.Send(challenge, challenge.Length);

            byte[] response = client.Receive(ref remoteIpEndpoint);
            byte[] request = new byte[] { FILLER_BYTE, FILLER_BYTE, FILLER_BYTE, FILLER_BYTE, requestByte, response[5], response[6], response[7], response[8] };
            client.Send(request, request.Length);

            return client.Receive(ref remoteIpEndpoint);
        }
    }
}
