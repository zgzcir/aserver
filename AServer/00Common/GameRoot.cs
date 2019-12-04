using System;

public class GameRoot
{
    private static GameRoot instance;

    public static GameRoot Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameRoot();
            }

            return instance;
        }
    }


    public void init(params object[] args)

    {

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
        CommonTool.Log("Start...");
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