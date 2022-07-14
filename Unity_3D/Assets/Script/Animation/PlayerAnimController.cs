using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
public class PlayerAnimController : AnimController
{
    public enum Motion
    {
        None = -1,
        Idle,
        Run,
        Attack1,
        Attack2,
        Attack3,
        Attack4,
        Skill1,
        Skill2,
        Roll,
        Max
    }
    Motion m_curMotion = Motion.None;
    public Motion CurrentMotion { get { return m_curMotion; } }
    StringBuilder m_sb = new StringBuilder();
    Dictionary<string, float> m_comboResetTickTable = new Dictionary<string, float>();

    public float GetComboResetTick(string animName)
    {
        return m_comboResetTickTable[animName];
    }
    void InitComboResetTickTable()
    {
        var ac = m_animator.runtimeAnimatorController;
        float time = 0f;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].events != null && ac.animationClips[i].events.Length > 1)
                time = ac.animationClips[i].events[1].time - ac.animationClips[i].events[0].time;
            m_comboResetTickTable.Add(ac.animationClips[i].name, time);
        }
    }
    public void Play(Motion motion, bool isBlend = true)
    {
        m_sb.Append(motion);
        Play(m_sb.ToString(), isBlend);
        m_curMotion = motion;
        m_sb.Clear();
    }
    protected override void Awake()
    {
        base.Awake();
        InitComboResetTickTable();
    }
}
