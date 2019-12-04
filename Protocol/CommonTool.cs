using System;
using PENet;
using Protocol;

public enum LogTypes
{
    Log = 0,
    Warn = 1,
    Error = 2,
    Info = 3
}
public class CommonTool 
{
    public static void Log(string msg = "",LogTypes lt=LogTypes.Log)
    {
        LogLevel lv = (LogLevel) lt;
        PETool.LogMsg(msg,lv);
    }
    public static void Log(object o,LogTypes lt=LogTypes.Log)
    {
        LogLevel lv = (LogLevel) lt;
        PETool.LogMsg(o.ToString(),lv);
    }
    public static void DataBaseErrLog(string methodName,Exception e)
    {
        Log("when"+methodName+"Error:"+e,LogTypes.Error);
    }
    public static int GetPowerLimit(int lv)
    {
        return ((lv - 1) / 10) * 150 + 150;
    }
    public static int CalcuEvaluation(PlayerData playerData)
    {
        return playerData.Level * 100 + playerData.PA + playerData.SA + playerData.PD + playerData.SD;
    }
    public static int GetExpUpValue(int lv)
    {
        return lv * 100 * lv;
    }

    public const int PowerAddSpace = 5;
    public const int PowerAddCount = 1;
    
    public static void CalcExp(PlayerData pd, int addExp)
    {
        int curLv = pd.Level;
        int curExp = pd.Exp;
        int addRestExp = addExp;

        while (true)
        {
            int upNeedExp = CommonTool.GetExpUpValue(curLv) - curExp;
            if (addRestExp >= upNeedExp)
            {
                curLv++;
                curExp = 0;
                addRestExp -= upNeedExp;
            }
            else
            {
                pd.Level = curLv;
                pd.Exp = addRestExp;
                break;
            }
        }
    }
}