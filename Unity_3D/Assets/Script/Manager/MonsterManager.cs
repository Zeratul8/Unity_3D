using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum WeaponType
{
    Maul,
    Staff,
    Max
}
public enum MonsterType
{
    Knight,
    Mage,
    Boss1,
    Max
}
public class MonsterManager : SingletonMonoBehaviour<MonsterManager>
{
    [SerializeField]
    PlayerController m_player;
    [SerializeField]
    WaypointController[] m_waypoints;
    [SerializeField]
    Transform m_hudPool;
    GameObject[] m_monsterPrefabs;
    Dictionary<MonsterType, GameObjectPool<MonsterController>> m_monsterPool = new Dictionary<MonsterType, GameObjectPool<MonsterController>>();
    List<MonsterController> m_monsterList = new List<MonsterController>();


    protected override void OnStart()
    {
        m_monsterPrefabs = Resources.LoadAll<GameObject>("Prefab/Monster");
        for (int i = 0; i < m_monsterPrefabs.Length; i++)
        {
            int index = System.Convert.ToInt32(m_monsterPrefabs[i].name.Split('.')[0]) - 1;
            var pool = new GameObjectPool<MonsterController>(5, () =>
            {
                var obj = Instantiate(m_monsterPrefabs[index]);
                obj.transform.SetParent(transform);
                obj.transform.localPosition = Vector3.zero;
                var mon = obj.GetComponent<MonsterController>();
                mon.InitMonster(m_player, (MonsterType)index);
                mon.gameObject.SetActive(false);
                var hud = mon.m_hudCtr;
                hud.InitHUD(m_hudPool);
                return mon;
            });
            m_monsterPool.Add((MonsterType)index, pool);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            var mon = m_monsterPool[(MonsterType)Random.Range((int)MonsterType.Knight, (int)MonsterType.Boss1)].Get();            
            mon.SetMonster(m_waypoints[Random.Range(0, 4)]);
            m_monsterList.Add(mon);
        }
        for(int i = 0;i < m_monsterList.Count; i++)
        {
            m_monsterList[i].BehaviourProcess();
        }
    }
}
