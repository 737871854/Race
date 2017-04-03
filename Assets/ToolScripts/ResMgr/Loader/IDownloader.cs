using UnityEngine;
using System.Collections;

namespace Need.Mx
{

    public interface IDownloader
    {
        void StartDown(LoadHelper loadHelper);
        void Clear();
    }
}
