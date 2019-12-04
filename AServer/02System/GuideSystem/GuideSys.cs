using Protocol;

public class GuideSys
{
    private static GuideSys instance;

    public static GuideSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GuideSys();
            }

            return instance;
        }
    }

    private CacheSvc cacheSvc;
    private CfgSvc cfgSvc;

    public void init()
    {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        CommonTool.Log("GuideSys Connected");
    }

    public void ReqGuide(MsgPack pack)
    {
        ReqGuide data = pack.Msg.ReqGuide;
        GameMsg msg = new GameMsg()
        {
            cmd = (int) CMD.RspGuide,
        };
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.Session);
        GuideCfg cfg = cfgSvc.GetGuideCfg(pd.GuideID);
        if (pd.GuideID == data.id)
        {
            //task
            if (pd.GuideID == 1001)
            {
                TaskRewardSys.Instance.CalcTaskPrgs(pd,1);
            }

            pd.GuideID++;
            pd.Coin += cfg.Coin;
            CommonTool.CalcExp(pd, cfg.Exp);
            if (!cacheSvc.UpdatePlayerData(pd.ID, pd))
            {
                msg.err = (int) ErrCode.UpdateDbErr;
            }
            else
            {
                msg.RspGuide = new RspGuide()
                {
                    id = pd.GuideID,
                    coin = pd.Coin,
                    lv = pd.Level,
                    exp = pd.Exp
                };
            }
        }
        else
        {
            msg.err = (int) ErrCode.ServerDataError;
        }

        pack.Session.SendMsg(msg);
    }
}