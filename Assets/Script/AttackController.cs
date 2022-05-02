using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Effekseer;

public class AttackController : MonoBehaviour
{
    public bool attackflag = false;
    int damege = 0;
    int hprDamage = 0;

    public GameObject effect;
    EffekseerEmitter effekseerEmitter;
    bool nulleffect = true;

    public void Start()
    {
        if (effect != null)
        {
            nulleffect = false;
            effekseerEmitter = effect.GetComponent<EffekseerEmitter>();
        }
        else
        {
            nulleffect = true;
        }
    }

    public void Damager(int dmg)
    {
        damege = dmg;
        attackflag = true;

        if (!nulleffect)
        {
            effekseerEmitter.Play();
        }

    }

    public void PlayerAttack(int dmg)
    {
        damege = dmg;
        attackflag = true;
    }

    public void PlayerEffecktPlay()
    {
        if (!nulleffect)
        {
            effekseerEmitter.Play();
        }
    }

    public void HprDamager(int hprdmg)
    {
        hprDamage = hprdmg;
    }

    public int EnemyDamage(int dmg)
    {
        dmg = damege;
        return dmg;
    }

    public void DamageReset()
    {
        attackflag = false;
        damege = 0;
        hprDamage = 0;
    }

    public int PlayerDamage(int hp)
    {
        hp -= damege;

        return hp;
    }

    public int PlayerHPrDamage(int hpr)
    {
        hpr -= hprDamage;

        return hpr;
    }

}
