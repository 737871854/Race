using UnityEngine;
using System.Collections;
namespace Update
{
    public interface ILoadHelper
    {
        DownFileVO FileVO { get;set;}
        WWW WWWObj { get; set; }
    }
}
