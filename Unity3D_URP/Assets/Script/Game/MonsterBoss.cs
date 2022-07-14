using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBoss : MonsterController
{
    float m_turningTime;
    float m_turnDration = 4f;
    float m_nomalizedTime;
    float m_sqrAttackDist;
    Transform m_dummyLeft;
    Quaternion m_startRot;
    Quaternion m_endRot;
    protected override void InitStatus()
    {
        m_status = new Status(1000, 90f, 5f, 10f, 100f, 38f, 30f);
    }
    void SetAttack()
    {
        SetState(BehaviourState.Attack);
        m_monAnimCtr.Play((MonsterAnimController.Motion)(Random.Range(1, 5) + 1), false);
    }
    float GetTargetAngle(Transform target)
    {
        var dir = target.position - transform.position;
        dir.y = 0f;
        return Vector3.Angle(transform.forward, dir);
    }
    public override void BehaviourProcess()
    {
        switch(m_state)
        {
            case BehaviourState.Idle:
                m_idleTime += Time.deltaTime;
                if(m_idleTime > m_idleDuration)
                {
                    m_idleTime = 0f;
                    if (CheckArea(m_player.transform.position, m_sqrAttackDist))
                    {
                        if(GetTargetAngle(m_player.transform) < 5.0f)
                        {
                            SetAttack();
                            return;
                        }
                        else
                        {
                            m_startRot = transform.rotation;
                            var dir = (m_player.transform.position - transform.position);
                            dir.y = 0f;
                            m_endRot = Quaternion.LookRotation(dir);
                            var dot = Vector3.Dot(m_dummyLeft.forward, dir.normalized);
                            if(dot > 0f)
                            {
                                m_monAnimCtr.Play(MonsterAnimController.Motion.Turn_Left);
                            }
                            else
                            {
                                m_monAnimCtr.Play(MonsterAnimController.Motion.Turn_Right);
                            }
                            SetState(BehaviourState.Turn);
                            var angle = Quaternion.Angle(m_startRot, m_endRot);
                            m_nomalizedTime = Mathf.Abs(angle) / 180.0f;
                            return;
                        }
                    }
                    if(FindTarget())
                    {
                        SetState(BehaviourState.Chase);
                        m_monAnimCtr.Play(MonsterAnimController.Motion.Run);
                        m_navAgent.stoppingDistance = m_attackDist;
                    }
                }
                break;
            case BehaviourState.Turn:
                m_turningTime += Time.deltaTime / (m_turnDration * m_nomalizedTime);
                transform.rotation = Quaternion.Lerp(m_startRot, m_endRot, m_turningTime);
                if(m_turningTime > 1f)
                {
                    m_turningTime = 0f;
                    //SetIdle(2f);
                    SetAttack();
                }
                break;
            case BehaviourState.Chase:
                m_navAgent.SetDestination(m_player.transform.position);               
                if (CheckArea(m_player.transform.position, Mathf.Pow(m_navAgent.stoppingDistance, 2f)))
                {
                    SetIdle(1f);
                }
                break;
        }
    }
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        m_dummyLeft = Util.FindChildObject(gameObject, "Dummy_Left").transform;
        m_sqrAttackDist = Mathf.Pow(m_attackDist, 2f);
        m_attackDelay = 1f;
    }
    void Update()
    {
        BehaviourProcess();
    }
}
