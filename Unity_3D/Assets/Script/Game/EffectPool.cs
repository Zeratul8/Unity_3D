using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : SingletonMonoBehaviour<EffectPool>
{
    [SerializeField]
    int m_presetSize = 1;
    List<string> m_listEffectName = new List<string>();
    Dictionary<string, GameObject> m_dicPrefab = new Dictionary<string, GameObject>();
    Dictionary<string, GameObjectPool<EffectPoolUnit>> m_dicEffectPool = new Dictionary<string, GameObjectPool<EffectPoolUnit>>();
    IEnumerator Coroutine_SetActive(EffectPoolUnit unit, bool isOn)
    {
        yield return new WaitForEndOfFrame();
        unit.gameObject.SetActive(isOn);
    }
    void LoadEffect()
    {
        TableEffect.Instance.LoadTable();
        foreach(KeyValuePair<int, TableEffect.Data> pair in TableEffect.Instance.m_dicData)
        {
            for(int i = 0; i < pair.Value.Prefab.Length; i++)
            {
                if(!m_listEffectName.Contains(pair.Value.Prefab[i]))
                {
                    m_listEffectName.Add(pair.Value.Prefab[i]);
                }
            }
        }
        for(int i = 0; i < m_listEffectName.Count; i++)
        {
            EffectPoolUnit poolUnit = null;
            string effectName = m_listEffectName[i];
            var prefab =  Resources.Load<GameObject>("Prefab/Effect/" + effectName);
            m_dicPrefab.Add(effectName, prefab);
            GameObjectPool<EffectPoolUnit> effectPool = new GameObjectPool<EffectPoolUnit>();
            m_dicEffectPool.Add(effectName, effectPool);
            effectPool.Allocate(m_presetSize, ()=>
            {
                var obj = Instantiate(m_dicPrefab[effectName]);
                poolUnit = obj.GetComponent<EffectPoolUnit>();
                if(poolUnit == null)
                {
                    poolUnit = obj.AddComponent<EffectPoolUnit>();
                }
                if(obj.GetComponent<ParticleAutoDestory>() == null)
                {
                    obj.AddComponent<ParticleAutoDestory>();
                }
                poolUnit.SetObjectPool(effectName);
                if (poolUnit.gameObject.activeSelf)
                    poolUnit.gameObject.SetActive(false);
                return poolUnit;
            });          
        }
    }
    public GameObject Create(string effectName, Vector3 position, Quaternion rotation)
    {
        EffectPoolUnit unit = null;
        var pool = m_dicEffectPool[effectName];
        for(int i = 0; i < pool.Count; i++)
        {
            unit = pool.Get();
            if(!unit.IsReady)
            {
                pool.Set(unit);
                unit = null;
            }
            else
            {
                break;
            }
        }
        if (unit == null)
        {
            unit = pool.New();
        }
        unit.transform.position = position;
        unit.transform.rotation = rotation;
        StartCoroutine(Coroutine_SetActive(unit, true));
        return unit.gameObject;
    }

    public GameObject Create(string effectName)
    {
        return Create(effectName, Vector3.zero, Quaternion.identity);
    }
    public void AddPoolUnit(string effectName, EffectPoolUnit unit)
    {
        var pool = m_dicEffectPool[effectName];
        if(pool != null)
        {
            pool.Set(unit);
        }
    }
    // Start is called before the first frame update
    protected override void OnAwake()
    {
        LoadEffect();
    }    
}
