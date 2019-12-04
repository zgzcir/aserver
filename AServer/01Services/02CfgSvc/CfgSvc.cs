using System;
using System.Collections.Generic;
using System.Xml;
using MySql.Data;



public class MapCfg : BaseData<MapCfg>
{
    public int Power;
}

public class GuideCfg : BaseData<GuideCfg>
{
    public int Coin;
    public int Exp;
}


public class StrengthenCfg : BaseData<StrengthenCfg>
{
    public int Pos;
    public int StarLv;
    public int AddHP;
    public int AddHurt;
    public int AddDef;
    public int MinLv;
    public int Coin;
    public int Crystal;
}
public class TaskRewardCfg : BaseData<TaskRewardCfg>
{
    public int Count;
    public int Exp;
    public int Coin;
}

public class TaskRewardData : BaseData<TaskRewardData>
{
    public int Prgs;
    public bool IsTaked;

}

public class BaseData<T>
{
    public int ID;

}

public class CfgSvc
{
    private static CfgSvc instance;

    public static CfgSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CfgSvc();
            }

            return instance;
        }
    }

    public void init()
    {
        InitGuideCfg();
        InitStrengthenCfg();
        InitTaskRewardCfgDic();
        InitMapCfg();
        CommonTool.Log("NetSvc Connected");
    }


    private Dictionary<int, GuideCfg> GuideCfgDic = new Dictionary<int, GuideCfg>();

    private void InitGuideCfg()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load("05ResCfgs/Guide.xml");
        XmlNodeList nodeLst = doc.SelectSingleNode("root")?.ChildNodes;
        for (int i = 0; i < nodeLst?.Count; i++)
        {
            XmlElement ele = nodeLst[i] as XmlElement;
            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }

            int id = Convert.ToInt32(ele.GetAttributeNode("ID")?.InnerText);
            GuideCfg guideCfg = new GuideCfg()
            {
                ID = id
            };
            foreach (XmlElement e in ele.ChildNodes)
            {
                switch (e.Name)
                {
                    case "coin":
                        guideCfg.Coin = int.Parse(e.InnerText);
                        break;
                    case "exp":
                        guideCfg.Exp = int.Parse(e.InnerText);
                        break;
                }
            }

            GuideCfgDic.Add(id, guideCfg);
        }

        CommonTool.Log("GuideCfg Done");
    }

    public GuideCfg GetGuideCfg(int id)
    {
        GuideCfg cfg;
        if (GuideCfgDic.TryGetValue(id, out cfg))
        {
            return cfg;
        }

        return null;
    }

    #region Strengthen

    private Dictionary<int, Dictionary<int, StrengthenCfg>> StrengthenCfgDic =
        new Dictionary<int, Dictionary<int, StrengthenCfg>>();

    private void InitStrengthenCfg()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load( "05ResCfgs/Strengthen.xml");
        XmlNodeList nodeLst = doc.SelectSingleNode("root")?.ChildNodes;
        for (int i = 0; i < nodeLst?.Count; i++)
        {
            XmlElement ele = nodeLst[i] as XmlElement;
            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }

            int id = Convert.ToInt32(ele.GetAttributeNode("ID")?.InnerText);
            StrengthenCfg sd = new StrengthenCfg()
            {
                ID = id
            };
            foreach (XmlElement e in ele.ChildNodes)
            {
                int value = int.Parse(e.InnerText);
                switch (e.Name)
                {
                    case "pos":
                        sd.Pos = value;
                        break;
                    case "starlv":
                        sd.StarLv = value;
                        break;
                    case "addhp":
                        sd.AddHP = value;
                        break;
                    case "addhurt":
                        sd.AddHurt = value;
                        break;
                    case "adddef":
                        sd.AddDef = value;
                        break;
                    case "minlv":
                        sd.MinLv = value;
                        break;
                    case "coin":
                        sd.Coin = value;
                        break;
                    case "crystal":
                        sd.Crystal = value;
                        break;
                }
            }

            Dictionary<int, StrengthenCfg> dic = null;
            if (StrengthenCfgDic.TryGetValue(sd.Pos, out dic))
            {
                dic.Add(sd.StarLv, sd);
            }
            else
            {
                dic = new Dictionary<int, StrengthenCfg>();
                dic.Add(sd.StarLv, sd);
                StrengthenCfgDic.Add(sd.Pos, dic);
            }
        }

        CommonTool.Log("StrengthenCfg Done");
    }

    public StrengthenCfg GetStrengthenCfg(int pos, int starlv)
    {
        StrengthenCfg sd = null;
        Dictionary<int, StrengthenCfg> dic = null;
        if (StrengthenCfgDic.TryGetValue(pos, out dic))
        {
            if (dic.ContainsKey(starlv))
            {
                sd = dic[starlv];
            }
        }

        return sd;
    }

    #endregion

    #region task

    private Dictionary<int, TaskRewardCfg> TaskRewardCfgDic = new Dictionary<int, TaskRewardCfg>();

    private void InitTaskRewardCfgDic()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load( "05ResCfgs/Taskreward.xml");
        XmlNodeList nodeLst = doc.SelectSingleNode("root")?.ChildNodes;
        for (int i = 0; i < nodeLst?.Count; i++)
        {
            XmlElement ele = nodeLst[i] as XmlElement;
            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }

            int id = Convert.ToInt32(ele.GetAttributeNode("ID")?.InnerText);
            TaskRewardCfg taskRewardCfg = new TaskRewardCfg()
            {
                ID = id
            };
            foreach (XmlElement e in ele.ChildNodes)
            {
                switch (e.Name)
                {
                    case "count":
                        taskRewardCfg.Count = int.Parse(e.InnerText);
                        break;
                    case "exp":
                        taskRewardCfg.Exp = int.Parse(e.InnerText);
                        break;
                    case "coin":
                        taskRewardCfg.Coin= int.Parse(e.InnerText);
                        break;
                }
            }

            TaskRewardCfgDic.Add(id, taskRewardCfg);
        }

        CommonTool.Log("TaskRewardCfgDic Done");
    }

    public TaskRewardCfg GetTaskRewardCfg(int id)
    {
        TaskRewardCfg trc = null;
        if (TaskRewardCfgDic.TryGetValue(id, out trc))
        {
            return trc;
        }

        return null;
    }
    
    #endregion


    #region map

    private Dictionary<int, MapCfg> mapCfgDataDic = new Dictionary<int, MapCfg>();

    private void InitMapCfg()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load( "05ResCfgs/Map.xml");
        XmlNodeList nodLst = doc.SelectSingleNode("root")?.ChildNodes;
            for (int i = 0; i < nodLst.Count; i++)
            {
                XmlElement ele = nodLst[i] as XmlElement;
                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }

                int id = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                MapCfg mc = new MapCfg
                {
                    ID = id
                };
                foreach (XmlElement e in ele.ChildNodes)
                {
                    switch (e.Name)
                    {
               
                        case "power":
                            mc.Power =int.Parse( e.InnerText);
                            break;
                    }
                }

                mapCfgDataDic.Add(id, mc);
            }
        }
    public MapCfg  GetMapCfgData(int id)
    {
        MapCfg data;
        if (mapCfgDataDic.TryGetValue(id, out data))
        {
            return data;
        }

        return null;
    }
    #endregion
    }


