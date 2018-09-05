using MarsDT.TcpSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace THEDARKKNIGHT.TcpSocket {


    public abstract class TcpSocketClient
    {

        public int ListenNum = 10;

        public Socket SocketClient;

        private Dictionary<string, StateObject> ClientDic = new Dictionary<string, StateObject>();

        private List<string> CloseClients = new List<string>();

        private StateObjectPool pool;

        public TcpSocketClient() {
            pool = new StateObjectPool(ListenNum);
        }

        public void ConnectToServer(string IP , int Port) {
            SocketClient = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            EndPoint IPAddress = CreateRemoteIP(IP, Port);
            SocketClient.BeginConnect(IPAddress, new AsyncCallback(OnSuccessConnected), SocketClient);
        }

        private void OnSuccessConnected(IAsyncResult ar)
        {
            Socket ConnectSocket = ((Socket)ar.AsyncState);
            StateObject state = pool.OutQuene();
            state.workSocket = ConnectSocket;
            ClientDic.Add(ConnectSocket.RemoteEndPoint.ToString(), state);
            ConnectSuccess(ConnectSocket.RemoteEndPoint.ToString());
            ConnectSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            int bytesRead = handler.EndReceive(ar);
            Debug.Log("bytesRead :" + bytesRead);
            if (bytesRead > 0)
            {
                ReceviceData(state.buffer, bytesRead, handler.RemoteEndPoint.ToString());
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            else
            {
                Debug.Log("连接已经断开:");
                ClientDic.Remove(state.workSocket.RemoteEndPoint.ToString());
                ClientConnectClose(state.workSocket.RemoteEndPoint.ToString());
                pool.EnQuene(state);
            }
        }

        public abstract void ConnectSuccess(string IPAddress);

        public abstract void ReceviceData(byte[] data, int length, string IPAddress);

        public abstract void ClientConnectClose(string IPAddress);

        public void SendMsg(byte[] msg) {
         
            CloseClients.Clear();
            foreach (KeyValuePair<string, StateObject> item in ClientDic)
            {
                try
                {
                    Debug.Log("消息长度为:"+ msg.Length);
                    Debug.Log("发送消息："+Encoding.UTF8.GetString(msg) +" to "+item.Value.workSocket.RemoteEndPoint.ToString());
                    Send(item.Value.workSocket, msg);
                }
                catch (Exception ex)
                {
                    CloseClients.Add(item.Key);
                }
            }
            CloseClients.ForEach((string IPaddress) =>
            {
                pool.EnQuene(ClientDic[IPaddress]);
                ClientDic.Remove(IPaddress);
            });
        }

        private void Send(Socket handler, byte[] data)
        {
            handler.BeginSend(data, 0, data.Length, 0, null, handler);
        }

        private EndPoint CreateRemoteIP(string ipAddress, int port)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipAddress) , port);
            return endPoint;
        }

        public void CloseClient() {
            try
            {
                if (SocketClient != null)
                    SocketClient.Close();
                ClientDic.Clear();
            }
            catch (Exception ex) {
                Debug.Log(ex.Message);
            }
        }
    }
}

