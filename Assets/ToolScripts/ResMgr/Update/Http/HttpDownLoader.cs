using UnityEngine;
using System;
using System.Collections;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public delegate void NotifyFileSizeHandler(uint fileSize);
public delegate void NotifyDownLoadedSizeHandler(int downLoadedSize);
public delegate void NotifyDownLoadErrorHandler();
public delegate void NotifyDownLoadedCompleteHandler();

public class HttpDownLoader : MonoBehaviour
{
    string serverHost = ResUpdateManager.Instance.updateModel.ServerHost;
    int port = ResUpdateManager.Instance.updateModel.Port;
    string headHost = ResUpdateManager.Instance.updateModel.HeadHost;
    string absolutePath = ResUpdateManager.Instance.AbsolutePath;
    string saveZippath = ResUpdateManager.Instance.SaveZippath;
    uint contentLength;
    int n = 0;
    int read = 0;
    bool isClosed = false;

    NetworkStream networkStream;
    FileStream fileStream;
    Socket client;

    private NotifyFileSizeHandler notifyFileSizeHandler = ResUpdateManager.Instance.NotifyFileSize;
    private NotifyDownLoadedSizeHandler notifyDownLoadedSizeHandler = ResUpdateManager.Instance.NotifyDownLoadedSize;
    private Action notifyDownLoadErrorHandler = ResUpdateManager.Instance.NotifyDownLoadError;
    private Action notifyDownLoadedCompleteHandler = ResUpdateManager.Instance.NotifyDownLoadedComplete;

    #region 单例
    private static HttpDownLoader _instance;
    public static HttpDownLoader instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject();
                go.name = "ResDownLoader";
                _instance = go.AddComponent<HttpDownLoader>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }
    #endregion

    void Start()
    {
        try
        {
            long lStartPos = 0;
            if (File.Exists(saveZippath))
            {
                fileStream = File.OpenWrite(saveZippath);
                lStartPos = fileStream.Length;
                fileStream.Seek(lStartPos, System.IO.SeekOrigin.Current);
            }
            else
            {
                string dirName=Path.GetDirectoryName(saveZippath);
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                fileStream = new FileStream(saveZippath, System.IO.FileMode.Create);
                lStartPos = 0;
            }

            string query = string.Empty;
            if (lStartPos > 0)
            {
                query = "GET " + absolutePath.Replace(" ", "%20") + " HTTP/1.1\r\n" +
                "Host: " + headHost + "\r\n" +
                "User-Agent: undefined\r\n" +
                "Cache-control: no-cache\r\n" +
                "Range: bytes=" + lStartPos + "-\r\n" +
                "Connection: close\r\n" +
                "\r\n";
            }
            else
            {
                query = "GET " + absolutePath.Replace(" ", "%20") + " HTTP/1.1\r\n" +
                "Host: " + headHost + "\r\n" +
                "User-Agent: undefined\r\n" +
                "Cache-control: no-cache\r\n" +
                "Connection: close\r\n" +
                "\r\n";
            }

            Debug.Log(query);

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

            Debug.Log(response);

            Regex reContentLength = new Regex(@"(?<=Content-Length:\s)\d+", RegexOptions.IgnoreCase);
            string value = reContentLength.Match(response).Value;
            if (!string.IsNullOrEmpty(value))
                contentLength = uint.Parse(reContentLength.Match(response).Value);
            else
                contentLength = 0;
            //if (notifyFileSizeHandler != null)
            //    notifyFileSizeHandler(contentLength);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            if (notifyDownLoadErrorHandler != null) notifyDownLoadErrorHandler();
        }
    }


    void Update()
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
                    fileStream.Write(buffer, 0, read);
                }
                if (notifyDownLoadedSizeHandler != null)
                    notifyDownLoadedSizeHandler(n);                
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                if (notifyDownLoadErrorHandler != null) notifyDownLoadErrorHandler();
            }
        }
        else
        {
            if (!isClosed)
            {
                isClosed = true;
                fileStream.Flush();
                fileStream.Close();
                client.Close();
                if (notifyDownLoadedCompleteHandler != null) notifyDownLoadedCompleteHandler();
            }
        }
    }
}
