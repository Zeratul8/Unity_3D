using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : SingletonMonoBehaviour<EquipmentManager>
{
    GameObject m_weapon;
    GameObject m_shield;
    public void EquipItem(GameObject target, int weaponId, int shieldId)
    {
        GameObject dummyWeapon = null;
        GameObject dummyShield = null;

        ItemData? weaponData = null;
        ItemData? shieldData = null;
        if(weaponId != -1)
            weaponData = ItemDataTable.Instance.m_itemDatas[weaponId];
        if(shieldId != -1)
            shieldData = ItemDataTable.Instance.m_itemDatas[shieldId];
        if (weaponData != null)
        {
            dummyWeapon = Util.FindChildObject(target, "Dummy_Weapon");
            if (m_weapon) Destroy(m_weapon);
            var obj = Resources.Load<GameObject>("Prefab/PlayerWeapon/" + weaponData.Value.prefab);
            m_weapon = Instantiate(obj);
            m_weapon.transform.SetParent(dummyWeapon.transform);
            m_weapon.transform.localPosition = Vector3.zero;
            m_weapon.transform.localScale = Vector3.one;
            m_weapon.transform.localRotation = obj.transform.localRotation;
            m_weapon.layer = dummyWeapon.layer;
        }
        if (shieldData != null)
        {
            dummyShield = Util.FindChildObject(target, "Dummy_Shield");
            if (m_shield) Destroy(m_shield);
            var obj = Resources.Load<GameObject>("Prefab/PlayerShield/" + shieldData.Value.prefab);
            m_shield = Instantiate(obj);
            m_shield.transform.SetParent(dummyShield.transform);
            m_shield.transform.localPosition = Vector3.zero;
            m_shield.transform.localScale = Vector3.one;
            m_shield.transform.localRotation = obj.transform.localRotation;
            m_shield.layer = dummyShield.layer;
        }
    }
}
