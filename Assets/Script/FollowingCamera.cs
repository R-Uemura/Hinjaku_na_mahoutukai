using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �J������Ǐ]������Script
public class FollowingCamera : MonoBehaviour
{
    public GameObject target; // �v���C���[���
    public Vector3 offset; // ���΋���

    void Start()
    {
        // �^�[�Q�b�g�̎擾
        this.target = GameObject.Find("Player");

        // �J�����ƃ^�[�Q�b�g�̈ʒu�����߂�
        // offset = transform.position - target.transform.position;
    }

    void Update()
    {
        // �ʒu�̍X�V
        transform.position = target.transform.position + offset;
    }
}
