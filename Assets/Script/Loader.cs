using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�@�V�[���J�n���ɃQ�[���}�l�[�W���[���擾����N���X
public class Loader : MonoBehaviour
{
    // �Q�[���}�l�[�W���[�̎擾
    public GameObject gameManager;

    public void Awake()
    {
        if(GameManager.instance == null)
        {
            Instantiate(gameManager); // �Q�[���}�l�[�W���[�̎擾
        }
    }

}
