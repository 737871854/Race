using UnityEngine;
using System.Collections;
using Update;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;

public class TcpLoad : BaseLoad
{
    
    Socket client;
    NetworkStream networkStream;
    uint contentLength;
    int n = 0;
    int read = 0;
    bool isClosed = false;
    List<byte> bytes = new List<byte>();
    private UpdateEventHandler completeLoadHandler;
    public override void GetWebFile(DownFileVO downFileVO, UpdateEventHandler completeLoadHandler, bool needSave)
    {
        try
        {
            string headHost = ResUpdateManager.Instance.updateModel.HeadHost;
            string serverHost = ResUpdateManager.Instance.updateModel.ServerHost;
            int port = ResUpdateManager.Instance.updateModel.Port;

            this.completeLoadHandler = completeLoadHandler;
            this.bytes = new List<byte>();
            string query = string.Empty;
            string path = "http://" + downFileVO.DownFilePath;
            query = "GET " + path.Replace(" ", "%20") + " HTTP/1.1\r\n" +
            "Host: " + headHost + "\r\n" +
            "User-Agent: undefined\r\n" +
            "Cache-control: no-cache\r\n" +
            "Connection: close\r\n" +
            "\r\n";

            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            client.Connect(serverHost, port);

            networkStream = new NetworkStream(client);

            byte[] bytes = Encoding.Default.GetBytes(query);
            networkStream.Write(bytes, 0, bytes.Length);

            BinaryReader bReader = new BinaryReader(networkStream, Encoding.Default);

            string response = "";
            string line;
            char c;

            //read head
            do
            {
                line = "";
                c = '\u0000';
                while (true)
                {
                    c = bReader.ReadChar();
                    if (c == '\r')
                        break;
                    line += c;
                }
                c = bReader.ReadChar();
                response += line + "\r\n";
            }
            while (line.Length > 0);


            Regex reContentLength = new Regex(@"(?<=Content-Length:\s)\d+", RegexOptions.IgnoreCase);
            string value = reContentLength.Match(response).Value;
            if (!string.IsNullOrEmpty(value))
                contentLength = uint.Parse(reContentLength.Match(response).Value);
            else
                contentLength = 0;
            Main.RegisterUpdateCallback(this.Update);
        }
        catch (Exception ex)
        {
            string error = string.Format("TcpLoad version 开始下载失败,错误信息:{0}", ex.ToString());
            if (this.ErrorHandler != null) this.ErrorHandler(error);
        }
        
    }
    void Update(float time, float deltaTime)
    {
        byte[] buffer = new byte[4 * 1024 * 1000];

        if (n < contentLength)
        {
            try
            {
                if (networkStream.DataAvailable)
                {
                    read = networkStream.Read(buffer, 0, buffer.Length);
                    n += read;
                    bytes.AddRange(buffer);
                }
            }
            catch (Exception ex)
            {
                string error = string.Format("TcpLoad version 下载中失败,错误信息:{0}", ex.ToString());
                if (this.ErrorHandler != null) this.ErrorHandler(error);

            }
        }
        else
        {
            if (!isClosed)
            {
                string version = UTF8Encoding.UTF8.GetString(this.bytes.ToArray());
                isClosed = true;
                client.Close();
                Main.UnRegisterUpdateCallback(this.Update);
                if (this.completeLoadHandler != null) this.completeLoadHandler(version);
            }
        }
    }
}
