using System;
using System.Threading;

public class ServerStart
    {
        public static void Main(string []args)
        {
            if (args.Length == 0)
            {
                CommonTool.Log("请输入启动参数");
                return;
            }
            ServerRoot.Instance.init(args);
            while (true)
            {
                ServerRoot.Instance.Update();
                Thread.Sleep(20);
            }
        }
        
    }
