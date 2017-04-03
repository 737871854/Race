using System;
using System.IO;
using UnityEngine;
using System.Collections;
using JsonFx.Json;
using System.Text;

public class HelperTool
{
    public static string DoTimeFormat(int flag, int time)
    {
        string str;
        int hour = 0;
        int minute = 0;
        int second = 0;
        second = time;
        if(second>60)
        {
            minute = second / 60;
            second %= 60;
        }
        if(minute > 60)
        {
            hour = minute / 60;
            minute %= 60;
        }
        switch(flag)
        {
            case 0:
                str = (hour.ToString()+"小时"+minute.ToString()+"分钟");
                break;
            case 1:
                str =  (hour.ToString()+" h "+minute.ToString()+" m");
                break;
            default:
                str = (hour.ToString()+"小时"+minute.ToString()+"分钟");
                break;
        }
        return str;
    }


    public static void SaveJson(SettingConfigData data, string path)
    {
        string str = JsonWriter.Serialize(data);
        if (File.Exists(path))
            File.Delete(path);

        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 8192, FileOptions.WriteThrough);
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        fs.Write(bytes, 0, bytes.Length);
        fs.Flush();
        fs.Close();
        fs.Dispose();

    }
    public static SettingConfigData LoadJson(string path)
    {
        SettingConfigData data = new SettingConfigData();
        if (!File.Exists(path))
            return null;
        string text = File.ReadAllText(path);

        if (text.Length == 0)
        {
            File.Delete(path);
            return null;
        }
        try
        {
            data = JsonReader.Deserialize<SettingConfigData>(text);
        }
        catch (Exception e)
        {
            File.Delete(path);
        }

        return data;
    }
}
