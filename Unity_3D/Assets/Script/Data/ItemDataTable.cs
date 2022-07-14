using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataTable : DontDestroy<ItemDataTable>
{
    [SerializeField]
    public ItemData[] m_ItemTable;
    public Dictionary<int, ItemData> m_itemDatas = new Dictionary<int, ItemData>();
    // Start is called before the first frame update
    protected override void OnAwake()
    {
        for(int i = 0; i < m_ItemTable.Length; i++)
        {
            m_itemDatas.Add(m_ItemTable[i].id, m_ItemTable[i]);
        }
    }
}
