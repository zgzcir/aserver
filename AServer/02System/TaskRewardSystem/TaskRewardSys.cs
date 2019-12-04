using System;
using System.Security.Cryptography;
using Protocol;

public class TaskRewardSys
{
    private static TaskRewardSys instance;

    public static TaskRewardSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TaskRewardSys();
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
        CommonTool.Log("PowerSys Connected");
    }

    public void ReqTakeTaskReward(MsgPack pack)
    {
        ReqTakeTaskReward data = pack.Msg.ReqTakeTaskReward;
        GameMsg msg = new GameMsg()
            {
                cmd = (int) CMD.RspTakeTaskReward,
            }
            ;
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.Session);
        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(data.ID);
        TaskRewardData trd = CalcTaskRewardData(pd, data.ID);
        if (trd.Prgs == trc.Count && trd.IsTaked == false)
        {
            pd.Coin += trc.Coin;
            CommonTool.CalcExp(pd, trc.Exp);
            trd.IsTaked = true;
            CalcTaskArr(pd, trd);

            if (!cacheSvc.UpdatePlayerData(pd.ID, pd))
            {
                msg.err = (int) ErrCode.UpdateDbErr;
            }
            else
            {
                msg.RspTakeTaskReward = new RspTakeTaskReward()
                {
                    Coin = pd.Coin,
                    Lv = pd.Level,
                    Exp = pd.Exp,
                    TaskRewardArr = pd.TaskRewardArr
                };
            }
        }
        else
        {
            msg.err = (int) ErrCode.ClientDataErr;
        }

        pack.Session.SendMsg(msg);
    }

    public TaskRewardData CalcTaskRewardData(PlayerData pd, int id)
    {
        TaskRewardData trd = null;
        for (int i = 0; i < pd.TaskRewardArr.Length; i++)
        {
            string[] taskInfo = pd.TaskRewardArr[i].Split('|');
            if (int.Parse(taskInfo[0]) == id)
            {
                trd = new TaskRewardData()
                {
                    ID = id,
                    Prgs = int.Parse(taskInfo[1]),
                    IsTaked = taskInfo[2].Equals("1")
                };
                break;
            }
        }

        return trd;
    }

    public void CalcTaskArr(PlayerData pd, TaskRewardData trd)
    {
        string result = trd.ID + "|" + trd.Prgs+ "|" + (trd.IsTaked ? 1 : 0);
        int index = -1;
        for (int i = 0; i < pd.TaskRewardArr.Length; i++)
        {
            string[] taskInfo = pd.TaskRewardArr[i].Split('|');
            if (int.Parse(taskInfo[0]) == trd.ID)
            {
                index = i;
                break;
            }
        }

        pd.TaskRewardArr[index] = result;
    }

    public void CalcTaskPrgs(PlayerData pd, int id)
    {
        TaskRewardData trd = CalcTaskRewardData(pd, id);
        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(id);
        if (trd.Prgs < trc.Count)
        {
            trd.Prgs++;
            CalcTaskArr(pd, trd);
            var session = cacheSvc.GetOnLineSession(pd.ID);
            if (session != null)
            {
                session.SendMsg(new GameMsg()
                {
                    cmd = (int) CMD.PshTaskPrgs,
                    PshTaskPrgs = new PshTaskPrgs()
                    {
                        TaskRewardArr = pd.TaskRewardArr
                    }
                });
            }
        }
    }
    
    public PshTaskPrgs GetTaskPrgs(PlayerData pd, int id)
    {
        TaskRewardData trd = CalcTaskRewardData(pd, id);
        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(id);
        if (trd.Prgs < trc.Count)
        {
            trd.Prgs++;
            CalcTaskArr(pd, trd);

            return new PshTaskPrgs()
            {
                TaskRewardArr = pd.TaskRewardArr
            };
        }
        return null;
    }
}