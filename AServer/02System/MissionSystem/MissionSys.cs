using Protocol;

public class MissionSys
{
    private static MissionSys instance;

    public static MissionSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MissionSys();
            }

            return instance;
        }
    }

    private CacheSvc cacheSvc;

    public void init()
    {
        cacheSvc = CacheSvc.Instance;
        CommonTool.Log("MissionSys Connected");
    }

    public void ReqMission(MsgPack pack)
    {
        var data = pack.Msg.ReqMission;

        int mid = data.MID;

        GameMsg msg = new GameMsg()
        {
            cmd = (int) CMD.RspMission
        };

        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.Session);
        int powerNeed = CfgSvc.Instance.GetMapCfgData(mid).Power;

        if (pd.Mission < data.MID)
        {
            msg.err = (int) ErrCode.ClientDataErr;
        }
        else if (pd.Power < powerNeed)
        {
            msg.err = (int) ErrCode.LackPower;
        }
        else
        {
            pd.Power -= powerNeed;
            if (cacheSvc.UpdatePlayerData(pd.ID, pd))
            {
                msg.RspMission = new RspMission()
                {
                    MID = mid,
                    Power = pd.Power
                };
            }
            else
            {
                msg.err = (int) ErrCode.UpdateDbErr;
            }
        }

        pack.Session.SendMsg(msg);
    }
}