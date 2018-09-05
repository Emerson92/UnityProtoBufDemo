using MarsDT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace MarsDT.TcpSocket
{

    /// <summary>
    /// Socket管理类
    /// </summary>
    public class TcpSocketMgr : TcpSocketServer
    {

        /// <summary>
        /// 消息处理器
        /// </summary>
        private ReceviceDataKeeper Keeper;

        /// <summary>
        /// 消息发送器
        /// </summary>
        private MessagerDataSender Messger;

        private static TcpSocketMgr Instance;

        public static TcpSocketMgr GetInstance()
        {
            if (Instance == null)
                Instance = new TcpSocketMgr();
            return Instance;
        }

        private TcpSocketMgr()
        {
            //base.Init();
        }
        public void Init(ReceviceDataKeeper Keeper = null, MessagerDataSender Messger = null)
        {
            base.Init();
            if (Keeper == null)
                Keeper = new ReceviceDataKeeper(new MessagerSolver());
            else
                this.Keeper = Keeper;
            if (Messger == null)
                Messger = new MessagerDataSender();
            else
                this.Messger = Messger;
            Messger.SetSendMsgFunction(SendToAll);//授权
        }


        /// <summary>
        /// 获取消息发送器
        /// </summary>
        /// <returns></returns>
        public MessagerDataSender GetSendAssist() {
            return Messger;
        }

        public ReceviceDataKeeper GetMessageKeeper() {
            return Keeper;
        }


        public override void ConnectSuccess(string IPAddress, StateObject state)
        {
            NativeJoin(IPAddress);
        }

        public override void ReceviceData(byte[] data, int length,string IPAddress)
        {
            if (Keeper != null)
                Keeper.MessageDataRecevice(data, length, IPAddress);
            else
                Debug.Log("没有发现消息接受者！");
        }

        public override void ClientConnectClose(StateObject state)
        {
            if(Keeper !=null)
               Keeper.ClientClose(state);
            Debug.Log("断开连接的IP:"+state.workSocket.RemoteEndPoint.ToString());
            string CloseConnectIP = state.workSocket.RemoteEndPoint.ToString();
            NativeExit(CloseConnectIP);
        }

        public void OnDestoryServer() {
            if(Keeper != null )
                Keeper.Close();
            if(Messger != null )
                Messger.Close();
            CloseServer();
        }

        void NativeJoin(string ipPort)
        {
            Debug.Log("新的设备加入:" + (string)ipPort);
        }

        void NativeExit(string ipPort)
        {
            Debug.Log("设备已经断开连接:" + (string)ipPort);
        }
    }

}
