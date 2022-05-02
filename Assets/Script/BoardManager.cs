using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    public int colums = 12, rows = 12; // player移動可能範囲
    public GameObject[] floorTiles; // 床の配列
    public GameObject[] blockObj; // 障害物の配列
    public GameObject[] itemObj; // アイテムの配列
    public GameObject[] enemyObj; // 敵の配列
    public GameObject goalobj;

    private List<Vector3> gridPositons = new List<Vector3>(); // 全オブジェクトの座標リスト

    public int blockMinimun = 1, blockMaximum = 4; // 障害物の数
    public int itemMinimun = 4, itemMaximum = 7; // アイテムの数

    // フロアの自動生成の処理
    void BoardSetup()
    {
        GameObject toInstantiate; // Instantiate用の変数

        toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)]; // 生成するfloorをランダムで選出

        Instantiate(toInstantiate, new Vector3(6, 0, 6), Quaternion.identity); // 生成

    }

    void InitialiseList()
    {
        gridPositons.Clear(); // 座標リストの初期化

        for(int x = 1; x < colums -1; x++)
        {
            for (int z = 1; z < rows - 1; z++)
            {
                gridPositons.Add(new Vector3(x, 0, z)); // 生成ポジションの取得
            }
        }
    }

    //　オブジェクト生成の座標をランダムに選出
    Vector3 RandomPositon()
    {
        int randmIndex = Random.Range(0, gridPositons.Count); // Listからランダムにポジションを1つ取得

        Vector3 randomPositon = gridPositons[randmIndex]; // randomPositonに座標を格納

        gridPositons.RemoveAt(randmIndex); // randmPositonに使用した要素をListから削除

        return randomPositon;
    }

    // オブジェクト生成関数
    void LayoutobjectAtRandom(GameObject[] objArray, int min, int max)
    {
        int objectCount = Random.Range(min, max + 1); // オブジェクト生成する数の決定

        for(int i = 0; i < objectCount; i++)
        {
            Vector3 randomPositon = RandomPositon();

            GameObject objChoice = objArray[Random.Range(0, objArray.Length)]; // 生成するオブジェクトをランダムで選出

            int randomQua = Random.Range(-180, 180);
            Instantiate(objChoice, randomPositon, Quaternion.Euler(0, randomQua, 0)); // 生成
        }
    }

    // ステージ生成
    public void SetupSecene(int depth)
    {
        BoardSetup();
        InitialiseList();
        LayoutobjectAtRandom(blockObj, blockMinimun, blockMaximum); // 障害物の生成
        LayoutobjectAtRandom(itemObj, itemMinimun, itemMaximum); // アイテムの生成


        int enemyCountRd = 2 + (int)Mathf.Log(depth, 2f);

        // enemyの最大出現数を5にする
        if(enemyCountRd > 5)
        {
            enemyCountRd = 5;
        }

        GameManager.instance.totalEnemy = enemyCountRd;

        LayoutoEnemyRandom(enemyObj, enemyCountRd, enemyCountRd ,depth); // エネミーの生成

        int randomX = Random.Range(0, 13);
        Instantiate(goalobj, new Vector3 (randomX,0,12), Quaternion.identity);

    }

    // enemyの生成関数
    void LayoutoEnemyRandom(GameObject[] objArray, int min, int max , int stageLV)
    {
        int objectCount = Random.Range(min, max + 1); // オブジェクト生成する数の決定


        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPositon = RandomPositon();

            GameObject objChoice = objArray[Random.Range(0, objArray.Length)]; // 生成するオブジェクトをランダムで選出

            Enemy enemy = objChoice.GetComponent<Enemy>();

            if(enemy.lv >= stageLV)
            {
                objChoice = objArray[0];
            }

            Instantiate(objChoice, randomPositon, Quaternion.Euler(0, 180, 0)); // 生成
        }
    }
}
