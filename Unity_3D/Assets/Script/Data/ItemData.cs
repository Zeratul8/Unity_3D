using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Weapon,
    Shield,
    Max
}
[System.Serializable]
public struct ItemData 
{
    public int id;
    public ItemType type;
    public string name;
    public int tier;
    public float value;
    public int price;
    public int level;
    public int icon;
    public string prefab;
}
