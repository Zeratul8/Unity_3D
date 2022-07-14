﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class MonsterAnimController : AnimController
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
        Turn_Left,
        Turn_Right,
        Hit,
        Die,
        Stun,
        Knockdown,
        Max
    }
    Motion m_curMotion = Motion.None;
    public Motion CurrentMotion { get { return m_curMotion; } }
    StringBuilder m_sb = new StringBuilder();
    public void SetWeaponType(WeaponType type)
    {
        m_animator.SetFloat("WeaponType", (int)type);
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
    }
}
