using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableEffect : Singleton<TableEffect>
{ 
    public class Data
    {
        public int id;
        public string dummy;
        public string[] Prefab = new string[4];
    }
    public Dictionary<int, Data> m_dicData = new Dictionary<int, Data>();    
    public void LoadTable()
    {
        TableLoader.Instance.LoadData(TableLoader.Instance.LoadTableData("Effect"));
        m_dicData.Clear();
        for(int i = 0; i < TableLoader.Instance.Count; i++)
        {
            Data data = new Data();
            data.id = TableLoader.Instance.GetInt("Id", i);
            data.dummy = TableLoader.Instance.GetString("Dummy", i);
            for(int j = 0; j < 4; j++)
            {
                data.Prefab[j] = TableLoader.Instance.GetString("Prefab_" + (j + 1), i);
            }
            m_dicData.Add(data.id, data);
        }
        TableLoader.Instance.Clear();
    }
}
