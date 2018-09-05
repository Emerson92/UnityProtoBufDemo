using cs;
using MarsDT.Https;
using MarsDT.TcpSocket;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using THEDARKKNIGHT.TcpSocket;
using UnityEngine;
using System;
using UnityEngine.UI;
using ProtoBuf;
using System.IO;
using System.Text;

public class ProtobufferTest : MonoBehaviour {

    public bool IsClient = true;

    public Text txtMsg;

    public Button SendMsg;

    public Button ConnectTosever;

    public bool IsOpenNewMethod = true;

    private void Awake()
    {
        ThreadCrossHelper.Instance.CreatThreadCrossHelp();
        if (!IsClient)
        {
            TcpSocketMgr.GetInstance().Init(new ReceviceDataKeeper(new MessagerSolver()), new MessagerDataSender());
            TcpSocketMgr.GetInstance().StartSever(GetIP(), 50015);
            TcpSocketMgr.GetInstance().GetMessageKeeper().Solver.MessageFixedUpCallback += TcpServerMsgCallback;
        }
        else {
            TcpSocketClientMgr.GetInstance().Init(new ReceviceDataKeeper(new MessagerSolver()), new MessagerDataSender());
            TcpSocketClientMgr.GetInstance().GetMessageKeeper().Solver.MessageFixedUpCallback += TcpClientMsgCallback;
        }
        SendMsg.onClick.AddListener(SendMsgOnClick);
        ConnectTosever.onClick.AddListener(()=> {
            if(IsClient)
                TcpSocketClientMgr.GetInstance().ConnectToServer(GetIP(), 50015);
        });
        //UserData user1 = new UserData();
        //user1.id = 1;
        //user1.name = "User1";
        //user1.level = 10;
        //string json = JsonUtility.ToJson(user1);
        //Debug.Log("Json Length:" + Encoding.UTF8.GetByteCount(json));
    }

    private void TcpClientMsgCallback(byte[] data)
    {
        Debug.Log("收到消息回复 TcpClientMsgCallback");
        ThreadCrossHelper.Instance.ExcutionFunc(()=> {
            string msg = null;
            if (IsOpenNewMethod)
            {
                UserData user = PackCodec.Deserialize<UserData>(data);
                msg = string.Format("user2-> id:{0}, name:{1}, level:{2}", user.id, user.name, user.level);
            }
            else {
                CSLoginInfo mLoginInfo = PackCodec.Deserialize<CSLoginInfo>(data);
                msg = "UserName = " + mLoginInfo.UserName + ", Password = " + mLoginInfo.Password;
            }
            txtMsg.text += msg;
        });

    }

    private void TcpServerMsgCallback(byte[] data)
    {
        Debug.Log("收到消息回复 TcpServerMsgCallback");
        ThreadCrossHelper.Instance.ExcutionFunc(() => {
            string msg = null;
            if (IsOpenNewMethod)
            {
                UserData user = PackCodec.Deserialize<UserData>(data);
                msg = string.Format("user2-> id:{0}, name:{1}, level:{2}", user.id, user.name, user.level);
            }
            else {
                CSLoginInfo mLoginInfo = PackCodec.Deserialize<CSLoginInfo>(data);
                msg = "UserName = " + mLoginInfo.UserName + ", Password = " + mLoginInfo.Password;
            }

            txtMsg.text += msg;
        });
        byte[] buff = null;
        if (IsOpenNewMethod)
        {
            UserData user1 = new UserData();
            user1.id = 1;
            user1.name = "User1";
            user1.level = 10;
            //序列化 
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize<UserData>(ms, user1);
                ms.Position = 0;
                int length = (int)ms.Length;
                buff = new byte[length];
                ms.Read(buff, 0, length);
            }
        }
        else {
            CSLoginInfo Info = new CSLoginInfo();
            Info.UserName = "linshuhe";
            Info.Password = "123456";
            CSLoginReq mReq = new CSLoginReq();
            mReq.LoginInfo = Info;
            buff = PackCodec.Serialize(mReq);
        }

        TcpSocketMgr.GetInstance().GetSendAssist().SendMsg(buff);
    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.D))
        {
            SendMsgOnClick();
        }
    }

    private void SendMsgOnClick()
    {
        if (!IsClient)
        {
            byte[] buff = null;
            if (IsOpenNewMethod)
            {
                UserData user1 = new UserData();
                user1.id = 1;
                user1.name = "User1";
                user1.level = 10;
                //序列化 
                using (MemoryStream ms = new MemoryStream())
                {
                    Serializer.Serialize<UserData>(ms, user1);
                    ms.Position = 0;
                    int length = (int)ms.Length;
                    buff = new byte[length];
                    ms.Read(buff, 0, length);
                }
            }
            else
            {
                CSLoginInfo Info = new CSLoginInfo();
                Info.UserName = "linshuhe";
                Info.Password = "123456";
                CSLoginReq mReq = new CSLoginReq();
                mReq.LoginInfo = Info;
                buff = PackCodec.Serialize(mReq);
            }
            TcpSocketMgr.GetInstance().GetSendAssist().SendMsg(buff);
        }
        else
        {
            byte[] buff = null;
            if (IsOpenNewMethod)
            {
                UserData user1 = new UserData();
                user1.id = 1;
                user1.name = "User1";
                user1.level = 10;
                //序列化 
                using (MemoryStream ms = new MemoryStream())
                {
                    Serializer.Serialize<UserData>(ms, user1);
                    ms.Position = 0;
                    int length = (int)ms.Length;
                    buff = new byte[length];
                    ms.Read(buff, 0, length);
                }
            }
            else
            {
                CSLoginInfo Info = new CSLoginInfo();
                Info.UserName = "linshuhe";
                Info.Password = "123456";
                CSLoginReq mReq = new CSLoginReq();
                mReq.LoginInfo = Info;
                buff = PackCodec.Serialize(mReq);
            }
            TcpSocketClientMgr.GetInstance().GetSendAssist().SendMsg(buff);
        }
    }


    /// <summary>
    /// GetIpAddress Created by TheDarkKnight
    /// </summary>
    /// <returns></returns>
    public static string GetIP()
    {
#if UNITY_STANDALONE_WIN || UNITY_IOS || UNITY_ANDROID||UNITY_EDITOR
        //获取说有网卡信息
        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface adapter in nics)
        {
            //NetworkInterfaceType.Ethernet 本地连接   NetworkInterfaceType.Wireless80211无线连接
            if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet || adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
            {
                if (adapter.Name.StartsWith("v"))
                    continue;
                //获取以太网卡<a href="https://www.baidu.com/s?wd=%E7%BD%91%E7%BB%9C%E6%8E%A5%E5%8F%A3&tn=44039180_cpr&fenlei=mv6quAkxTZn0IZRqIHckPjm4nH00T1Ydm1TzP1NhmWw9nvn3nADd0ZwV5Hcvrjm3rH6sPfKWUMw85HfYnjn4nH6sgvPsT6KdThsqpZwYTjCEQLGCpyw9Uz4Bmy-bIi4WUvYETgN-TLwGUv3EnHnvP10YnHRznjf1n1bznjnLrf" target="_blank" class="baidu-highlight">网络接口</a>信息
                IPInterfaceProperties ip = adapter.GetIPProperties();
                //获取单播地址集
                UnicastIPAddressInformationCollection ipCollection = ip.UnicastAddresses;
                foreach (UnicastIPAddressInformation ipadd in ipCollection)
                {
                    if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        //Debug.Log(""+ ipadd.Address.ToString());
                        if (ipadd.Address.ToString().Contains("192.168"))
                            return ipadd.Address.ToString();
                    }
                }
            }
        }
        return null;
#elif UNITY_UWP
        var hosts = NetworkInformation.GetHostNames();
        foreach (var h in hosts) {
            bool isIpaddr = (h.Type == Windows.Networking.HostNameType.Ipv4) || (h.Type == Windows.Networking.HostNameType.Ipv6);
            if (isIpaddr) {
                // 如果不是IP地址表示的名称，则忽略
                IPInformation ipinfo = h.IPInformation;
                // 71表示无线，6表示以太网
                if (ipinfo.NetworkAdapter.IanaInterfaceType == 71 || ipinfo.NetworkAdapter.IanaInterfaceType == 6)
                {
                    Debug.Log("本机的IP地址为："+ h.DisplayName);
                    return h.DisplayName;
                }
            }

        }
        return null;
#endif
#if UNITY_WSA
        return null;
#endif
    }
}
/// <summary>
/// 用于测试的数据类
/// </summary>
[Serializable]
[ProtoContract]  //声明这个类能被序列化
public class UserData {
    //声明每一个需要被序列化的成员，编号从1开始
    [ProtoMember(1)]
    public int id;

    [ProtoMember(2)]
    public string name;

    [ProtoMember(3)]
    public int level;

}