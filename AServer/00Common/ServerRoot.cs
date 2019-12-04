using System;
using static AServer.Config;
using static AServer.Enviroment;

public class ServerRoot
{
    private static ServerRoot instance;

    public static ServerRoot Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ServerRoot();
            }

            return instance;
        }
    }


    public void init(params object[] args)

    {
        switch (args[0])
        {
            case "dev":
                Env = Dev;
                break;
            case "prod":
                Env = Production;
                break;
        }

        DBMgr.Instance.Init();
        //svc
        CacheSvc.Instance.Init();
        NetSvc.Instance.init();
        CfgSvc.Instance.init();
        TimerSvc.Instance.init();
        //sys
        LoginSys.Instance.init();
        GuideSys.Instance.init();
        StrengthenSys.Instance.init();
        ChatSys.Instance.init();
        TranscationSys.Instance.init();
        PowerSys.Instance.init();
        TaskRewardSys.Instance.init();
        MissionSys.Instance.init();
        CommonTool.Log($"now the server is runing in {Env}");
    }

    public void Update()
    {
        NetSvc.Instance.Update();
        TimerSvc.Instance.Update();
    }

    private int sessionId;

    public int GetSessionID()
    {
        if (sessionId == int.MaxValue)
        {
            sessionId = 0;
        }

        return sessionId++;
    }
}