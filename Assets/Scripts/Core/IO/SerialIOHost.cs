/**
* Copyright (c) 2012,广州纷享游艺设备有限公司
* All rights reserved.
* 
* 文件名称：SerialIOHost.cs
* 简    述：每条协议包含的信息
* 创建标识：？&meij  2015/10/28
* 修改标识：meij  2015/11/10
* 修改描述：采用求和校验，一条协议包含三个玩家的所有信息。
*/
using System;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using Need.Mx;
using System.Collections;

public class SerialIOHost
{
    private SerialPort _serialPort;
    private string[] _portNames;
    private int _curCom = 0;
    private int _errorCount = 0;
    private const int MaxErrorCount = 5;


    public delegate void SerialIoEventHandler();

    public event SerialIoEventHandler OnReceiveSucceed;
    public event SerialIoEventHandler OnReceiveFailed;

    private bool _hadFindCom = false;

    private static Thread _updateThread;

    /// <summary>
    /// 数据是否买可用，也就是串口发送和接收是否已经完成
    /// </summary>
    private volatile bool _updateFinished = false;
    private object _updateFinishedLocker = new object();
    public bool UpdateFinished
    {
        get
        {
            bool temp = false;
            lock (_updateFinishedLocker)
            {
                temp = _updateFinished;
            }
            return temp;
        }
        set
        {
            lock (_updateFinishedLocker)
            {
                _updateFinished = value;
            }
        }
    }

    private volatile bool _detectingHeader = true;
    private volatile bool _receiveSucceed = false;
    private object _receiveSucceedLocker = new object();
    public bool ReceiveSucceed
    {
        get
        {
            bool temp = false;
            lock (_receiveSucceedLocker)
            {
                temp = _receiveSucceed;
            }
            return temp;
        }
        set
        {
            lock (_receiveSucceedLocker)
            {
                _receiveSucceed = value;
            }
        }
    }
    private volatile bool _sendSucceedLastTime = true;
    private int _curReadIndex = 0;

    private byte[] _readBuf;

    private byte[] _writeBuf;

    private byte[] _userReadBuf;

    private byte[] _userWriteBuf;

    private int _readBufSize;
    private int _writeBufSize;

    private int _refreshRate = 1;

    private int _timer = 0;
    private int _timeout = 1000;

    private bool _dataValid = false;
    public bool DataValid
    {
        get
        {
            return _dataValid;
        }
        set
        {
            _dataValid = value;
        }
    }

    public bool Setup(int readBufSize, int writeBufSize, int refreshRate = 5)
    {
        _readBufSize = readBufSize;
        _writeBufSize = writeBufSize;

        _readBuf = new byte[_readBufSize];
        _writeBuf = new byte[_writeBufSize];

        _userReadBuf = new byte[_readBufSize];
        _userWriteBuf = new byte[_writeBufSize];

        _refreshRate = refreshRate;

        _portNames = SerialPort.GetPortNames();

        return FindDevice();
    }

    /// <summary>
    /// 打开串口
    /// </summary>
    /// <param name="com">端口号</param>
    /// <returns></returns>
    private bool OpenCom(int com)
    {
        if (_portNames == null)
        {
            _portNames = SerialPort.GetPortNames();
        }
        this._serialPort = new SerialPort(this._portNames[com], 0x4B00, Parity.None, 8, StopBits.One);
        this._serialPort.ReadTimeout = 0x3e8;
        this._serialPort.WriteTimeout = 0x3e8;
        try
        {
            _serialPort.Open();
        }
        catch (Exception e)
        {
            Debug.Log(_serialPort.PortName + ":" + e);
            return false;
        }
        return _serialPort.IsOpen;
    }

    /// <summary>
    /// 关闭串口
    /// </summary>
    public void Close()
    {
        if (_updateThread == null)
        {
            _serialPort.Close();
            return;
        }
        if (!_updateThread.IsAlive)
        {
            _serialPort.Close();
        }
        else
        {
            _updateThread.Abort();
            float timer = 0;
            while (_updateThread.IsAlive)
            {
                Thread.Sleep(100);
                timer += 100;
                Debug.Log("Watting for abort!");
                if (timer > 10000)
                {
                    Debug.Log("Force close serial port!");
                    break;
                }
            }
            _serialPort.Close();
        }
    }

    /// <summary>
    /// 是否已找到串口
    /// </summary>
    /// <returns></returns>
    public bool HadDevice()
    {
        if (_serialPort == null) return false;
        return _hadFindCom && _serialPort.IsOpen;
    }

    /// <summary>
    /// 查找串口
    /// </summary>
    /// <returns>找到返回True</returns>
    public bool FindDevice()
    {
        int roundCount = 5;
        if (_portNames == null || _portNames.Length == 0)
        {
            _portNames = SerialPort.GetPortNames();
        }
        if (_portNames.Length <= 0) return false;
        while (roundCount-- > 0)
        {
            for (int i = 0; i < _portNames.Length; i++)
            {
                if (!OpenCom(_curCom))
                {
                    Debug.LogWarning(_portNames[_curCom] + " open failed!");
                    continue;
                }
                //Send();
                int count = 0;
                try
                {
                    count = _serialPort.Read(_readBuf, _curReadIndex, _readBufSize);
                }
                catch (TimeoutException)
                {
                    Debug.LogWarning(_portNames[_curCom] + " no respond!!");
                }
                if (count > 0 && (_readBuf[0] == 0x55 || _readBuf[0] == 0xAA))
                {
                    UpdateFinished = false;
                    _hadFindCom = true;
                    _updateThread = new Thread(UpdateThread);
                    _updateThread.Start();
                    Debug.Log("Host:" + _serialPort.PortName + " have been found!!");
                    return true;
                }
                if (++_curCom >= _portNames.Length)
                {
                    _curCom = 0;
                }
            }
        }
        return false;
    }

    private void Send()
    {
        if (!_serialPort.IsOpen)
            return;
        if (ReceiveSucceed)
        {
            _writeBuf[0] = 0xAA;
        }
        else
        {
            _writeBuf[0] = 0x55;
        }
        byte num = 0;
        for (int i = 1; i < this._writeBufSize - 1; i++)
        {
            num += this._writeBuf[i];
        }
        this._writeBuf[this._writeBufSize - 1] = num;
        _serialPort.DiscardInBuffer();
        _serialPort.Write(_writeBuf, 0, _writeBufSize);
    }

    private bool Receive()
    {
        try
        {
            if (!_detectingHeader)
            {
                int count = _serialPort.Read(_readBuf, _curReadIndex, _readBufSize - _curReadIndex);
                if (count > 0)
                {
                    _curReadIndex += count;
                    _timer += _refreshRate;
                    if (_curReadIndex >= _readBufSize)
                    {
                        ReceiveSucceed = VerifyData();
                        _detectingHeader = true;
                        _curReadIndex = 0;
                        _timer = 0;
                        return true;
                    }
                    else if (_timer >= _timeout)
                    {
                        _timer = 0;
                        ReceiveSucceed = false;
                        _detectingHeader = true;
                        _curReadIndex = 0;
                        Debug.Log("Time out !");
                        return true;
                    }
                }
            }
            else
            {
                int count = _serialPort.Read(_readBuf, 0, _readBufSize);
                if (count > 0)
                {
                    if (_readBuf[0] == 0xaa)
                    {
                        _detectingHeader = false;
                        _sendSucceedLastTime = true;
                        _curReadIndex = count;
                        if (_curReadIndex >= _readBufSize)
                        {
                            ReceiveSucceed = VerifyData();
                            _detectingHeader = true;
                            _curReadIndex = 0;
                            _timer = 0;
                            return true;
                        }
                    }
                    else if (_readBuf[0] == 0x55)
                    {
                        _detectingHeader = false;
                        _sendSucceedLastTime = false;
                        _curReadIndex = count;
                        if (_curReadIndex >= _readBufSize)
                        {
                            ReceiveSucceed = VerifyData();
                            _detectingHeader = true;
                            _curReadIndex = 0;
                            _timer = 0;
                            return true;
                        }
                    }
                    else
                    {
                        Debug.Log("Miss2");
                    }
                }
            }
        }
        catch (TimeoutException)
        {
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
        return false;
    }

    private bool VerifyData()
    {
        byte num = 0;
        for (int i = 1; i < this._readBufSize - 1; i++)
        {
            num += _readBuf[i];
        }
        if (num != _readBuf[_readBufSize - 1])
        {
            Debug.LogWarning("端口:" + this._serialPort.PortName + "求和校验失败");
            this._serialPort.DiscardInBuffer();
            this._serialPort.DiscardOutBuffer();
            this._receiveSucceed = false;
            return false;
        }
        this.SwitchReadBuf();
        this._receiveSucceed = true;
        _serialPort.DiscardInBuffer();
        _serialPort.DiscardOutBuffer();
        return true;
    }

    /// <summary>
    /// 计算CRC
    /// </summary>
    /// <param name="buf">要计算的数据</param>
    /// <param name="len">数据长度</param>
    /// <returns>返回CRC</returns>
    private static uint CrcCompute(byte[] buf, uint len)
    {
        uint crc = 0x20121201;
        uint i;
        for (i = 0; i < len; i++)
        {
            crc ^= buf[i] + len;
        }
        crc *= len;
        return crc;
    }

    public void Write(int index, byte data)
    {
        _userWriteBuf[index + 1] = data;
    }

    public void Write(int index, int bit, bool set)
    {
        if (set)
        {
            byte b = (byte)(0x01 << bit);
            _userWriteBuf[index + 1] |= b;
        }
        else
        {
            byte b = (byte)(~(0x01 << bit));
            _userWriteBuf[index + 1] &= b;
        }
    }

    public void Write(int startIndex, byte[] data)
    {
        for (int i = startIndex + 1, imax = Mathf.Min(startIndex + 1 + data.Length, _userWriteBuf.Length - 4), dataIndex = 0; i < imax; ++i, ++dataIndex)
        {
            _userWriteBuf[i] = data[dataIndex];
        }
    }

    public byte Read(int index)
    {
        return _userReadBuf[index + 1];
    }

    public bool Read(int index, int bit)
    {
        return (_userReadBuf[index + 1] & (0x01 << bit)) != 0x00;
    }

    public void Read(ref byte[] outData)
    {
        for (int i = 1, imax = Mathf.Min(outData.Length + 1, _userWriteBuf.Length - 4); i < imax; ++i)
        {
            outData[i] = _userReadBuf[i];
        }
    }

    public bool IsSendSucceedLastTime()
    {
        return _sendSucceedLastTime;
    }

    static void DebugOutput(string header, byte[] data)
    {
        string s = header;
        for (int i = 0; i < data.Length; ++i)
        {
            s += Convert.ToString(data[i], 16) + "(" + data[i] + ")";
            s += " ";
        }
        Debug.Log(s);
    }

    /// <summary>
    /// 更新，在每帧里面调用
    /// </summary>
    public void UpdateIOHost()
    {
        DataValid = false;
        if (!_hadFindCom) return;
        if (!UpdateFinished) return;

        if (ReceiveSucceed)
        {
            if (OnReceiveSucceed != null)
            {
                OnReceiveSucceed();
            }
            DataValid = true;
        }
        else
        {
            if (OnReceiveFailed != null)
            {
                OnReceiveFailed();
            }
        }

        UpdateFinished = false;
    }

    /// <summary>
    /// 线程更新
    /// </summary>
    void UpdateThread()
    {
        if (_serialPort == null) return;
        if (!_serialPort.IsOpen) return;
        for (; ; )
        {
            if (!UpdateFinished)
            {
                SwitchWriteBuf();
                if (IOManager.Instance.IOCheckPass)
                {
                    Send();
                }
                while (!Receive()) ;
                UpdateFinished = true;
            }

            Thread.Sleep(_refreshRate);
        }
    }

    /// <summary>
    /// 将数据写到交换缓存
    /// </summary>
    private void SwitchWriteBuf()
    {
        lock (_userWriteBuf)
        {
            byte[] temp = _userWriteBuf;
            _userWriteBuf = _writeBuf;
            _writeBuf = temp;
        }
    }

    /// <summary>
    /// 从交换缓存读取数据
    /// </summary>
    private void SwitchReadBuf()
    {
        lock (_userReadBuf)
        {
            byte[] temp = _userReadBuf;
            _userReadBuf = _readBuf;
            _readBuf = temp;
        }
    }
}

