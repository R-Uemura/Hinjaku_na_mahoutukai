using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    private BoxCollider boxCollider;
    Player player;

    // 移動関係変数
    bool isMoving = false;
    public float moveSpeed = 0.1f; // 移動スピード
    int counter = 0; // 移動回数カウンター
    Vector3 targetPosition; // 移動完了座標格納用

    private Transform playerPosition;
    public GameObject enemyAttackRange;
    AttackController attackController;

    // ゲームステータス
    public string EnemyName = "";
    public int lv = 1;
    public int hp = 5;
    public int attack = 1;
    public int hprattack = 0;
    public int maxActionCount = 1;
    public int acquiredExp = 10;
    public int acquiredScore = 3;
    private int actionCount = 0;

    // レベルによるステータス上昇値
    public int hpRiseValue = 5;
    public int attackRiseValue = 1;
    public float hprattackRiseValue = 0.1f;

    // ハイレベルアップ時のステータス上昇
    public int deepflag = 25;
    public int deephp = 100;
    public int deepattack = 5;
    public int deephprattack = 30;
    public int deepmaxActionCount = 1;

    // ターン制御変数
    bool isAttack = false;
    public bool turnEnd = false;

    // バトル制御変数
    bool isHit = false;
    public bool isDown = false;
    bool isAttackCheckPhese = false;
    bool attackCheckPheseEnd = false;
    bool canAttack = false;
    float attackTimer;
    float hitTimer;
    float delayTime;
    float downTimer = 0;
    float downTime = 10.0f;
    float damageResetTime = 0.3f;
    float attckCheckTimer = 0;
    int dmg;


    void Start()
    {
        GameManager.instance.AddEnemy(this); // 自分をゲームマネージャーのリストに渡す

        animator = GetComponent<Animator>(); // アニメーターの取得

        playerPosition = GameObject.FindGameObjectWithTag("Player").transform; // tagがプレイヤーのオブジェクトを検索

        boxCollider = GetComponent<BoxCollider>();

        attackController = enemyAttackRange.GetComponent<AttackController>();
        player = GameObject.Find("Player").GetComponent<Player>();

        int stage = GameManager.instance.depth;

        EnemyLvUP(stage);

        turnEnd = true;
    }

    void Update()
    {
        // 移動処理
        if (isMoving)
        {
            if(maxActionCount == actionCount)
            {
                animator.SetBool("Walk", true); // 移動アニメ開始
            }

            Vector3 mpos = new Vector3(0, 0, moveSpeed); // 移動量
            this.transform.Translate(mpos); // 移動
            counter++; // 移動回数

            if (counter >= 1 / moveSpeed)
            {
                //　移動終了
                this.transform.position = targetPosition; // 座標のずれをグローバル座標で上書き
                counter = 0;
                isMoving = false;
                // animator.SetBool("Walk", false);
                actionCount--;

                TurnCheck();
            }
        }

        // 攻撃可能化判断する
        if (isAttackCheckPhese && !turnEnd)
        {
            if (canAttack)
            {
                AttackTriggerON();
            }

            if(0.1f > attckCheckTimer)
            {
                attckCheckTimer += Time.deltaTime;
            }
            else
            {
                isAttackCheckPhese = false;
                attackCheckPheseEnd = true;
            }
        }

        //　攻撃処理
        if (isAttack)
        {
            //　攻撃演出時間処理
            if (delayTime > attackTimer)
            {
                attackTimer += Time.deltaTime;
            }
            else
            {
                AttackTriggerOFF();
            }

            //　AttackControllerのダメージ判定を消す
            if (damageResetTime < attackTimer)
            {
                attackController.DamageReset();
            }
        }

        // 被ダメージ処理
        if (isHit)
        {
            if (delayTime < hitTimer)
            {
                if (hp < 1)
                {
                    hp = 0;
                    animator.SetBool("Down", true);
                    isDown = true;
                    isHit = false;
                    boxCollider.enabled = false; // 判定を消し、移動の邪魔にならなくする

                    EnemyName = "";

                    player.getExp += acquiredExp;
                    player.score += acquiredScore;
                }
                else
                {
                    animator.SetTrigger("Hit");
                    isHit = false;
                }
            }
            else
            {
                hitTimer += Time.deltaTime;
            }
        }

        // 撃破されたあとの処理
        if (isDown)
        {
            if (downTime > downTimer)
            {
                downTimer += Time.deltaTime;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

    }

    // 移動可否判定
    bool MoveCheck()
    {
        // Ray宣言
        RaycastHit hit;

        Ray ray = new Ray(transform.position, transform.forward);

        //　rayの判定
        if (Physics.Raycast(ray, out hit, 1.0f))
        {
            // 障害物があるから移動しない
            if (hit.collider.tag == "Enemy" || hit.collider.tag == "Blocking" || hit.collider.tag == "Player")
            {
                return false;
            }
        }
        return true;
    }

    // 行動決め①攻撃判定→②移動
    // 攻撃できた場合は移動せずに攻撃する
    public void EnemyActionPhase()
    {
        if(!isAttackCheckPhese && !attackCheckPheseEnd) // 攻撃判定開始の合図
        {
            AttackCheckStart();
        }
        else if (attackCheckPheseEnd) // 攻撃判定終了後の移動処理
        {
            if(!isMoving && !isAttack && actionCount > 0)
            {
                if (playerPosition.position.z - transform.position.z > 0)
                {
                    UPmove();
                }
                else if (playerPosition.position.z - transform.position.z < 0)
                {
                    Downmove();
                }
                else if (playerPosition.position.x - transform.position.x > 0)
                {
                    Rightmove();
                }
                else if (playerPosition.position.x - transform.position.x < 0)
                {
                    Leftmove();
                }
            }
        }

    }

    // 向き変更
    private void EnemyDirection()
    {
        if (playerPosition.position.z - transform.position.z > 0)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if (playerPosition.position.z - transform.position.z < 0)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 180f, 0));
        }
        else if (playerPosition.position.x - transform.position.x > 0)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 90f, 0));
        }
        else if (playerPosition.position.x - transform.position.x < 0)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 270f, 0));
        }
    }

    // playerが画面上にいる場合
    private void UPmove()
    {
        EnemyDirection();
        isMoving = MoveCheck();
        if (!isMoving)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 270f, 0));
            isMoving = MoveCheck();
            if (isMoving)
            {
                targetPosition = this.transform.position + new Vector3(-1.0f, 0, 0);
            }
            else
            {
                actionCount--;
                TurnCheck();
            }
        }
        else
        {
            targetPosition = this.transform.position + new Vector3(0, 0, 1.0f); // グローバル座標で最終移動先座標を記録
        }
    }

    // playerが画面下にいる場合
    private void Downmove()
    {
        EnemyDirection();
        isMoving = MoveCheck();
        if (!isMoving)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 270f, 0));
            isMoving = MoveCheck();
            if (isMoving)
            {
                targetPosition = this.transform.position + new Vector3(-1.0f, 0, 0);
            }
            else
            {
                actionCount--;
                TurnCheck();
            }
        }
        else
        {
            targetPosition = this.transform.position + new Vector3(0, 0, -1.0f); // グローバル座標で最終移動先座標を記録
        }
    }

    // playerが画面右にいる場合
    private void Rightmove()
    {
        EnemyDirection();
        isMoving = MoveCheck();
        if (!isMoving)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 180f, 0));
            isMoving = MoveCheck();
            if (isMoving)
            {
                targetPosition = this.transform.position + new Vector3(0, 0, -1.0f);
            }
            else
            {
                actionCount--;
                TurnCheck();
            }
        }
        else
        {
            targetPosition = this.transform.position + new Vector3(1.0f, 0, 0); // グローバル座標で最終移動先座標を記録
        }
    }

    // playerが画面左にいる場合
    private void Leftmove()
    {
        EnemyDirection();
        isMoving = MoveCheck();
        if (!isMoving)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 180f, 0));
            isMoving = MoveCheck();
            if (isMoving)
            {
                targetPosition = this.transform.position + new Vector3(0, 0, -1.0f);
            }
            else
            {
                actionCount--;
                TurnCheck();
            }
        }
        else
        {
            targetPosition = this.transform.position + new Vector3(-1.0f, 0, 0); // グローバル座標で最終移動先座標を記録
        }
    }

    //　ターン開始時の行動フラグ初期化処理
    public void StartEnemyTurn()
    {
        if (!isDown)
        {
            turnEnd = false;
            isMoving = false;
            isAttack = false;
            isAttackCheckPhese = false;
            attackCheckPheseEnd = false;

            actionCount = maxActionCount;
        }
        else
        {
            turnEnd = true;
        }
    }

    private void TurnCheck()
    {
        isAttackCheckPhese = false;
        attackCheckPheseEnd = false;

        if (actionCount == 0)
        {
            animator.SetBool("Walk", false);

            turnEnd = true;
        }
    }

    // 被弾判定
    private void OnTriggerStay(Collider other)
    {
        if (!isHit)
        {
            if (other.tag == "PlayerAtJudg")
            {
                AttackController damageScript = other.GetComponent<AttackController>();
                if (damageScript != null)
                {
                    if (damageScript.attackflag)
                    {
                        dmg = damageScript.EnemyDamage(dmg);

                        hp -= dmg;
                        player.score += dmg;

                        isHit = true;
                        delayTime = 0.5f;
                        hitTimer = 0;
                    }
                }
            }
        }
    }

    // 攻撃開始
    private void AttackTriggerON()
    {
        if (!isDown)
        {
            animator.SetBool("Walk", false);

            animator.SetTrigger("Attack");
            attackController.Damager(attack);
            attackController.HprDamager(hprattack);
            isAttack = true;
            isAttackCheckPhese = false;
            attackCheckPheseEnd = true;
            attackTimer = 0;
            delayTime = 1.5f;
        }
    }

    // 攻撃終了
    private void AttackTriggerOFF()
    {
        isAttack = false;
        isMoving = false;
        actionCount--;
        attackController.DamageReset();
        TurnCheck();
    }

    // 攻撃できる位置にプレイヤーがいるか確認を開始する関数
    private void AttackCheckStart()
    {
        if (Mathf.Abs(playerPosition.position.z - transform.position.z) < 1) // zの差の絶対値が1より小さい
        {
            if(playerPosition.position.x - transform.position.x > 0)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 90f, 0));
            }
            else
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 270f, 0));
            }
        }
        else if (Mathf.Abs(playerPosition.position.x - transform.position.x) < 1) // xの差の絶対値が1より小さい
        {
            if (playerPosition.position.z - transform.position.z > 0)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 180f, 0));
            }
        }

        // アタックチェックを開始するbool判定
        isAttackCheckPhese = true;
        attackCheckPheseEnd = false;
        attckCheckTimer = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canAttack = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canAttack = false;
        }
    }

    // 階層によるステータスの上昇
    public void EnemyLvUP(int depth)
    {
        int playerLv = GameManager.instance.playerlv;

        if (lv < playerLv)
        {
            hp += playerLv * hpRiseValue;
            attack += playerLv * attackRiseValue;
            hprattack += (int)(playerLv * hprattackRiseValue);
        }

        if (depth > deepflag)
        {
            hp += deephp;
            attack += deepattack;
            hprattack += deephprattack;
            maxActionCount += deepmaxActionCount;

            EnemyName += "★";
        }
    }
}
