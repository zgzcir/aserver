using System;
using System.Threading;

public class GameStart
    {
        public static void Main(string []args)
        {
            GameRoot.Instance.init();
            while (true)
            {
                GameRoot.Instance.Update();
                Thread.Sleep(20);
            }
        }
        
    }
