using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DebuffController : MonoBehaviour
{
    public class DebuffInfo
    {
        public DebuffData debuff;
        public float time;        
    }
    List<DebuffInfo> m_debuffList = new List<DebuffInfo>();
    public void SetDebuff(DebuffData debuff)
    {
        var result = m_debuffList.Find(current => current.debuff.type == debuff.type);
        if(result != null)
        {
            result.time = 0f;
        }
        else
        {
            if (!string.IsNullOrEmpty(debuff.StartMethodName))
                gameObject.SendMessage(debuff.StartMethodName);
            m_debuffList.Add(new DebuffInfo() { debuff = debuff, time = 0f });
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_debuffList.Count > 0)
        {
            for(int i = 0; i < m_debuffList.Count; i++)
            {
                m_debuffList[i].time += Time.deltaTime;
                if(m_debuffList[i].time > m_debuffList[i].debuff.duration)
                {
                    if (!string.IsNullOrEmpty(m_debuffList[i].debuff.FinishMethodName))
                    {
                        Debug.Log(m_debuffList[i].debuff.FinishMethodName);
                        gameObject.SendMessage(m_debuffList[i].debuff.FinishMethodName);
                        m_debuffList[i].time = 0f;
                    }                   
                }
            }
            m_debuffList.RemoveAll(debuff => debuff.time == 0);
        }
    }
}
