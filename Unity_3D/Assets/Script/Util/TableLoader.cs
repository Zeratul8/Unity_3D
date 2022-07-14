using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TableLoader : Singleton<TableLoader>
{
    List<Dictionary<string, string>> m_table = new List<Dictionary<string, string>>();
    public int Count { get { return m_table.Count; } }
    public string GetString(string key, int index)
    {
        return m_table[index][key];
    }
    public byte GetByte(string key, int index)
    {
        return byte.Parse(GetString(key, index));
    }
    public int GetInt(string key, int index)
    {
        return int.Parse(GetString(key, index));
    }
    public float GetFloat(string key, int index)
    {
        return float.Parse(GetString(key, index));
    }
    public bool GetBool(string key, int index)
    {
        return bool.Parse(GetString(key, index));
    }
    public void Clear()
    {
        m_table.Clear();
    }
    public byte[] LoadTableData(string tableName)
    {
        var data = Resources.Load<TextAsset>("ExcelDatas/" + tableName);
        return data.bytes;
    }
    public void LoadData(byte[] data)
    {
        MemoryStream ms = new MemoryStream(data);
        BinaryReader br = new BinaryReader(ms);

        int rowCount = br.ReadInt32();
        int columCount = br.ReadInt32();
        var strDatas = br.ReadString();
        var datas = strDatas.Split('\t');

        m_table.Clear();
        List<string> keyList = new List<string>();
        int offset = 1;
        for(int i = 0; i < rowCount; i++)
        {
            offset++;
            if(i == 0)
            {
                for(int j = 0; j < columCount - 1; j++)
                {
                    keyList.Add(datas[offset]);
                    offset++;
                }
            }
            else
            {
                Dictionary<string, string> dicData = new Dictionary<string, string>();
                for(int j = 0; j < columCount - 1; j++)
                {
                    dicData.Add(keyList[j], datas[offset]);
                    offset++;
                }
                m_table.Add(dicData);
            }
        }
        ms.Close();
        br.Close();
    }
}
