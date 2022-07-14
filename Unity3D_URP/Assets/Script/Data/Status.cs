using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Status 
{
    public int hp;
    public int hpMax;
    public float hitRate;
    public float dodgeRate;
    public float criRate;
    public float criAtk;
    public float attack;
    public float defence;

    public Status(int hp, float hitRate, float dodgeRate, float criRate, float criAtk, float attack, float defence)
    {
        this.hp = this.hpMax = hp;
        this.hitRate = hitRate;
        this.dodgeRate = dodgeRate;
        this.criRate = criRate;
        this.criAtk = criAtk;
        this.attack = attack;
        this.defence = defence;
    }
}
