using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public bool attackflag = false;
    int damege = 0;
    int hprDamage = 0;

    public void Damager(int dmg)
    {
        damege = dmg;
        attackflag = true;
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
