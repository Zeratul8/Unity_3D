using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
public class PlayerDataManager : SingletonMonoBehaviour<PlayerDataManager>
{
    PlayerData m_myData;
    public int WeaponID { get { return m_myData.weaponId; } set { m_myData.weaponId = value; } }
    public int ShieldID { get { return m_myData.shieldId; } set { m_myData.shieldId = value; } }
    public uint GetGold()
    {
        return m_myData.goldOwned;
    }
    public void IncreaseGold(uint gold)
    {
        m_myData.goldOwned += gold;
    }
    public bool DecreaseGold(uint gold)
    {
        if ((int)m_myData.goldOwned - gold < 0)
            return false;
        m_myData.goldOwned -= gold;
        return true;
    }
    public bool IsOwnedItem(int id)
    {
        var itemData = m_myData.m_itemList.Find(item => item.id == id);
        if (itemData.id == 0)
            return false;
        return true;
    }
    public void AddItem(ItemData data)
    {        
        m_myData.m_itemList.Add(data);
    }
    public uint GetGem()
    {
        return m_myData.gemOwned;
    }
    public void IncreaseGem(uint gem)
    {
        m_myData.gemOwned += gem;
    }
    public bool DecreaseGem(uint gem)
    {
        if ((int)m_myData.gemOwned - gem < 0)
            return false;
        m_myData.gemOwned -= gem;
        return true;
    }
    public PlayerData Load()
    {
        var jsonData = PlayerPrefs.GetString("PLAYER_DATA", string.Empty);
        return JsonUtility.FromJson<PlayerData>(jsonData);
    }
    public void Save()
    {
        var jsonData = JsonUtility.ToJson(m_myData);
        PlayerPrefs.SetString("PLAYER_DATA", jsonData);
        PlayerPrefs.Save();
    }
    // Start is called before the first frame update
    protected override void OnAwake()
    {
        m_myData = Load();
        if(m_myData == null)
        {
            m_myData = new PlayerData();
            Save();
        }
        //PlayerPrefs.DeleteAll();
    }
}
