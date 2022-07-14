using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDataTable : SingletonMonoBehaviour<SkillDataTable>
{    
    Dictionary<PlayerAnimController.Motion, SkillData> m_skillTable = new Dictionary<PlayerAnimController.Motion, SkillData>();
    public SkillData GetSkillData(PlayerAnimController.Motion motion)
    {
        return m_skillTable[motion];
    }
    // Start is called before the first frame update
    void Start()
    {
        m_skillTable.Add(PlayerAnimController.Motion.Attack1, new SkillData() { attackArea = 0, attack = 10, delayTime = 0.1f, knockback = 0.5f, effectId = 1, hitRate = 0, debuff = new DebuffData() { type = DebuffType.None } });
        m_skillTable.Add(PlayerAnimController.Motion.Attack2, new SkillData() { attackArea = 1, attack = 10, delayTime = 0.1f, knockback = 1f, effectId = 1, hitRate = 0, debuff = new DebuffData() { type = DebuffType.None } });
        m_skillTable.Add(PlayerAnimController.Motion.Attack3, new SkillData() { attackArea = 2, attack = 10, delayTime = 0.1f, knockback = 1f, effectId = 2, hitRate = 0, debuff = new DebuffData() { type = DebuffType.None } });
        m_skillTable.Add(PlayerAnimController.Motion.Attack4, new SkillData() { attackArea = 3, attack = 10, delayTime = 0.15f, knockback = 1.2f, effectId = 3, hitRate = 10, debuff = new DebuffData() { type = DebuffType.None } });
        m_skillTable.Add(PlayerAnimController.Motion.Skill1, new SkillData() { attackArea = 2, attack = 30, delayTime = 1f, knockback = 2f, effectId = 15, hitRate = 100, coolTime = 3f, debuff = new DebuffData() { type = DebuffType.Stun, duration = 3f, StartMethodName = "SetStun", FinishMethodName = "EndStun" } });
        m_skillTable.Add(PlayerAnimController.Motion.Skill2, new SkillData() { attackArea = 3, attack = 25, delayTime = 1f, knockback = 2.5f, effectId = 4, hitRate = 100, coolTime = 5f, debuff = new DebuffData() { type = DebuffType.Knockdown, duration = 6f, StartMethodName = "SetKnockdown", FinishMethodName = "EndKnockdown" } });
    }
}
