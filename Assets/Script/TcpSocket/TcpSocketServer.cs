﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
namespace MarsDT.TcpSocket
{

    public abstract class TcpSocketServer
    {
        public enum NetWorkLife
        {
            None,
            Create,
            Connecting,
            Connected,
            Disconnected,
            Destory
        }

        public static NetWorkLife TcpLifeCycle;

        private ManualResetEvent allDone = new ManualResetEvent(false);

        private int ListenNum;

        private StateObjectPool StatePool;

        private Dictionary<string, StateObject> ClientDic;

        private List<string> CloseClients;

        private Thread SocketListenThread;

        Socket severSocket = null;

        IAsyncResult SettingListernStatus = null;

        public TcpSocketServer()
        {

        }

        public void Init()
        {
            Debug.Log("TcpSocketServer Init");
            ClientDic = new Dictionary<string, StateObject>();
            CloseClients = new List<string>();
            TcpLifeCycle = NetWorkLife.None;
            ListenNum = 5;
            //allDone = new ManualResetEvent(false);
            CreateStateObjectPool();
        }

        private void CreateStateObjectPool()
        {
            StatePool = new StateObjectPool(ListenNum);
        }

        public void StartSever(string IP, int port = 50015)
        {
            if (SocketListenThread != null)
            {
                SocketListenThread.Abort();
            }
            SocketListenThread = new Thread(() =>
            {
                IPAddress ipAddress = IPAddress.Parse(IP);
                IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
                severSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                severSocket.Bind(endPoint);
                severSocket.Listen(ListenNum);
                Debug.Log("IP地址为 :" + IP + " 端口为 :" + port + " 服务已建立");
                try
                {
                    
                    while (TcpLifeCycle != NetWorkLife.Destory)
                    {
                        allDone.Reset();
                        TcpLifeCycle = NetWorkLife.Connecting;
                        if (severSocket != null && SettingListernStatus == null) {
                            Debug.Log("等待客服端连接:" + SocketListenThread.ManagedThreadId);
                            SettingListernStatus = severSocket.BeginAccept(new AsyncCallback(AcceptCallback), severSocket);
                        }
                        allDone.WaitOne();
                        Thread.Sleep(100);
                    }
                }
                catch (ThreadAbortException ex)
                {
                    Debug.Log("Socket监听线程关闭 :");
                    return;
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
                Debug.Log("Socket监听线程正常退出 :");
            });
            SocketListenThread.Start();
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            TcpLifeCycle = NetWorkLife.Connected;
            allDone.Set();
            SettingListernStatus = null;
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            StateObject state = StatePool.OutQuene();
            state.workSocket = handler;
            ClientDic.Add(handler.RemoteEndPoint.ToString(), state);
            ConnectSuccess(handler.RemoteEndPoint.ToString(), state);
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            int bytesRead = handler.EndReceive(ar);
            if (bytesRead > 0)
            {
                ReceviceData(state.buffer, bytesRead, handler.RemoteEndPoint.ToString());
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            else
            {
                TcpLifeCycle = NetWorkLife.Disconnected;
                SettingListernStatus = null;
                ClientConnectClose(state);
                ClientDic.Remove(state.workSocket.RemoteEndPoint.ToString());
                StatePool.EnQuene(state);
            }

        }

        public abstract void ConnectSuccess(string IPAddress, StateObject state);

        public abstract void ReceviceData(byte[] data, int length, string IPAddress);

        public abstract void ClientConnectClose(StateObject state);

        public void SendToAll(byte[] data)
        {
            CloseClients.Clear();
            foreach (KeyValuePair<string, StateObject> item in ClientDic)
            {
                try
                {
                    Send(item.Value.workSocket, data);
                    Debug.Log("消息长度为："+ data.Length);
                    Debug.Log("发送消息:" + Encoding.UTF8.GetString(data,0,data.Length) +" to: "+ (item.Value.workSocket.RemoteEndPoint != null ? item.Value.workSocket.RemoteEndPoint.ToString() : ""));
                }
                catch (Exception ex)
                {
                    CloseClients.Add(item.Key);
                }
            }
            CloseClients.ForEach((string IPaddress) =>
            {
                StatePool.EnQuene(ClientDic[IPaddress]);
                ClientDic.Remove(IPaddress);
            });
        }

        public void CloseServer()
        {
            try
            {
                SettingListernStatus = null;
                TcpLifeCycle = NetWorkLife.Destory;
                severSocket.Close();
                severSocket = null;
                foreach (KeyValuePair<string, StateObject> item in ClientDic)
                {
                    item.Value.workSocket.Close();
                }
                Debug.Log("尝试关闭线程 :"+ SocketListenThread.ManagedThreadId);
                SocketListenThread.Abort();
                SocketListenThread = null;
            }
            catch (Exception ex)
            {
                Debug.Log("关闭Socket连接发生异常:" + ex.Message);
            }

        }

        private void Send(Socket handler, byte[] data)
        {
            handler.BeginSend(data, 0, data.Length, 0, null, handler);
        }
    }

    public class StateObjectPool
    {

        Queue<StateObject> StateQuene = new Queue<StateObject>();

        public StateObjectPool(int num)
        {
            for (int i = 0; i < num; i++)
            {
                EnQuene(new StateObject());
            }
        }

        public void EnQuene(StateObject ob)
        {
            ClearStateObject(ob);
            StateQuene.Enqueue(ob);
        }

        public StateObject OutQuene()
        {
            if (StateQuene.Count > 0)
                return StateQuene.Dequeue();
            else
                return new StateObject();
        }

        public void ClearStateObject(StateObject ob)
        {
            if (ob != null)
                ob.workSocket = null;
        }
    }


    public class StateObject
    {

        public Socket workSocket = null;

        public const int BufferSize = 1024;

        public byte[] buffer = new byte[BufferSize];

    }

}
