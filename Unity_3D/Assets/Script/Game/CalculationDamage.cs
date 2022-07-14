using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AttackType
{
    Normal,
    Critical,
    Dodge,
    Max
}
public class CalculationDamage 
{
    public static bool AttackDecision(float attackerHit, float defenceDodge)
    {
        if (Mathf.Approximately(attackerHit, 100.0f) || attackerHit > 100.0f)
            return true;
        float total = attackerHit + defenceDodge;
        float hitRate = Random.Range(0.0f, total);
        if(hitRate <= attackerHit)
        {
            return true;
        }
        return false;
    }
    public static float NormalDamage(float attackerAtk, float skillAtk, float defenceDef)
    {
        float attack = attackerAtk + (attackerAtk * skillAtk) / 100.0f;
        return attack - defenceDef;
    }
    public static bool CriticalDecision(float criRate)
    {
        var result = Random.Range(0.0f, 100.0f);
        if (result <= criRate)
            return true;
        return false;
    }
    public static float CriticalDamage(float damage, float criAtk)
    {
        return damage + (damage * criAtk) / 100.0f;
    }
}
