using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData 
{
    public readonly static uint BasicGold = 2000;
    public readonly static uint BasicGem = 200;
    public uint goldOwned;
    public uint gemOwned;
    public int weaponId;
    public int shieldId;
    public List<ItemData> m_itemList = new List<ItemData>();

    public PlayerData()
    {
        goldOwned = BasicGold;
        gemOwned = BasicGem;
        m_itemList.Add(ItemDataTable.Instance.m_itemDatas[1]);
        m_itemList.Add(ItemDataTable.Instance.m_itemDatas[51]);
        weaponId = 1;
        shieldId = 51;
    }
}
