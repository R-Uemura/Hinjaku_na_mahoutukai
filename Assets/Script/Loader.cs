using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//　シーン開始毎にゲームマネージャーを取得するクラス
public class Loader : MonoBehaviour
{
    // ゲームマネージャーの取得
    public GameObject gameManager;

    public void Awake()
    {
        if(GameManager.instance == null)
        {
            Instantiate(gameManager); // ゲームマネージャーの取得
        }
    }

}
