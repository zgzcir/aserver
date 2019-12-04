using System.Xml;
using Protocol;

public class ChatSys
{
    private static ChatSys instance;

    public static ChatSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ChatSys();
            }

            return instance;
        }
    }

    private CacheSvc cacheSvc;

    public void init()
    {
        cacheSvc = CacheSvc.Instance;
        CommonTool.Log("ChatSys Connected");
    }

    public void SndChat(MsgPack pack)
    {
        var data = pack.Msg.SndChat;
        var pd = cacheSvc.GetPlayerDataBySession(pack.Session);

        TaskRewardSys.Instance.CalcTaskPrgs(pd, 6);


        var msg = new GameMsg
        {
            cmd = (int) CMD.PshChat,
            PshChat = new PshChat()
            {
                name = pd.Name,
                msg = data.msg
            }
        };
        byte[] bytes = PENet.PETool.PackNetMsg(msg);
        cacheSvc.GetOnLineSessions().ForEach(s => { s.SendMsg(bytes); });
    }
}