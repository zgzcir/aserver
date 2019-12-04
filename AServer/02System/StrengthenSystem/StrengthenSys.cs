using System;
using System.Diagnostics.Eventing.Reader;
using Protocol;

public class StrengthenSys
{
    private static StrengthenSys instance;
    private CacheSvc cacheSvc;
    private CfgSvc cfgSvc;

    public static StrengthenSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new StrengthenSys();
            }

            return instance;
        }
    }

    public void init()
    {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        CommonTool.Log("StrengthenSys Connected");
    }

    public void ReqStrengthen(MsgPack pack)
    {
        ReqStrengthen data = pack.Msg.ReqStrengthen;
        GameMsg msg = new GameMsg()
        {
            cmd = (int) CMD.RspStrengthen,
        };

        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.Session);
        int curStarLv = pd.StrenArr[data.pos];
        StrengthenCfg nextSc = cfgSvc.GetStrengthenCfg(data.pos, curStarLv + 1);
        if (pd.Level < nextSc.MinLv)
        {
            msg.err = (int) ErrCode.LackLevel;
        }
        else if (pd.Coin < nextSc.Coin)
        {
            msg.err = (int) ErrCode.LackCoin;
        }
        else if (pd.Crystal < nextSc.Crystal)
        {
            msg.err = (int) ErrCode.LackCrystal;
        }
        else
        {
            TaskRewardSys.Instance.CalcTaskPrgs(pd, 3);
            pd.Coin -= nextSc.Coin;
            pd.Crystal -= nextSc.Crystal;

            pd.StrenArr[data.pos] += 1;

            pd.HP += nextSc.AddHP;
            pd.PA += nextSc.AddHurt;
            pd.SA += nextSc.AddHurt;
            pd.PD += nextSc.AddDef;
            pd.SD += nextSc.AddDef;
            if (!cacheSvc.UpdatePlayerData(pd.ID, pd))
            {
                msg.err = (int) ErrCode.UpdateDbErr;
            }
            else
            {
                msg.RspStrengthen = new RspStrengthen()
                {
                    coin = pd.Coin,
                    crystal = pd.Crystal,
                    hp = pd.HP,
                    sa = pd.SA,
                    pa = pd.PA,
                    pd = pd.PD,
                    sd = pd.SD,
                    strenarr = pd.StrenArr
                };
            }
        }

        pack.Session.SendMsg(msg);
    }
}