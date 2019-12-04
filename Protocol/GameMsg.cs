using System;
using System.ComponentModel;
using PENet;

namespace Protocol
{
    [Serializable]
    public class GameMsg :PEMsg
    {
        public ReqLogin ReqLogin;
        public RspLogin RspLogin;
        public ReqReName ReqReName;
        public RspReName RspReName;
        public ReqGuide ReqGuide;
        public RspGuide RspGuide;
        public ReqStrengthen ReqStrengthen;
        public RspStrengthen RspStrengthen;
        public SndChat SndChat;
        public PshChat PshChat;
        public ReqTranscation ReqTranscation;
        public RspTranscation RspTranscation;
        public PshPower PshPower;
        public ReqTakeTaskReward ReqTakeTaskReward;
        public RspTakeTaskReward RspTakeTaskReward;
        public PshTaskPrgs PshTaskPrgs;
        public ReqMission ReqMission;
        public RspMission RspMission;
    }


    #region login

    [Serializable]
    public class ReqLogin
    {
        public string acct;
        public string pass;
    }

    [Serializable]
    public class RspLogin
    {
        public PlayerData PlayerData;
    }

    [Serializable]
    public class PlayerData
    {
        public int ID;
        public string Name;
        public int Level;
        public int Exp;
        public int Power;
        public int HP;
        public int Coin;
        public int Diamond;
        public int Crystal;

        public int PA;
        public int SA;
        public int PD;
        public int SD;

        public int Dodge; //闪避
        public int Pierce; //穿透
        public int Critical; //暴击率

        public int GuideID;
        public int[] StrenArr;
        public long Time;
        public string[] TaskRewardArr;
        public int Mission;

        public string Kanban;
    }

    [Serializable]
    public class ReqReName
    {
        public string Name;
    }

    [Serializable]
    public class RspReName
    {
        public string Name;
    }

    #endregion


    #region task

    [Serializable]
    public class ReqGuide
    {
        public int id;
    }

    [Serializable]
    public class RspGuide
    {
        public int id;
        public int coin;
        public int exp;
        public int lv;
    }

    #endregion

    #region strenthen

    [Serializable]
    public class ReqStrengthen
    {
        public int pos;
    }

    [Serializable]
    public class RspStrengthen
    {
        public int coin;
        public int crystal;
        public int hp;
        public int sa;
        public int pa;
        public int sd;
        public int pd;
        public int[] strenarr;
    }

    #endregion

    #region chat

    [Serializable]
    public class SndChat
    {
        public string msg;
    }

    [Serializable]
    public class PshChat
    {
        public string name;
        public string msg;
    }

    #endregion

    #region transcation

    [Serializable]
    public class ReqTranscation
    {
        public int type;
        public int cost;
    }

    [Serializable]
    public class RspTranscation
    {
        public int type;
        public int diamond;
        public int coin;
        public int power;
    }

    #endregion

    #region power

    [Serializable]
    public class PshPower
    {
        public int power;
    }

    #endregion

    #region taskreward

    [Serializable]
    public class ReqTakeTaskReward
    {
        public int ID;
    }

    [Serializable]
    public class RspTakeTaskReward
    {
        public int Coin;
        public int Lv;
        public int Exp;
        public string[] TaskRewardArr;
    }

    [Serializable]
    public class PshTaskPrgs
    {
        public string[] TaskRewardArr;
    }

    #endregion

    #region mission

    [Serializable]
    public class ReqMission
    {
        public int MID;
    }

    [Serializable]
    public class RspMission
    {
        public int Power;
        public int MID;
    }

    #endregion

    public enum CMD
    {
        None = 0,

        //login
        ReqLogin = 101,
        RspLogin = 102,
        ReqReName = 103,
        RspReName = 104,

        ReqGuide = 201,
        RspGuide = 202,
        ReqStrengthen = 203,
        RspStrengthen = 204,
        SndChat = 205,
        PshChat = 206,
        ReqTranscation = 207,
        RspTranscation = 208,
        PshPower = 209,
        ReqTakeTaskReward = 210,
        RspTakeTaskReward = 211,
        PshTaskPrgs = 212,

        ReqMission = 301,
        RspMission = 302
    }

    public enum ErrCode


    {
        None = 0,

        ServerDataError,

        AcctIsOnLine,
        WrongPass,

        NameIsExits,
        UpdateDbErr,
        ClientDataErr,

        LackLevel,
        LackCoin,
        LackCrystal,
        LackDiamond,
        LackPower
    }

    //public class SrvCfg
    //{
    //    public const string srvIP = "127.0.0.1";
    //    public const int srvPort = 17666;
    //}
    public class SrvCfg
    {
        public const string devIP = "127.0.0.0";
        public const string srvIP = "172.18.177.108";
        public const string srvIPClient = "47.106.254.223";
        public const int srvPort = 17666;
    }
}