using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
namespace MarsDT.TcpSocket
{

    /// <summary>
    /// 消息接受器
    /// </summary>
    public class ReceviceDataKeeper
    {
        public IMessagerParseSolver Solver;

        public Func<byte[], int, string, object> SolverRuleExcution;

        private Dictionary<string, DataInfo> ClientTempMsgDic = new Dictionary<string, DataInfo>();

        private List<string> WholeMsgList = new List<string>();

        private const string SplitString = "\r\n";
        private const string VersionHeader = "Version:";
        private const string ContentLenghtHeader = "Content-Length:";
        private const string TcpHeadType = "Version:.*\r\nContent-Length:.*\r\n\r\n";
        public ReceviceDataKeeper(IMessagerParseSolver solver = null) {
            Debug.Log("ReceviceDataKeeper 初始化完成");
            if (solver != null) {
                SetMessagerSolver(solver);
            }
        }


        /// <summary>
        /// 消息处理器
        /// </summary>
        /// <param name="solver"></param>
        public void SetMessagerSolver(IMessagerParseSolver solver)
        {
            this.Solver = solver;
        }

        /// <summary>
        /// 设置消息规则解析器
        /// </summary>
        /// <param name="function"></param>
        public void SetRuleSolver(Func<byte[], int, string, object> function) {
            this.SolverRuleExcution = function;
        }

        public void MessageDataRecevice(byte[] data, int length, string IPAddress)
        {
            Debug.Log("MessageDataRecevice :" + Encoding.UTF8.GetString(data,0, length));
            Solver.MessageSolver(data.Skip(0).Take(length).ToArray());
        }

        public void Close() {
            ClientTempMsgDic.Clear();
            WholeMsgList.Clear();
            WholeMsgList = null;
            ClientTempMsgDic = null;
        }

        


        /// <summary>
        /// 连接已经断开
        /// </summary>
        /// <param name="state"></param>
        public virtual void ClientClose(StateObject state)
        {
            if (state != null)
            {
                string IPAddress = state.workSocket.RemoteEndPoint.ToString();
                if (ClientTempMsgDic.ContainsKey(IPAddress))
                {
                    ClientTempMsgDic.Remove(IPAddress);
                }
            }
        }


    
        private class DataInfo{

            public string Version;

            public int AsyncTotalLen = 0;//接收总长度

            public int AsyncContentLength = -1;//内容长度

            public List<byte> TempMsg;

            public DataInfo(List<byte> msg,int totalLen,int ContentLength) {
                this.TempMsg = msg;
                this.AsyncTotalLen = totalLen;
                this.AsyncContentLength = ContentLength;
            }
        }
    }

}
