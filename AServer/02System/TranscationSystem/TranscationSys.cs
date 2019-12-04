using Protocol;

public class TranscationSys
{
    private static TranscationSys instance;

    public static TranscationSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TranscationSys();
            }

            return instance;
        }
    }

    private CacheSvc cacheSvc;

    public void init()
    {
        cacheSvc = CacheSvc.Instance;
        CommonTool.Log("TranscationSys Connected");
    }

    public void ReqTranscation(MsgPack pack)
    {
        var data = pack.Msg.ReqTranscation;
        GameMsg msg = new GameMsg()
        {
            cmd = (int) CMD.RspTranscation
        };
        var pd = cacheSvc.GetPlayerDataBySession(pack.Session);
        if (pd.Diamond < data.cost)
        {
            msg.err = (int) ErrCode.LackDiamond;
        }
        else
        {
            pd.Diamond -= data.cost;
            PshTaskPrgs pshTaskPrgs = null;
            switch (data.type)
            {
                case 0:

                    pd.Power += 100;
                    pshTaskPrgs = TaskRewardSys.Instance.GetTaskPrgs(pd, 4);
                    break;

                case 1:
                    pd.Coin += 1000;
                    pshTaskPrgs = TaskRewardSys.Instance.GetTaskPrgs(pd, 5);
                    break;
            }

            if (!cacheSvc.UpdatePlayerData(pd.ID, pd))
            {
                msg.err = (int) ErrCode.UpdateDbErr;
            }
            else
            {
                msg.RspTranscation = new RspTranscation()
                {
                    type = data
                        .type,
                    diamond = pd.Diamond,
                    coin = pd.Coin,
                    power = pd.Power
                };
                msg.PshTaskPrgs = pshTaskPrgs;//并包优化
            }
        }
        pack.Session.SendMsg(msg);
    }
}