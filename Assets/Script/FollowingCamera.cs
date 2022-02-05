using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// カメラを追従させるScript
public class FollowingCamera : MonoBehaviour
{
    public GameObject target; // プレイヤー情報
    public Vector3 offset; // 相対距離

    void Start()
    {
        // ターゲットの取得
        this.target = GameObject.Find("Player");

        // カメラとターゲットの位置を求める
        // offset = transform.position - target.transform.position;
    }

    void Update()
    {
        // 位置の更新
        transform.position = target.transform.position + offset;
    }
}
