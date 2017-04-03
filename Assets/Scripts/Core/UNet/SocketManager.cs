/*
 * Copyright (c) 
 * 
 * 文件名称：   SocketManager.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/11/18 13:42:38
 * 
 * 修改描述：   UDP 通信
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

[Serializable]
public class IPHostMessage
{
    private string _ip;
    private bool _host;

    public IPHostMessage()
    {
        _ip = SocketManager.Instance.LocalIP;
        _host = false;
    }

    public string IP { get { return _ip; } set { _ip = value; } }
    public bool Host { get { return _host; } set { _host = value; } }
    
}

public class SocketManager : Singleton<SocketManager>
{
    private string _localIP;
    private bool _isConnect = true;
    private IPHostMessage _localMesg;               // 本地IP信息
    private IPHostMessage _messageReceive;          // 接受的IP信息
    private IPHostMessage _hostMesg;                // 服务器IP信息

    // 发送
    private Socket _sendSocket;
    private byte[] _sendBytes;
    private IPEndPoint _sendIEP;
    // 接收
    private Socket _receiveSocket;
    private byte[] _receiveBytes;
    private IPEndPoint _receiveIEP;

    #region public properties
    public string LocalIP { get { return _localIP; } }
    public IPHostMessage HostMesg 
    { 
        get 
        {
            return _hostMesg; 
        } 
        set
        {
            _hostMesg = value;
        }
    }
    public IPHostMessage LocalMesg
    {
        get
        {
            if (null == _localMesg)
            {
                _localMesg = new IPHostMessage();
            }
            return _localMesg;
        }
        set
        {
            _localMesg = value;
        }
    }
   
    #endregion
    /// <summary>
    /// Socket通信初始化
    /// </summary>
    public override void Init()
    {
        GetLocalIP();
        StartSendThread();
        StartReceiveThread();
    }

 
    #region Public Function
  
    public byte[] Serialize<T>(T obj) where T : class
    {
        IFormatter _formatter = new BinaryFormatter();
        MemoryStream _streamS = new MemoryStream();
        // 序列化到一个内存流中，不生成物理文件
        _formatter.Serialize(_streamS, obj);
        _streamS.Position = 0;
        byte[] buffer = new byte[_streamS.Length];
        _streamS.Read(buffer, 0, buffer.Length);
        _streamS.Flush();
        _streamS.Close();

        return buffer;
    }

    public T Deserialize<T>(byte[] buffer) where T : class
    {
        T t = null;
        IFormatter _formatter = new BinaryFormatter();
        MemoryStream _streamD = new MemoryStream(buffer);
        t = (T)_formatter.Deserialize(_streamD);

        _streamD.Flush();
        _streamD.Close();

        return t;
    }

    /// <summary>
    /// 关闭Socket收发线程
    /// </summary>
    public void CloseConnectThread()
    {
        if (null != _sendSocket)
        {
            _sendSocket.Close();
        }

        if (null != _receiveSocket)
        {
            _receiveSocket.Close();
        }

        _isConnect = false;
    }

    /// <summary>
    /// 发送服务器IP信息
    /// </summary>
    public void SendHostIPMesg()
    {
        try
        {
            _sendBytes = Serialize<IPHostMessage>(LocalMesg);
            _sendSocket.SendTo(_sendBytes, _sendIEP);
            LocalMesg.Host = false;
            Debuger.Log("发出的IP： " + _sendBytes.Length);
        }
        catch (Exception ex)
        {
            Debuger.Log(ex.ToString());
        }
    }
    #endregion

    #region Private Function
    private void StartSendThread()
    {
        _sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _sendIEP = new IPEndPoint(IPAddress.Broadcast, 9060);
        _sendSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
    }

    /// <summary>
    /// 接受服务器端发送的IP信息
    /// </summary>
    private void StartReceiveThread()
    {
        try
        {
            _receiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _receiveIEP = new IPEndPoint(IPAddress.Any, 9060);
            _receiveSocket.Bind(_receiveIEP);
            IPEndPoint ep = (IPEndPoint)_receiveIEP;
            Thread thread = new Thread(new ThreadStart(GetHostIPThread));
            thread.Start();
        }catch(Exception ex)
        {
            Debuger.Log("没有连接到服务器");
        }
    }

    /// <summary>
    /// 获取本地IP
    /// </summary>
    private void GetHostIPThread()
    {
        while(_isConnect)
        {
            try
            {
                // 获取发送广播主机的确切通信IP
                EndPoint ep = (EndPoint)_receiveIEP;
                byte[] data = new byte[1024];
                int recv = _receiveSocket.ReceiveFrom(data, ref ep);
                _messageReceive = Deserialize<IPHostMessage>(data);
                _hostMesg = _messageReceive;
                Debuger.Log("接收到的IP： " + _messageReceive.Host + "   :  " + _messageReceive.IP);
            }
            catch(Exception ex)
            {
                Debuger.Log("没有接收到信息");
            }
        }
    }

    /// <summary>
    ///  获取本地IP地址
    /// </summary>
    private void GetLocalIP()
    {
        NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface adapter in adapters)
        {
            if (adapter.Supports(NetworkInterfaceComponent.IPv4))
            {
                UnicastIPAddressInformationCollection uniCast = adapter.GetIPProperties().UnicastAddresses;
                if (uniCast.Count > 0)
                {
                    foreach (UnicastIPAddressInformation uni in uniCast)
                    {
                        if (uni.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            _localIP = uni.Address.ToString();
                        }
                    }
                }
            }
        }
    }
    #endregion
}
