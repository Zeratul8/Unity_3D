using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MonsterController : MonoBehaviour
{
    public enum BehaviourState
    {
        Idle,
        Attack,
        Chase,
        Patrol,
        Damaged,
        Debuff,
        Die,
        Turn,
        Max
    }
    [SerializeField]
    protected Status m_status;
    [Header("타겟 인식 범위")]
    [SerializeField]
    protected float m_dectectDist = 10f;
    [Header("공격 거리")]
    [SerializeField]
    protected float m_attackDist = 3f;
    [Header("몬스터 타입")]
    protected MonsterType m_type;
    [SerializeField]
    protected BehaviourState m_state;
    [SerializeField]
    protected WaypointController m_waypointCtr;
    MoveTween m_moveTween;
    protected NavMeshAgent m_navAgent;
    protected MonsterAnimController m_monAnimCtr;
    [SerializeField]
    protected PlayerController m_player;
    CapsuleCollider m_collider;
    Renderer[] m_renderers;
    protected DebuffController m_debuffCtr;
    public HUDController m_hudCtr;
    protected bool m_isPatrol;
    protected int m_curWaypoint;
    [SerializeField]
    protected float m_attackDelay = 2f;
    [SerializeField]
    protected float m_idleDuration = 5f;
    protected float m_idleTime = 0;    
    Coroutine m_hitColorCoroutine;
    MaterialPropertyBlock m_mpBlock;
    public MonsterAnimController.Motion GetMotion { get { return m_monAnimCtr.CurrentMotion; } }
    public bool IsDie { get { return m_state == BehaviourState.Die; } }
    public MonsterType Type { get { return m_type; } }
    public Status MyStatus { get { return m_status; } }
    #region Animation Event Methods
    protected virtual void AnimEvent_Attack()
    {

    }
    void AnimEvent_AttackFinished()
    {
        SetIdle(m_attackDelay);
    }
    void AnimEvent_HitFinished()
    {
        StartCoroutine("Coroutine_SetIdle");
    }
    #endregion
    protected virtual void InitStatus()
    {
        m_status = new Status(200, 70f, 40f, 5f, 50f, 10f, 10f);
    }
    protected void SetState(BehaviourState state)
    {
        m_state = state;
    }
    void SetIdleDuration(float duration)
    {
        m_idleTime = m_idleDuration - duration;
    }
    IEnumerator Coroutine_SetIdle()
    {
        yield return new WaitForSeconds(m_idleDuration - m_idleTime);
        SetState(BehaviourState.Idle);
        m_monAnimCtr.Play(MonsterAnimController.Motion.Idle);
        SetIdleDuration(0.2f);
    }
    IEnumerator Coroutine_CalculateTargetPath(int frame)
    {
        while (m_state == BehaviourState.Chase)
        {
            for(int i = 0; i < frame; i++)
                yield return null;
            m_navAgent.SetDestination(m_player.transform.position);            
        }
    }
    protected void SetIdle(float duration)
    {
        SetState(BehaviourState.Idle);
        m_monAnimCtr.Play(MonsterAnimController.Motion.Idle);
        SetIdleDuration(duration);
    }    
    void SetHitColor(float duration)
    {
        if(m_hitColorCoroutine != null)
        {
            StopCoroutine(m_hitColorCoroutine);
            m_hitColorCoroutine = null;
        }
        m_hitColorCoroutine = StartCoroutine(Coroutine_SetHitColor(duration));
    }
    IEnumerator Coroutine_SetHitColor(float duration)
    {
        m_mpBlock.SetColor("_RimColor", Color.red);
        m_mpBlock.SetFloat("_RimPower", 1);
        for (int i = 0; i < m_renderers.Length; i++)
        {
            m_renderers[i].SetPropertyBlock(m_mpBlock);            
        }
        yield return new WaitForSeconds(duration);
        m_mpBlock.SetColor("_RimColor", Color.black);
        m_mpBlock.SetFloat("_RimPower", 10);
        for (int i = 0; i < m_renderers.Length; i++)
        {
            m_renderers[i].SetPropertyBlock(m_mpBlock);
        }
    }
    IEnumerator Coroutine_SetDissolve(float duration)
    {
        float time = 0f;
        float result = 0f;
        while(true)
        {
            time += Time.deltaTime;
            result = Mathf.Lerp(-1.5f, 0.7f, time / duration);            
            m_mpBlock.SetFloat("_Duration", result);
            for (int i = 0; i < m_renderers.Length; i++)
            {
                m_renderers[i].SetPropertyBlock(m_mpBlock);
            }
            if (time > duration)
            {
                gameObject.SetActive(false);
                yield break;
            }
            yield return null;
        }
    }
    public virtual void InitMonster(PlayerController player, MonsterType type)
    {
        m_player = player;
        m_type = type;
    }
    public void SetMonster(WaypointController waypoint)
    {
        InitStatus();
        m_hudCtr.SetHUD();
        m_waypointCtr = waypoint;
        transform.position = waypoint.transform.position;
        transform.forward = waypoint.transform.forward;
        m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
        gameObject.SetActive(true);
    }
    public void SetDamage(PlayerController attacker, AttackType type, float damage, SkillData skillData)
    {
        if (IsDie) return;
        
        m_status.hp -= Mathf.CeilToInt(damage);       
        m_hudCtr.ShowDamage(type, (float)m_status.hp / m_status.hpMax, (int)damage);
        if (type == AttackType.Dodge) return;
        m_navAgent.ResetPath();
        if(m_status.hp <= 0)
        {
            m_status.hp = 0;
            SetState(BehaviourState.Die);
            m_monAnimCtr.Play(MonsterAnimController.Motion.Die, false);
            StartCoroutine(Coroutine_SetDissolve(4f));
            m_collider.enabled = false;
            attacker.ResetAttackArea(this);
            m_hudCtr.gameObject.SetActive(false);
            m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            return;
        }
        if (skillData.debuff.type != DebuffType.None)
        {
            SetState(BehaviourState.Debuff);
            m_debuffCtr.SetDebuff(skillData.debuff);
        }
        else
        {
            SetIdleDuration(skillData.delayTime);
            SetState(BehaviourState.Damaged);
            m_monAnimCtr.Play(MonsterAnimController.Motion.Hit, false);
        }
        m_navAgent.ResetPath();
        m_navAgent.isStopped = true;
        SetHitColor(0.5f);
        if (skillData.knockback > 0f)
        {
            var dir = (transform.position - attacker.transform.position).normalized;
            dir.y = 0f;
            var duration = SkillData.MaxKnockbackDuration * (skillData.knockback / SkillData.MaxKnockbackDist);
            m_moveTween.Play(transform.position, transform.position + dir * skillData.knockback, duration);
        }
    }
    void SetStun()
    {
        m_monAnimCtr.Play(MonsterAnimController.Motion.Stun, false);
    }
    void EndStun()
    {
        SetIdle(1f);
    }
    void SetKnockdown()
    {
        m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        m_monAnimCtr.Play(MonsterAnimController.Motion.Knockdown, false);
    }
    void EndKnockdown()
    {
        m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
        SetIdle(1f);
    }
    protected bool FindTarget()
    {
        RaycastHit hit;
        var originPos = transform.position + Vector3.up * 1f;
        var targetPos = m_player.transform.position + Vector3.up * 1f;
        var dir = targetPos - originPos;
        Debug.DrawRay(originPos, dir.normalized * m_dectectDist, Color.magenta);
        if (Physics.Raycast(originPos, dir.normalized, out hit, m_dectectDist, 1 << LayerMask.NameToLayer("Background") | 1 << LayerMask.NameToLayer("Player")))
        {
            if(hit.collider.CompareTag("Background"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
    }
    protected bool CanAttack()
    {
        var dist = transform.position - m_player.transform.position;
        if(Mathf.Approximately(dist.sqrMagnitude, Mathf.Pow(m_attackDist, 2f))|| dist.sqrMagnitude < Mathf.Pow(m_attackDist, 2f))
        {
            return true;
        }
        return false;
    }
    protected bool CheckArea(Vector3 target, float area)
    {
        var dist = target - transform.position;
        if (Mathf.Approximately(dist.sqrMagnitude, area) || dist.sqrMagnitude < area)
            return true;
        return false;
    }
    public virtual void BehaviourProcess()
    {
        switch (m_state)
        {
            case BehaviourState.Idle:
                m_idleTime += Time.deltaTime;
                if (m_idleTime > m_idleDuration)
                {
                    if (FindTarget())
                    {
                        if (CanAttack())
                        {
                            SetState(BehaviourState.Attack);
                            var dir = m_player.transform.position - transform.position;
                            dir.y = 0f;
                            transform.forward = dir.normalized;
                            m_monAnimCtr.Play(MonsterAnimController.Motion.Attack1);
                            return;
                        }
                        else
                        {
                            SetState(BehaviourState.Chase);
                            StartCoroutine(Coroutine_CalculateTargetPath(30));
                            m_monAnimCtr.Play(MonsterAnimController.Motion.Run);
                            m_navAgent.stoppingDistance = m_attackDist;
                            m_idleTime = 0;
                        }
                    }
                    else
                    {
                        SetState(BehaviourState.Patrol);
                        m_monAnimCtr.Play(MonsterAnimController.Motion.Run);
                        m_navAgent.stoppingDistance = m_navAgent.radius;
                    }
                }
                break;
            case BehaviourState.Attack:
                break;
            case BehaviourState.Chase:
                //m_navAgent.SetDestination(m_player.transform.position);

                if (CheckArea(m_player.transform.position, Mathf.Pow(m_navAgent.stoppingDistance, 2f)))
                {
                    SetIdle(1f);
                }
                break;
            case BehaviourState.Patrol:
                if (!m_isPatrol)
                {
                    m_isPatrol = true;
                    m_curWaypoint++;
                    if (m_curWaypoint >= m_waypointCtr.m_waypoints.Length)
                    {
                        m_curWaypoint = 0;
                    }
                    m_navAgent.SetDestination(m_waypointCtr.m_waypoints[m_curWaypoint].transform.position);
                }
                else
                {
                    if (FindTarget())
                    {
                        m_isPatrol = false;
                        m_navAgent.ResetPath();
                        SetIdle(1f);
                    }
                    else
                    {
                        if (CheckArea(m_waypointCtr.m_waypoints[m_curWaypoint].transform.position, Mathf.Pow(m_navAgent.stoppingDistance, 2f)))
                        {
                            m_isPatrol = false;
                            SetIdle(2f);
                        }
                    }
                }
                break;
        }
    }
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        m_debuffCtr = GetComponent<DebuffController>();
        m_collider = GetComponent<CapsuleCollider>();
        m_monAnimCtr = GetComponent<MonsterAnimController>();
        m_mpBlock = new MaterialPropertyBlock();       
        m_moveTween = GetComponent<MoveTween>();
        m_navAgent = GetComponent<NavMeshAgent>();
        m_renderers = GetComponentsInChildren<Renderer>();
        InitStatus();
    }

    // Update is called once per frame
   /* void Update()
    {
        BehaviourProcess();
    }*/
}
