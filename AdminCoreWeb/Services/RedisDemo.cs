using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminCoreWeb.Tool;

namespace AdminCoreWeb.Services
{
    public class RedisDemo
    {
        public static void InitHashData()
        {
            var pipe = RedisHelper.StartPipe();
            for (int i = 0; i < 400000; i++)
            {
                pipe.HMSet("lzclickcache",  Guid.NewGuid(), i.ToString());
            }
            pipe.EndPipe();
        }
    }
}
