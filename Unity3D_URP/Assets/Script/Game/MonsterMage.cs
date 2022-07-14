using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMage : MonsterController
{
    Transform m_dummyProjectile;
    protected override void AnimEvent_Attack()
    {
        var projectile = EffectPool.Instance.Create(TableEffect.Instance.m_dicData[19].Prefab[0]);
        var dir = m_player.transform.position - transform.position;
        dir.y = 0f;
        projectile.transform.position = m_dummyProjectile.position;
        projectile.transform.forward = dir.normalized;
    }
    public override void InitMonster(PlayerController player, MonsterType type)
    {
        base.InitMonster(player, type);
        m_hudCtr.SetName("[ffff00]퇴근못한디자이너[-]");
    }
    public override void BehaviourProcess()
    {
        base.BehaviourProcess();
    }
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        m_dummyProjectile = Util.FindChildObject(gameObject, "Dummy_Projectile").transform;        
        m_dectectDist = 15f;
        m_attackDist = 6f;
    }
    void Start()
    {
        m_monAnimCtr.SetWeaponType(WeaponType.Staff);
    }
}
