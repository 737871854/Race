using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Update.Factory
{
    public class UpdateFactory
    {
        public static HttpLoad httpLoad
        {
            get;
            private set;
        }
        public static TcpLoad tcpLoad
        {
            get;
            private set;
        }
        /// <summary>
        /// 创建httpLoad
        /// </summary>
        /// <returns></returns>
        public static ILoad CreateHttpLoad(UpdateEventHandler errorHandler=null) {
            if (httpLoad == null)
            {
                httpLoad = new HttpLoad();
                httpLoad.SetEventHandler(errorHandler);
            }
            return httpLoad;
        }
        public static ILoad CreateTcpLoad(UpdateEventHandler errorHandler = null)
        {
            if (tcpLoad == null)
            {
                tcpLoad = new TcpLoad();
                tcpLoad.SetEventHandler(errorHandler);
            }
            return tcpLoad;
        }
    }
}
