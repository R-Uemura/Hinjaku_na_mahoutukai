using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour
{
    Animator animator;
    public float moveSpeed = 0.05f;
    Vector3 homePosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        homePosition = this.transform.position;
    }

    void Update()
    {
        if (this.transform.position.x < -(homePosition.x))
        {
            this.transform.position = homePosition;
        }
        else
        {
            Vector3 mpos = new Vector3(0, 0, moveSpeed); // ˆÚ“®—Ê
            this.transform.Translate(mpos); // ˆÚ“®
        }
    }
}
