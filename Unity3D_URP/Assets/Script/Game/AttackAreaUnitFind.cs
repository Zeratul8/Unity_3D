using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAreaUnitFind : MonoBehaviour
{
    public List<GameObject> m_unitList = new List<GameObject>();
    public void RemoveUnit(GameObject gObj)
    {
        m_unitList.Remove(gObj);
    }
    public GameObject GetNearestUnit(Transform target)
    {
        if (m_unitList.Count == 0) return null;
        int index = 0;
        float minDist = (target.position - m_unitList[0].transform.position).sqrMagnitude;

        for(int i = 1; i < m_unitList.Count; i++)
        {
            float curDist = (target.position - m_unitList[i].transform.position).sqrMagnitude;
            if(minDist > curDist)
            {
                minDist = curDist;
                index = i;
            }
        }
        return m_unitList[index];
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Monster"))
        {
            m_unitList.Add(other.gameObject);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            m_unitList.Remove(other.gameObject);
        }
    }    
}
