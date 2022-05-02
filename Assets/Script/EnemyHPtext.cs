using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPtext : MonoBehaviour
{
    int maxHP;
    float hpratio;
    public Text enemyHpText;

    public void GetEnemyMaxHP(int hp)
    {
        maxHP = hp;
        EnemyHPtextUpdate(hp);
    }

    public void EnemyHPtextUpdate(int hp)
    {
        hpratio = (float)hp / (float)maxHP * 100f;

        if(hpratio < 1 && hpratio > 0)
        {
            hpratio = 1f;
        }

        enemyHpText.text = "" + (int)hpratio + "%";
    }

    public void EnemyHPtexthide()
    {
        enemyHpText.text = " ";
    }
}
