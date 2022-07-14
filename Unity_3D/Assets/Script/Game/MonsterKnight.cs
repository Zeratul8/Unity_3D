using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterKnight : MonsterController
{
    protected override void AnimEvent_Attack()
    {
        if (m_player.m_isDodge) return;
        if (CheckArea(m_player.transform.position, Mathf.Pow(m_attackDist, 2f)))
        {
            var dir = m_player.transform.position - transform.position;
            var dot = Vector3.Dot(transform.forward, dir.normalized);
            if(dot > 0.9f)
            {
                var obj = EffectPool.Instance.Create(TableEffect.Instance.m_dicData[1].Prefab[0]);
                obj.transform.position = m_player.transform.position + Vector3.up * 1f;
                obj.transform.rotation = Quaternion.FromToRotation(Vector3.forward, dir.normalized);
            }
        }
    }
    public override void InitMonster(PlayerController player, MonsterType type)
    {
        base.InitMonster(player, type);
        m_hudCtr.SetName("밤새운프로그래머");
    }
    public override void BehaviourProcess()
    {
        base.BehaviourProcess();
    }
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();        
        m_dectectDist = 8f;
        m_attackDist = 2.5f;
    }
    private void Start()
    {
        m_monAnimCtr.SetWeaponType(WeaponType.Maul);
    }
}
