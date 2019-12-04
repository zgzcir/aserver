using System.Collections.Generic;
using System.Linq;
using Protocol;

public class CacheSvc
{
    private static CacheSvc instance;

    public static CacheSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CacheSvc();
            }

            return instance;
        }
    }

    private DBMgr dbMgr;

    private Dictionary<string, ServerSession> onLineAcctDic = new Dictionary<string, ServerSession>();
    private Dictionary<ServerSession, PlayerData> onLineSessionDic = new Dictionary<ServerSession, PlayerData>();

    public void Init()
    {
        dbMgr = DBMgr.Instance;
        CommonTool.Log("CacheSvc Connected");
    }

    public bool IsAcctOnline(string acct)
    {
        return onLineAcctDic.ContainsKey(acct);
    }

    public PlayerData GetPlayerData(string acct, string pass)
    {
        return dbMgr.QueryPlayerData(acct, pass);
    }

    public PlayerData GetPlayerDataBySession(ServerSession session)
    {
        if (onLineSessionDic.TryGetValue(session, out PlayerData playerData))
        {
            return playerData;
        }

        return null;
    }

    public bool UpdatePlayerData(int id, PlayerData pd)
    {
        return dbMgr.UpdatePlayerData(id, pd);
    }

    public void AcctOnLine(string acct, ServerSession serverSession, PlayerData playerData)
    {
        onLineAcctDic.Add(acct, serverSession);
        onLineSessionDic.Add(serverSession, playerData);
    }

    public bool IsNameExits(string name)
    {
        return dbMgr.QueryNameData(name);
    }

    public List<ServerSession> GetOnLineSessions()
    {
        List<ServerSession> sessions=new List<ServerSession>();
        foreach (var item in onLineSessionDic)
        {
            sessions.Add(item.Key);
        }
        return sessions;
    }

    public ServerSession GetOnLineSession(int id)
    {
        ServerSession session = null;

        foreach (var s in onLineSessionDic)
        {
            if (s.Value.ID == id)
            {
                session = s.Key;
                break;
            }
        }

        return session;
    }

    public bool AcctOffLine(ServerSession session)
    {
        foreach (var item in onLineAcctDic)
        {
            if (item.Value.sessionId.Equals(session.sessionId))
            {
                onLineAcctDic.Remove(item.Key);
            }
            break;
        }
        bool isSuccess = onLineSessionDic.Remove(session);
        return isSuccess;
    }

    public Dictionary<ServerSession, PlayerData> GetOnlineCache()
    {
        return onLineSessionDic;
    }
}