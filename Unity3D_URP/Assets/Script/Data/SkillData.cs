using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DebuffType
{
    None = -1,
    Stun,
    Knockdown,
    Airborne,
    Max
}
public struct DebuffData
{
    public DebuffType type;
    public float duration;
    public string StartMethodName;
    public string FinishMethodName;
}
[System.Serializable]
public struct SkillData
{
    public static float MaxKnockbackDuration = 0.7f;
    public static float MaxKnockbackDist = 5f;
    public int attackArea;
    public float attack;
    public float knockback;
    public float delayTime;
    public float hitRate;
    public float coolTime;
    public int effectId;
    public DebuffData debuff;
}
