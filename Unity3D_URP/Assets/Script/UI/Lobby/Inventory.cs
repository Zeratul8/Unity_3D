using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    GameObject m_itemSlotPrefab;
    [SerializeField]
    GameObject m_playerObj;
    [SerializeField]
    UIGrid m_slotGrid;
    List<ItemData> m_itemlist;
    List<ItemSlot> m_slotList = new List<ItemSlot>();
    Sprite[] m_iconSprites;
    void CreateSlot(ItemData data)
    {
        var obj = Instantiate(m_itemSlotPrefab);
        obj.transform.SetParent(m_slotGrid.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        var slot = obj.GetComponent<ItemSlot>();
        slot.SetSlot(this, data, GetIcon(data.icon));
        m_slotList.Add(slot);
    }
    void ClearSlot()
    {
        for(int i = 0; i < m_slotList.Count; i++)
        {
            Destroy(m_slotList[i].gameObject);
        }
        m_slotList.Clear();
    }
    public void OnSelectWeaponTab(bool value)
    {
        if (value)
        {
            ClearSlot();
            var results = m_itemlist.FindAll(item => item.type == ItemType.Weapon);
            for (int i = 0; i < results.Count; i++)
            {
                CreateSlot(results[i]);
            }
            m_slotGrid.repositionNow = true;
        }      
    }
    public void OnEquipItem(ItemSlot itemSlot)
    {
        var prevSlot = m_slotList.Find(slot =>
        {
            if (slot.ItemData.type == ItemType.Weapon)
            {
                if (slot.ItemData.id == PlayerDataManager.Instance.WeaponID)
                    return true;
                
            }
            else
            {
                if (slot.ItemData.id == PlayerDataManager.Instance.ShieldID)
                    return true;
            }
            return false;           
        });
        itemSlot.EquipItem();
        EquipmentManager.Instance.EquipItem(m_playerObj, itemSlot.ItemData.type == ItemType.Weapon ? itemSlot.ItemData.id : -1, itemSlot.ItemData.type == ItemType.Shield ? itemSlot.ItemData.id : -1);
        prevSlot.ResetBottomMenu();
    }
    public void OnSelectShieldTab(bool value)
    {
        if (value)
        {
            ClearSlot();
            var results = m_itemlist.FindAll(item => item.type == ItemType.Shield);
            for (int i = 0; i < results.Count; i++)
            {
                CreateSlot(results[i]);
            }
            m_slotGrid.repositionNow = true;
        }
        
    }
    public Sprite GetIcon(int index)
    {
        return m_iconSprites[index - 1];
    }
    void Awake()
    {
        m_iconSprites = Resources.LoadAll<Sprite>("Images/Icons");
        m_itemlist = ItemDataTable.Instance.m_ItemTable.ToList();
    }   
    
}