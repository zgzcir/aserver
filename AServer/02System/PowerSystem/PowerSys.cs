using Protocol;

public class PowerSys
{
    private static PowerSys instance;
    public static PowerSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PowerSys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc;
    private TimerSvc timerSvc;

    public void init()
    {
        cacheSvc = CacheSvc.Instance;
        timerSvc = TimerSvc.Instance;
        TimerSvc.Instance.AddTimeTask(CalcPowerAdd, CommonTool.PowerAddSpace, TimeUnit.Minute, 0);
        CommonTool.Log("PowerSys Connected");
    }
    private void CalcPowerAdd(int tid)
    {
        CommonTool.Log("All Online Add power....");
        GameMsg msg = new GameMsg()
        {
            cmd = (int) CMD
                .PshPower,
            PshPower = new PshPower()
        };
        var onlineDIc = cacheSvc.GetOnlineCache();
        foreach (var item in onlineDIc)
        {
            ServerSession session = item.Key;
            PlayerData pd = item.Value;
            int powerMax = CommonTool.GetPowerLimit(pd.Level);
            if (pd.Power >= powerMax)
            {
                continue;
            }

            pd.Time = timerSvc.GetNowTime();
            pd.Power += CommonTool.PowerAddCount;
            if (pd.Power >= powerMax)
            {
                pd.Power = powerMax;
            }

            if (!cacheSvc.UpdatePlayerData(pd.ID, pd)
            )
            {
                msg.err = (int) ErrCode.UpdateDbErr;
            }
            else
            {
                msg.PshPower.power = pd.Power;
                session.SendMsg(msg);
            }
        }
    }
}