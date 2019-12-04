using System;
using MySql.Data.MySqlClient;
using Protocol;
using System.Reflection;

public class DBMgr
{
    private static DBMgr instance;

    public static DBMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DBMgr();
            }

            return instance;
        }
    }

    private MySqlConnection conn;

    public void Init()
    {
        conn = new MySqlConnection(
            "server=localhost;User Id=root;password='a123456789...';Database=arpg;Charset=utf8");
        conn.Open();
        CommonTool.Log("DBMgr Connected");
    }

    public PlayerData QueryPlayerData(string acct, string pass)
    {
        PlayerData pd = null;
        MySqlDataReader reader = null;
        bool isNew = true;
        try
        {
            MySqlCommand cmd = new MySqlCommand("Select * from account where acct=@acct", conn);
            cmd.Parameters.AddWithValue("acct", acct);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                isNew = false;
                string _pass = reader.GetString("pass");
                if (_pass.Equals(pass))
                {
                    pd = new PlayerData()
                    {
                        ID = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                        Coin = reader.GetInt32("coin"),
                        Power = reader.GetInt32("power"),
                        Diamond = reader.GetInt32("diamond"),
                        Crystal = reader.GetInt32("crystal"),

                        HP = reader.GetInt32("hp"),
                        Exp = reader.GetInt32("exp"),
                        Level = reader.GetInt32("level"),
                        PD = reader.GetInt32("pd"),
                        SD = reader.GetInt32("sd"),
                        PA = reader.GetInt32("pa"),
                        SA = reader.GetInt32("sa"),
                        Dodge = reader.GetInt32("dodge"),
                        Pierce = reader.GetInt32("pierce"),
                        Critical = reader.GetInt32("critical"),
                        GuideID = reader.GetInt32("guideid"),
                        Time = reader.GetInt64("time"),
                        Mission = reader.GetInt32("mission"),
                        
                        Kanban = reader.GetString("kanban")
                    
                    };

                    #region stren

                    string[] strenStrArr = reader.GetString("strenarr").Split('#');
                    int[] strenArr = new int[6];
                    for (int i = 0; i < strenStrArr.Length; i++)
                    {
                        if (strenStrArr[i] == "")
                        {
                            continue;
                        }

                        if (int.TryParse(strenStrArr[i], out int starLV))
                        {
                            strenArr[i] = starLV;
                        }
                        else
                        {
                            CommonTool.Log("Parse strenarr Data Erroe", LogTypes.Error);
                        }
                    }

                    pd.StrenArr = strenArr;

                    #endregion

                    #region taskreward

                    string[] taskRewardStrArr = reader.GetString("taskrewardarr").Split('#');
                    pd.TaskRewardArr = new string[6]; //暂时写6
                    for (int i = 0; i < taskRewardStrArr.Length; i++)
                    {
                        if (taskRewardStrArr[i] == "")
                        {
                            continue;
                        }

                        if (taskRewardStrArr[i].Length >= 5)
                        {
                            pd.TaskRewardArr[i] = taskRewardStrArr[i];
                        }
                        else
                        {
                            throw new Exception("DataError");
                        }
                    }

                    #endregion
                }
            }
        }
        catch (Exception e)
        {
            CommonTool.Log("QueryPlayerData Error:" + e, LogTypes.Error);
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }

            if (isNew)
            {
                pd = new PlayerData()
                {
                    ID = -1,
                    Name = "",
                    Level = 1,
                    Exp = 0,
                    HP = 100,
                    Coin = 54684680,
                    Power = 100,
                    Diamond = 2000,
                    Crystal = 25550,
                    PD = 100,
                    SD = 100,
                    PA = 100,
                    SA = 100,
                    Dodge = 7,
                    Pierce = 5,
                    Critical = 2,
                    GuideID = 1001,
                    StrenArr = new int[6],
                    Time = TimerSvc.Instance.GetNowTime(),
                    TaskRewardArr = new string[6],
                    Mission = 1001,
                    Kanban = "chen"
                };
                for (int i = 0; i < pd.TaskRewardArr.Length; i++)
                {
                    pd.TaskRewardArr[i] = (i + 1) + "|0|0";
                }

                pd.ID = InsertNewPlayerData(acct, pass, pd);
            }
        }

        return pd;
    }

    private int InsertNewPlayerData(string acct, string pass, PlayerData playerData)
    {
        int id = -1;
        try
        {
            MySqlCommand cmd =
                new MySqlCommand(
                    "Insert into account set acct=@acct,pass=@pass,name=@name,hp=@hp,coin=@coin,power=@power,diamond=@diamond,crystal=@crystal,level=@level,exp=@exp,pd=@pd,sd=@sd,pa=@pa,sa=@sa,dodge=@dodge,pierce=@pierce,critical=@critical,guideid=@guideid,strenarr=@strenarr,time=@time,taskrewardarr=@taskrewardarr,mission=@mission,kanban=@kanban",
                    conn);
            cmd.Parameters.AddWithValue("acct", acct);
            cmd.Parameters.AddWithValue("pass", pass);
            cmd.Parameters.AddWithValue("name", playerData.Name);
            cmd.Parameters.AddWithValue("hp", playerData.HP);
            cmd.Parameters.AddWithValue("coin", playerData.Coin);
            cmd.Parameters.AddWithValue("power", playerData.Power);
            cmd.Parameters.AddWithValue("diamond", playerData.Diamond);
            cmd.Parameters.AddWithValue("crystal", playerData.Crystal);
            cmd.Parameters.AddWithValue("level", playerData.Level);
            cmd.Parameters.AddWithValue("exp", playerData.Exp);
            cmd.Parameters.AddWithValue("pd", playerData.PD);
            cmd.Parameters.AddWithValue("sd", playerData.SD);
            cmd.Parameters.AddWithValue("pa", playerData.PA);
            cmd.Parameters.AddWithValue("sa", playerData.SA);
            cmd.Parameters.AddWithValue("dodge", playerData.Dodge);
            cmd.Parameters.AddWithValue("pierce", playerData.Pierce);
            cmd.Parameters.AddWithValue("critical", playerData.Critical);
            cmd.Parameters.AddWithValue("guideid", playerData.GuideID);
            cmd.Parameters.AddWithValue("kanban", playerData.Kanban);
            
            string strenInfo = "";
            for (int i = 0; i < playerData.StrenArr.Length; i++)
            {
                strenInfo += playerData.StrenArr[i];
                strenInfo += "#";
            }

            cmd.Parameters.AddWithValue("strenarr", strenInfo);
            cmd.Parameters.AddWithValue("time", playerData.Time);
            string taskInfo = "";
            for (int i = 0; i < playerData.TaskRewardArr.Length; i++)
            {
                taskInfo += playerData.TaskRewardArr[i];
                taskInfo += "#";
            }

            cmd.Parameters.AddWithValue("taskrewardarr", playerData.TaskRewardArr);
            cmd.Parameters.AddWithValue("mission", playerData.Mission);

            cmd.ExecuteNonQuery();
            id = (int) cmd.LastInsertedId;
  
        }
        catch (Exception e)
        {
            CommonTool.DataBaseErrLog(MethodBase.GetCurrentMethod().Name, e);
        }

        return id;
    }

    public bool QueryNameData(string name)
    {
        bool exits = false;
        MySqlDataReader reader = null;
        try
        {
            MySqlCommand sql = new MySqlCommand("Select name from account where name=@name", conn);
            sql.Parameters.AddWithValue("name", name);
            reader = sql.ExecuteReader();
            if (reader.Read())
            {
                exits = true;
            }
        }
        catch (Exception e)
        {
            CommonTool.DataBaseErrLog(MethodBase.GetCurrentMethod().Name, e);
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }
        }

        return exits;
    }

    public bool UpdatePlayerData(int id, PlayerData pd)
    {
        try
        {
            MySqlCommand cmd =
                new MySqlCommand(
                    "update account  set name=@name,hp=@hp,coin=@coin,power=@power,diamond=@diamond,crystal=@crystal,level=@level,exp=@exp,pd=@pd,sd=@sd,pa=@pa,sa=@sa,dodge=@dodge,pierce=@pierce,critical=@critical,guideid=@guideid,strenarr=@strenarr,time=@time,taskrewardarr=@taskrewardarr,mission=@mission ,kanban=@Kanban where id =@id",
                    conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("name", pd.Name);
            cmd.Parameters.AddWithValue("hp", pd.HP);
            cmd.Parameters.AddWithValue("coin", pd.Coin);
            cmd.Parameters.AddWithValue("power", pd.Power);
            cmd.Parameters.AddWithValue("diamond", pd.Diamond);
            cmd.Parameters.AddWithValue("crystal", pd.Crystal);
            cmd.Parameters.AddWithValue("level", pd.Level);
            cmd.Parameters.AddWithValue("exp", pd.Exp);
            cmd.Parameters.AddWithValue("pd", pd.PD);
            cmd.Parameters.AddWithValue("sd", pd.SD);
            cmd.Parameters.AddWithValue("pa", pd.PA);
            cmd.Parameters.AddWithValue("sa", pd.SA);
            cmd.Parameters.AddWithValue("dodge", pd.Dodge);
            cmd.Parameters.AddWithValue("pierce", pd.Pierce);
            cmd.Parameters.AddWithValue("critical", pd.Critical);
            cmd.Parameters.AddWithValue("guideid", pd.GuideID);
            cmd.Parameters.AddWithValue("kanban", pd.Kanban);
            string strenInfo = "";
            for (int i = 0; i < pd.StrenArr.Length; i++)
            {
                strenInfo += pd.StrenArr[i];
                strenInfo += "#";
            }

            cmd.Parameters.AddWithValue("strenarr", strenInfo);
            cmd.Parameters.AddWithValue("time", pd.Time);

            string taskInfo = "";
            for (int i = 0; i < pd.TaskRewardArr.Length; i++)
            {
                taskInfo += pd.TaskRewardArr[i];
                taskInfo += "#";
            }
            cmd.Parameters.AddWithValue("taskrewardarr", taskInfo);
            cmd.Parameters.AddWithValue("mission", pd.Mission);

            cmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            CommonTool.DataBaseErrLog(MethodBase.GetCurrentMethod().Name, e);
            return false;
        }

        return true;
    }
}