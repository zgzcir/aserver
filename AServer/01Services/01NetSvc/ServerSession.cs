using System.Net.Mime;
using PENet;
using Protocol;

public class ServerSession : PESession<GameMsg>
{
    public int sessionId;

    protected override void OnConnected()
    {
        base.OnConnected();
        sessionId = GameRoot.Instance.GetSessionID();
        CommonTool.Log($"*{sessionId}  Client Connected*");
    }

    protected override void OnReciveMsg(GameMsg msg)
    {
        CommonTool.Log("RcvPack CMD:" + ((CMD) msg.cmd)+"from>>>"+sessionId);
        NetSvc.Instance.AddMsgPack(new MsgPack(this, msg));
    }
        
    protected override void OnDisConnected()
    {
        base.OnDisConnected();
        CommonTool.Log($"*{sessionId}  Client DisConnected*");
        if (LoginSys.Instance.ClentOffLineData(this))
        {
            CommonTool.Log($"*{sessionId} Connected*");
        }
        else
        {
            CommonTool.Log($"*{sessionId}  Data Clean Error*",LogTypes.Error);
        }
    }
}      