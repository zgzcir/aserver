using PENet;
using Protocol;

public class LoginSys
{
    private static LoginSys instance;
    public static LoginSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LoginSys();
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
        CommonTool.Log("LoginSys Connected");
    }
    public void ReqLogin(MsgPack pack)
    {
        GameMsg msg = new GameMsg()
        {
            cmd = (int) CMD.RspLogin,
            RspLogin = new RspLogin()
        };

        ReqLogin data = pack.Msg.ReqLogin;
        if (cacheSvc.IsAcctOnline(data.acct))
        {
            msg.err = (int) ErrCode.AcctIsOnLine;
        }
        else
        {
            PlayerData pd = cacheSvc.GetPlayerData(data.acct, data.pass);
            if (pd == null)
            {
                msg.err = (int) ErrCode.WrongPass;
            }
            else if(pd.ID==-1)
            {
                msg.err = (int) ErrCode.UpdateDbErr;
            }
            else
            {
                int power = pd.Power;
                long now = timerSvc.GetNowTime();
                long milliseconds = now - pd.Time;
                int addPower = (int) (milliseconds / (1000 * 60 * CommonTool.PowerAddSpace))*CommonTool.PowerAddCount;
                if (addPower > 0)
                {
                    int powerMax = CommonTool.GetPowerLimit(pd.Level);
                    if (pd.Power < powerMax)
                    {
                        pd.Power += powerMax;
                        if (pd.Power > powerMax)
                        {
                            pd.Power = powerMax;
                        }
                    }
                }
                if (power != pd.Power)
                {
                    cacheSvc.UpdatePlayerData(pd.ID, pd);
                }
                
                msg.RspLogin.PlayerData = pd;
                cacheSvc.AcctOnLine(data.acct, pack.Session, pd);
            }
        }
        pack.Session.SendMsg(msg);
    }
    public void ReqReName(MsgPack pack)
    {
        GameMsg msg = new GameMsg()
        {
            cmd = (int) CMD.RspReName
        };
        if (cacheSvc.IsNameExits(pack.Msg.ReqReName.Name))
        {
            msg.err = (int) ErrCode.NameIsExits;
        }
        else
        {
            PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.Session);
            pd.Name = pack.Msg.ReqReName.Name;
            if (cacheSvc.UpdatePlayerData(pd.ID, pd))
            {
                msg.RspReName = new RspReName()
                {
                    Name = pack.Msg.ReqReName.Name
                };
            }
            else
            { 
                msg.err = (int)ErrCode.UpdateDbErr;
            }
        }
        pack.Session.SendMsg(msg);
    }
    public bool ClentOffLineData(ServerSession session)
    {
        PlayerData pd = cacheSvc.GetPlayerDataBySession(session);
        if (pd != null)
        {
            pd.Time = timerSvc.GetNowTime();
            if (!cacheSvc.UpdatePlayerData(pd.ID, pd))
            {
                CommonTool.Log("ClentOffLineData UpdatePlayerData Error",LogTypes.Error);
            }
        }
        return  cacheSvc.AcctOffLine(session);
    }
}