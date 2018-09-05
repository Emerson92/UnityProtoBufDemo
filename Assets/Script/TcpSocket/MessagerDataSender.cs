using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
namespace MarsDT.TcpSocket
{

    /// <summary>
    /// 消息发送器
    /// </summary>
    public class MessagerDataSender
    {

        Action<byte[]> SendMsgToClient;

        /// <summary>
        /// 发送消息队列
        /// </summary>
        public Queue<MsgParam> SendMsgQueue;

        /// <summary>
        /// 消息ID集合
        /// </summary>
        List<string> MsgIDList;

        /// <summary>
        /// 发送消息ID
        /// </summary>
        private MsgParam TempMsg = null;

        /// <summary>
        /// 消息ID缓存数量
        /// </summary>
        private int CacahIDNum = 20;

        private Thread SendMsgThread;

        /// <summary>
        /// 补发时间间隔
        /// </summary>
        private float MsgSendIntervalTime = 5;//单位秒

        public enum TOKENSTATE
        {
            None,
            Vaild,
            Timeout
        }

        /// <summary>
        /// 令牌状态
        /// </summary>
        public static TOKENSTATE TokenState = TOKENSTATE.None;

        public MessagerDataSender()
        {
            InitData();
            CreateMsgQueueSend();
            RegisterEvent();
        }

        private void InitData()
        {
            SendMsgQueue = new Queue<MsgParam>();
            MsgIDList = new List<string>();
        }

        public void SetSendMsgFunction(Action<byte[]> function)
        {
            this.SendMsgToClient = function;
        }


        private void RegisterEvent()
        {

        }
        private void UnRegisterEvent()
        {
        }

        private bool Heartbeart(object obj)
        {
            return false;
        }


        private void CreateMsgQueueSend()
        {
            SendMsgThread = new Thread(() =>
            {
                int SendTime = 0;
                while (TcpSocketServer.TcpLifeCycle != TcpSocketServer.NetWorkLife.Destory)
                {
                    try
                    {
                        if (TempMsg == null)
                        {
                            if (SendMsgQueue != null)
                            {
                                if (SendMsgQueue.Count > 0)
                                {
                                    TempMsg = SendMsgQueue.Dequeue();
                                    //开始发送消息
                                    SendMsg(TempMsg.Msg);
                                    SendTime = GetTimeStamp();
                                }
                            }
                            else
                            {
                                SendMsgQueue = new Queue<MsgParam>();
                                Debug.Log("SendMsgQueue 没有实例化 :" + SendMsgThread.Name);
                            }

                        }
                        else
                        {
                            if (GetTimeStamp() - SendTime >= MsgSendIntervalTime)//间隔一定时间补发消息
                            {
                                //开始发送消息
                                SendMsg(TempMsg.Msg);
                                SendTime = GetTimeStamp();
                            }
                        }
                        Thread.Sleep(200);
                    }
                    catch (ThreadAbortException ex)
                    {
                        Debug.Log("消息发送队列线程关闭");
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("CreateMsgQueueSend" + ex.Message);
                    }
                }
                Debug.Log("消息发送队列线程正常退出");
            });
            SendMsgThread.IsBackground = true;
            SendMsgThread.Start();
        }

        public void Close()
        {
            UnRegisterEvent();
            SendMsgThread.Abort();
            MsgIDList.Clear();
            SendMsgQueue.Clear();
            SendMsgQueue = null;
            MsgIDList = null;
        }

        public void SendMsg(string msg)
        {
            if (SendMsgToClient != null)
                SendMsgToClient(Encoding.UTF8.GetBytes(msg));
        }

        public void SendMsg(byte[] msg) {

            if (SendMsgToClient != null)
                SendMsgToClient(msg);
        }

        /// <summary>
        /// msgID的检测与管理
        /// </summary>
        /// <param name="msgID"></param>
        /// <returns></returns>
        private bool MsgIDCheck(string msgID)
        {
            if (MsgIDList.Count > CacahIDNum)
                CleanListElement();
            if (ComparerIDList(msgID))
                return true;
            else
                MsgIDList.Add(msgID);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgID"></param>
        /// <returns></returns>
        private bool ComparerIDList(string msgID)
        {
            bool isFind = false;
            MsgIDList.ForEach((value) =>
            {
                if (msgID == value)
                    isFind = true;
            });
            return isFind;
        }

        private void CleanListElement()
        {
            //消息ID超过一定数量时候，清空部分ID
            for (int i = 0; i < CacahIDNum / 2; i++)
            {
                MsgIDList.RemoveAt(i);
            }
        }

        private void CheckMsgID(string ID)
        {
            if (TempMsg.ID == ID)
                TempMsg = null;
        }

        /// <summary> 
        /// 获取时间戳 
        /// </summary> 
        /// <returns></returns> 
        public int GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return (int)Convert.ToInt64(ts.TotalSeconds);
        }
    }

    public class MsgParam
    {

        public string ID;

        public string Msg;

        public MsgParam(string ID, string Msg)
        {
            this.ID = ID;
            this.Msg = Msg;
        }
    }
}
