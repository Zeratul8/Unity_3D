using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header("Status")]
    [SerializeField]
    Status m_status;
    [Header("공격영역")]
    [SerializeField]
    GameObject m_attackAreaObj;
    [SerializeField]
    HUDText m_hudText;
    AttackAreaUnitFind[] m_attackArea;
    [Header("인식영역")]
    [SerializeField]
    AttackAreaUnitFind m_detectArea;
    PlayerAnimController m_animCtr;
    CharacterController m_charCtr;
    NavMeshAgent m_navAgent;
    Vector3 m_dir;
    Vector3 m_clickDir;
    Vector3 m_targetPos;    
    float m_speed = 3f;
    float m_gravity;
    bool m_isPressAttack;
    bool m_isCombo;
    int m_comboIndex;
    NavMeshPath m_navPath;
    LineRenderer m_pathLine;    
    List<PlayerAnimController.Motion> m_comboList = new List<PlayerAnimController.Motion>() { PlayerAnimController.Motion.Attack1, PlayerAnimController.Motion.Attack2, PlayerAnimController.Motion.Attack3, PlayerAnimController.Motion.Attack4 };
    Queue<KeyCode> m_keyBuffer = new Queue<KeyCode>();
    public bool m_isDodge;
    public Status MyStatus { get { return m_status; } }
    public PlayerAnimController.Motion GetMotion { get { return m_animCtr.CurrentMotion; } }
    public bool IsAttack { get
        {
            if (GetMotion == PlayerAnimController.Motion.Attack1 ||
                GetMotion == PlayerAnimController.Motion.Attack2 ||
                GetMotion == PlayerAnimController.Motion.Attack3 ||
                GetMotion == PlayerAnimController.Motion.Attack4 ||
                GetMotion == PlayerAnimController.Motion.Skill1  ||
                GetMotion == PlayerAnimController.Motion.Skill2)
                return true;
            return false;
        } }
    public bool IsDodge { get { return GetMotion == PlayerAnimController.Motion.Roll; } }
    #region Animation Event Methods
    void AnimEvent_SetLayerMonster(int isChange)
    {
        if (isChange == 1)
            gameObject.layer = LayerMask.NameToLayer("Monster");
        else
            gameObject.layer = LayerMask.NameToLayer("Player");
    }
    AttackType AttackProcess(MonsterController mon, SkillData skill, out float damage)
    {
        AttackType type = AttackType.Dodge;
        damage = 0f;
        if(CalculationDamage.AttackDecision(MyStatus.hitRate + skill.hitRate, mon.MyStatus.dodgeRate))
        {
            type = AttackType.Normal;
            damage = CalculationDamage.NormalDamage(MyStatus.attack, skill.attack, mon.MyStatus.defence);
            if(CalculationDamage.CriticalDecision(MyStatus.criRate))
            {
                type = AttackType.Critical;
                damage = CalculationDamage.CriticalDamage(damage, MyStatus.criAtk);
            }
        }
        return type;
    }
    void AnimEvent_Attack()
    {
        var skillData = SkillDataTable.Instance.GetSkillData(GetMotion);
        var unitList = m_attackArea[skillData.attackArea].m_unitList;
        float damage = 0f;
        if (unitList.Count > 0)
        {
            for(int i = 0; i < unitList.Count; i++)
            {
                var mon = unitList[i].GetComponent<MonsterController>();
                if (mon.IsDie) continue;
                var type = AttackProcess(mon, skillData, out damage);
                mon.SetDamage(this, type, damage, skillData);
                if (type == AttackType.Dodge) continue;
                var obj = EffectPool.Instance.Create(TableEffect.Instance.m_dicData[skillData.effectId].Prefab[type == AttackType.Critical ? 1 : 0]);
                var dir = transform.position - mon.transform.position;
                obj.transform.position = mon.transform.position + Vector3.up * 1f;
                obj.transform.rotation = Quaternion.FromToRotation(Vector3.forward, dir.normalized);
            }
        }
    }
    void AnimEvent_AttackFinished()
    {
        m_isCombo = false;
        if (m_isPressAttack)
            m_isCombo = true;
        if(m_keyBuffer.Count > 0)
        {
            if(m_keyBuffer.Count > 1)
            {
                m_isCombo = false;
                ReleaseKeyBuffer();
            }
            else
            {
                var key = m_keyBuffer.Dequeue();
                m_isCombo = true;
            }
        }
        if(m_isCombo)
        {
            m_comboIndex++;
            if (m_comboIndex > m_comboList.Count - 1)
                m_comboIndex = 0;
            m_animCtr.Play(m_comboList[m_comboIndex]);
        }
        else
        {
            SetLocomotion(true);
            m_comboIndex = 0;
        }
    }
    void AnimEvent_RollFinished()
    {
        SetLocomotion(true);
    }
    #endregion
    void InitStatus()
    {
        m_status = new Status(100, 70f, 10f, 5f, 100f, 35f, 15f);
    }
    Vector3? GetClickPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100f, 1 << LayerMask.NameToLayer("Background") | 1 << LayerMask.NameToLayer("Ground")))
        {
            return hit.point;
        }
        return null;       
    }
    void ReleaseKeyBuffer()
    {
        m_keyBuffer.Clear();
    }
    void SetLocomotion(bool isAttackFinish = false)
    {
        if (m_dir != Vector3.zero)
        {
            if (GetMotion == PlayerAnimController.Motion.Idle || isAttackFinish)
            {
                m_animCtr.Play(PlayerAnimController.Motion.Run);
                if (m_clickDir == Vector3.zero)
                    m_navAgent.ResetPath();
                else
                    m_animCtr.SetRootMotion(false);
            }
            //m_animCtr.SetRootMotion(false);
            if(m_clickDir == Vector3.zero && !IsAttack && !IsDodge)
                transform.forward = m_dir;
        }
        else
        {
            m_animCtr.SetRootMotion(true);
            if (GetMotion == PlayerAnimController.Motion.Run || isAttackFinish)
            {
                m_animCtr.Play(PlayerAnimController.Motion.Idle);                
            }
        }
    }
    Vector3 GetTargetDir()
    {
        var target = m_detectArea.GetNearestUnit(transform);
        if (target == null) return Vector3.zero;
        var dir = target.transform.position - transform.position;
        dir.y = 0f;
        return dir.normalized;
    }
    Vector3 GetPadDir()
    {
        var padDir = MovePadController.Instance.GetAxis;
        Vector3 dir = Vector3.zero;
        if(padDir.x < 0f)
        {
            dir += Vector3.left * Mathf.Abs(padDir.x);
        }
        if(padDir.x > 0f)
        {
            dir += Vector3.right * padDir.x;
        }
        if(padDir.y < 0f)
        {
            dir += Vector3.back * Mathf.Abs(padDir.y);
        }
        if(padDir.y > 0f)
        {
            dir += Vector3.forward * padDir.y;
        }
        return dir;
    }
    public void ResetAttackArea(MonsterController mon)
    {
        for(int i = 0; i < m_attackArea.Length; i++)
        {
            m_attackArea[i].RemoveUnit(mon.gameObject);
        }
        m_detectArea.RemoveUnit(mon.gameObject);
    }
    public void OnPressAttack()
    {
        m_navAgent.ResetPath();
        m_isPressAttack = true;
        if (GetMotion == PlayerAnimController.Motion.Idle || GetMotion == PlayerAnimController.Motion.Run)
        {
            var dir = GetTargetDir();
            if (dir != Vector3.zero)
                transform.forward = dir;
            m_animCtr.Play(PlayerAnimController.Motion.Attack1);
        }
        else
        {
            m_keyBuffer.Enqueue(KeyCode.Space);
        }
        if (IsInvoking("ReleaseKeyBuffer"))
            CancelInvoke("ReleaseKeyBuffer");
        Invoke("ReleaseKeyBuffer", m_animCtr.GetComboResetTick(GetMotion.ToString()));
    }
    public void OnReleaseAttack()
    {
        m_isPressAttack = false;
    }
    public void OnPressSkill1()
    {
        m_animCtr.Play(PlayerAnimController.Motion.Skill1, false);
    }
    public void OnPressSkill2()
    {
        m_animCtr.Play(PlayerAnimController.Motion.Skill2, false);
    }
    public void OnPressDodge()
    {
        m_navAgent.ResetPath();
        m_animCtr.Play(PlayerAnimController.Motion.Roll, false);
    }
    void OnAttack()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            OnPressAttack();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnReleaseAttack();
        }
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            ActionButtonManager.Instance.OnPressButton(ActionButtonManager.ButtonType.Skill1);
        }
        if(Input.GetKeyDown(KeyCode.LeftAlt))
        {
            ActionButtonManager.Instance.OnPressButton(ActionButtonManager.ButtonType.Skill2);
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            ActionButtonManager.Instance.OnPressButton(ActionButtonManager.ButtonType.Skill3);
        }
    }
    void OnMove()
    {
        m_dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        var padDir = GetPadDir();
        if (Input.GetMouseButtonDown(0))
        {            
            if (UICamera.touchCount >= 1) return;
            var pos = GetClickPosition();
            if(pos != null)
            {
                m_targetPos = pos.Value;
                m_clickDir = (pos.Value - transform.position).normalized;
                m_navAgent.SetDestination(m_targetPos);                
               // NavMesh.CalculatePath(transform.position, m_targetPos, NavMesh.AllAreas, m_navPath);
            }
        }
        if(padDir != Vector3.zero)
        {
            m_dir = padDir;
        }
        if(m_clickDir != Vector3.zero)
        {
            var dist = (m_targetPos - transform.position);
            if (Mathf.Approximately(dist.sqrMagnitude, Mathf.Pow(0.5f, 2f)) || dist.sqrMagnitude < Mathf.Pow(0.5f, 2f))
            {
                m_clickDir = Vector3.zero;
            }
            m_dir = m_clickDir;                        
        }
        SetLocomotion();

        //transform.position += m_dir * m_speed * Time.deltaTime;
        /* if (m_charCtr.isGrounded)
         {
             Debug.Log("지면에 닿아있음");
             m_gravity = 0f;
         }
         else
         {
             m_gravity += Physics2D.gravity.y * Time.deltaTime; 
         }
         m_charCtr.Move(m_dir * m_speed * Time.deltaTime + Vector3.up * m_gravity);*/
        //m_charCtr.SimpleMove(m_dir * m_speed);
        if(m_clickDir == Vector3.zero && !IsAttack && !IsDodge)
           m_navAgent.Move(m_dir * m_navAgent.speed * Time.deltaTime);
    }
    private void OnParticleCollision(GameObject other)
    {        
        Debug.Log("플레이어 명중");
    }
    // Start is called before the first frame update
    void Start()
    {     
        m_animCtr = GetComponent<PlayerAnimController>();
        m_charCtr = GetComponent<CharacterController>();
        m_navAgent = GetComponent<NavMeshAgent>();
        m_pathLine = GetComponent<LineRenderer>();      
        m_attackArea = m_attackAreaObj.GetComponentsInChildren<AttackAreaUnitFind>();        
        m_animCtr.Play(PlayerAnimController.Motion.Idle);
        m_navPath = new NavMeshPath();
        InitStatus();
        ActionButtonManager.Instance.SetButton(ActionButtonManager.ButtonType.Main, 0f, OnPressAttack, OnReleaseAttack);
        ActionButtonManager.Instance.SetButton(ActionButtonManager.ButtonType.Skill1, SkillDataTable.Instance.GetSkillData(PlayerAnimController.Motion.Skill1).coolTime, OnPressSkill1);
        ActionButtonManager.Instance.SetButton(ActionButtonManager.ButtonType.Skill2, SkillDataTable.Instance.GetSkillData(PlayerAnimController.Motion.Skill2).coolTime, OnPressSkill2);
        ActionButtonManager.Instance.SetButton(ActionButtonManager.ButtonType.Skill3, 2f, OnPressDodge);
    }

    // Update is called once per frame
    void Update()
    {
        OnMove();
        OnAttack();
        if(Input.GetKeyDown(KeyCode.K))
        {
            m_hudText.Add(Random.Range(1, 200).ToString(), Color.red, 0f);
        }
    }
    void LateUpdate()
    {
       if(m_navAgent.hasPath)
        {
            m_pathLine.positionCount = m_navAgent.path.corners.Length;
            m_pathLine.SetPositions(m_navAgent.path.corners);
        }
    }
}
