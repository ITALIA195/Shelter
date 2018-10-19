using System;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Threading;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Photon
{
    internal class SocketUdp : IPhotonSocket
    {
        private Socket sock;
        private readonly object syncer;

        public SocketUdp(PeerBase npeer) : base(npeer)
        {
            Application.Quit();
            this.syncer = new object();
            if (ReportDebugOfLevel(DebugLevel.ALL))
            {
                Listener.DebugReturn(DebugLevel.ALL, "CSharpSocket: UDP, Unity3d.");
            }
            Protocol = ConnectionProtocol.Udp;
            PollReceive = false;
        }

        public override bool Connect()
        {
            object syncer = this.syncer;
            lock (syncer)
            {
                if (!base.Connect())
                {
                    return false;
                }
                State = PhotonSocketState.Connecting;
                new Thread(new ThreadStart(this.DnsAndConnect)) { Name = "photon dns thread", IsBackground = true }.Start();
                return true;
            }
        }

        public override bool Disconnect()
        {
            if (ReportDebugOfLevel(DebugLevel.INFO))
            {
                EnqueueDebugReturn(DebugLevel.INFO, "CSharpSocket.Disconnect()");
            }
            State = PhotonSocketState.Disconnecting;
            object syncer = this.syncer;
            lock (syncer)
            {
                if (this.sock != null)
                {
                    try
                    {
                        this.sock.Close();
                        this.sock = null;
                    }
                    catch (Exception exception)
                    {
                        EnqueueDebugReturn(DebugLevel.INFO, "Exception in Disconnect(): " + exception);
                    }
                }
            }
            State = PhotonSocketState.Disconnected;
            return true;
        }

        internal void DnsAndConnect()
        {
            try
            {
                object syncer = this.syncer;
                lock (syncer)
                {
                    this.sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    IPAddress ipAddress = GetIpAddress(ServerAddress);
                    this.sock.Connect(ipAddress, ServerPort);
                    Debug.Log("Connecting to " + ServerAddress + ":" + ServerPort);
                    State = PhotonSocketState.Connected;
                }
            }
            catch (SecurityException exception)
            {
                if (ReportDebugOfLevel(DebugLevel.ERROR))
                {
                    Listener.DebugReturn(DebugLevel.ERROR, "Connect() failed: " + exception);
                }
                HandleException(StatusCode.SecurityExceptionOnConnect);
                return;
            }
            catch (Exception exception2)
            {
                if (ReportDebugOfLevel(DebugLevel.ERROR))
                {
                    Listener.DebugReturn(DebugLevel.ERROR, "Connect() failed: " + exception2);
                }
                HandleException(StatusCode.ExceptionOnConnect);
                return;
            }
            new Thread(new ThreadStart(this.ReceiveLoop)) { Name = "photon receive thread", IsBackground = true }.Start();
        }

        public override PhotonSocketError Receive(out byte[] data)
        {
            data = null;
            return PhotonSocketError.NoData;
        }

        public void ReceiveLoop()
        {
            byte[] buffer = new byte[MTU];
            while (State == PhotonSocketState.Connected)
            {
                try
                {
                    int length = this.sock.Receive(buffer);
                    HandleReceivedDatagram(buffer, length, true);
                    continue;
                }
                catch (Exception exception)
                {
                    if (State != PhotonSocketState.Disconnecting && State != PhotonSocketState.Disconnected)
                    {
                        if (ReportDebugOfLevel(DebugLevel.ERROR))
                        {
                            EnqueueDebugReturn(DebugLevel.ERROR, string.Concat(new object[] { "Receive issue. State: ", State, " Exception: ", exception }));
                        }
                        HandleException(StatusCode.ExceptionOnReceive);
                    }
                    continue;
                }
            }
            this.Disconnect();
        }

        public override PhotonSocketError Send(byte[] data, int length)
        {
            object syncer = this.syncer;
            lock (syncer)
            {
                if (!this.sock.Connected)
                {
                    return PhotonSocketError.Skipped;
                }
                try
                {
                    this.sock.Send(data, 0, length, SocketFlags.None);
                }
                catch
                {
                    return PhotonSocketError.Exception;
                }
            }
            return PhotonSocketError.Success;
        }
    }
}
