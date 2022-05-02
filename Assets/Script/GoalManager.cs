using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(GameManager.instance.totalEnemy < 1)
        {
            animator.SetTrigger("Open");
            this.tag = "Goal";
        }
    }
}
