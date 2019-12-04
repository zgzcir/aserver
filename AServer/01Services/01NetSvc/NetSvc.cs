using System.Collections.Generic;
using PENet;
using Protocol;
using static AServer.Config;
using static AServer.Enviroment;

public class MsgPack
{
    public ServerSession Session;
    public GameMsg Msg;


    public MsgPack(ServerSession session, GameMsg msg)
    {
        this.Session = session;
        this.Msg = msg;
    }
}

public class NetSvc
{
    private static NetSvc instance;

    public static NetSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NetSvc();
            }

            return instance;
        }
    }

    public static readonly string obj = "lock";
    private Queue<MsgPack> msgPackQue = new Queue<MsgPack>();

    public void init()
    {
        PESocket<ServerSession, GameMsg> server = new PESocket<ServerSession, GameMsg>();
        switch (Env)
        {
            case Dev:
                server.StartAsServer(SrvCfg.devIP, SrvCfg.srvPort);
                break;
            case Production:
                server.StartAsServer(SrvCfg.srvIP, SrvCfg.srvPort);
                break;
        }

        CommonTool.Log("NetSvc Connected");
    }

    public void AddMsgPack(MsgPack pack)
    {
        lock (obj)
        {
            msgPackQue.Enqueue(pack);
        }
    }

    public void Update()
    {
        if (msgPackQue.Count > 0)
        {
            lock (obj)
            {
                MsgPack pack = msgPackQue.Dequeue();
                ProcessMsg(pack);
            }
        }
    }

    private void ProcessMsg(MsgPack pack)
    {
        switch ((CMD) pack.Msg.cmd)
        {
            case CMD.ReqLogin:
                LoginSys.Instance.ReqLogin(pack);
                break;
            case CMD.ReqReName:
                LoginSys.Instance.ReqReName(pack);
                break;
            case CMD.ReqGuide:
                GuideSys.Instance.ReqGuide(pack);
                break;
            case CMD.ReqStrengthen:
                StrengthenSys.Instance.ReqStrengthen(pack);
                break;
            case CMD.SndChat:
                ChatSys.Instance.SndChat(pack);
                break;
            case CMD.ReqTranscation:
                TranscationSys.Instance.ReqTranscation(pack);
                break;
            case CMD.ReqTakeTaskReward:
                TaskRewardSys.Instance.ReqTakeTaskReward(pack);
                break;
            case CMD.ReqMission:
                MissionSys.Instance.ReqMission(pack);
                break;
        }
    }
}