using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    public int colums = 12, rows = 12; // player�ړ��\�͈�
    public GameObject[] floorTiles; // ���̔z��
    public GameObject[] blockObj; // ��Q���̔z��
    public GameObject[] itemObj; // �A�C�e���̔z��
    public GameObject[] enemyObj; // �G�̔z��
    public GameObject goalobj;

    private List<Vector3> gridPositons = new List<Vector3>(); // �S�I�u�W�F�N�g�̍��W���X�g

    public int blockMinimun = 1, blockMaximum = 4; // ��Q���̐�
    public int itemMinimun = 4, itemMaximum = 7; // �A�C�e���̐�

    // �t���A�̎��������̏���
    void BoardSetup()
    {
        GameObject toInstantiate; // Instantiate�p�̕ϐ�

        toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)]; // ��������floor�������_���őI�o

        Instantiate(toInstantiate, new Vector3(6, 0, 6), Quaternion.identity); // ����

    }

    void InitialiseList()
    {
        gridPositons.Clear(); // ���W���X�g�̏�����

        for(int x = 1; x < colums -1; x++)
        {
            for (int z = 1; z < rows - 1; z++)
            {
                gridPositons.Add(new Vector3(x, 0, z)); // �����|�W�V�����̎擾
            }
        }
    }

    //�@�I�u�W�F�N�g�����̍��W�������_���ɑI�o
    Vector3 RandomPositon()
    {
        int randmIndex = Random.Range(0, gridPositons.Count); // List���烉���_���Ƀ|�W�V������1�擾

        Vector3 randomPositon = gridPositons[randmIndex]; // randomPositon�ɍ��W���i�[

        gridPositons.RemoveAt(randmIndex); // randmPositon�Ɏg�p�����v�f��List����폜

        return randomPositon;
    }

    // �I�u�W�F�N�g�����֐�
    void LayoutobjectAtRandom(GameObject[] objArray, int min, int max)
    {
        int objectCount = Random.Range(min, max + 1); // �I�u�W�F�N�g�������鐔�̌���

        for(int i = 0; i < objectCount; i++)
        {
            Vector3 randomPositon = RandomPositon();

            GameObject objChoice = objArray[Random.Range(0, objArray.Length)]; // ��������I�u�W�F�N�g�������_���őI�o

            int randomQua = Random.Range(-180, 180);
            Instantiate(objChoice, randomPositon, Quaternion.Euler(0, randomQua, 0)); // ����
        }
    }

    // �X�e�[�W����
    public void SetupSecene(int depth)
    {
        BoardSetup();
        InitialiseList();
        LayoutobjectAtRandom(blockObj, blockMinimun, blockMaximum); // ��Q���̐���
        LayoutobjectAtRandom(itemObj, itemMinimun, itemMaximum); // �A�C�e���̐���


        int enemyCountRd = 2 + (int)Mathf.Log(depth, 2f);

        // enemy�̍ő�o������5�ɂ���
        if(enemyCountRd > 5)
        {
            enemyCountRd = 5;
        }

        GameManager.instance.totalEnemy = enemyCountRd;

        LayoutoEnemyRandom(enemyObj, enemyCountRd, enemyCountRd ,depth); // �G�l�~�[�̐���

        int randomX = Random.Range(0, 13);
        Instantiate(goalobj, new Vector3 (randomX,0,12), Quaternion.identity);

    }

    // enemy�̐����֐�
    void LayoutoEnemyRandom(GameObject[] objArray, int min, int max , int stageLV)
    {
        int objectCount = Random.Range(min, max + 1); // �I�u�W�F�N�g�������鐔�̌���


        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPositon = RandomPositon();

            GameObject objChoice = objArray[Random.Range(0, objArray.Length)]; // ��������I�u�W�F�N�g�������_���őI�o

            Enemy enemy = objChoice.GetComponent<Enemy>();

            if(enemy.lv >= stageLV)
            {
                objChoice = objArray[0];
            }

            Instantiate(objChoice, randomPositon, Quaternion.Euler(0, 180, 0)); // ����
        }
    }
}
