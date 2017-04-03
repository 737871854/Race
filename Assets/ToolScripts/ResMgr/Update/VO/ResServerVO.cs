using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class URLObject
{
    public string url;
    public uint length;
}

public class ResServerVO
{
    public List<URLObject> urlObjectList;

    public ResServerVO()
    {
        urlObjectList = new List<URLObject>();
    }

    public void Clear()
    {
        urlObjectList.Clear();
    }
    public void Init(LitJson.JsonData jsonData)
    {
        this.Success = jsonData["res"].ToString().ToInt() == 0 ? true : false;
        if (this.Success)
        {
            string strType = jsonData["type"].ToString();
            if (strType == UpdateType.resource.ToString())
            {
                this.UType = UpdateType.resource;
            }
            else if (strType == UpdateType.client.ToString())
            {
                this.UType = UpdateType.client;
            }
            else if (strType == UpdateType.platform.ToString())
            {
                this.UType = UpdateType.platform;
            }
            else if (strType == UpdateType.none.ToString())
            {
                this.UType = UpdateType.none;
            }
            this.ServerVersion = new VersionVO(jsonData["version"].ToString().Trim());

            urlObjectList.Clear();
            int count = jsonData["urlarray"].Count;
            LitJson.JsonData jsonElement = jsonData["urlarray"];
            for (int index = 0; index < count;++index )
            {
                URLObject url = new URLObject();
                url.url    = jsonElement[index]["url"].ToString();
                url.length = (uint)jsonElement[index]["length"].ToString().ToInt();
                urlObjectList.Add(url);
                TotalLength += url.length;
            }
        }
    }
    public int GetUrlCount()
    {
        return urlObjectList.Count;
    }

    public string PopUrl()
    {
        string url = "";
        if (urlObjectList.Count == 0)
            return url;
        url = urlObjectList[0].url;
        urlObjectList.RemoveAt(0);
        return url;
    }
    public bool Success
    {
        get;
        set;
    }
    public UpdateType UType
    {
        get;
        set;
    }
    public string Url
    {
        get;
        set;
    }
    public VersionVO ServerVersion
    {
        get;
        set;
    }
    public uint TotalLength
    {
        get;
        set;
    }
}
