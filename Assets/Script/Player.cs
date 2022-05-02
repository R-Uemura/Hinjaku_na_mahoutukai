using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    // カードの生成
    [SerializeField] Transform cardHandTransform; // 外部データの位置を受け取る
    [SerializeField] CardController cardPrefab; // 外部データのプレハブを受け取る

    public Animator animator;
    public GameObject[] skillObject;

    AttackController attackController;
    UIManager uiManager;
    MessageTextManager messageTextManager;

    // ターンコントロール変数
    public int phasecount;
    public bool isCardDraw = false;
    public bool isCardSelect = false;
    public bool isActionturn = false;

    // 移動関係変数
    bool buttonHold = false;
    bool isMoving = false;
    bool isDirection = false;
    public Text moveText;
    public Text directionText;

    public float moveSpeed = 0.05f; // 移動スピード
    int counter = 0; // 移動回数カウンター
    Vector3 targetPosition; // 移動完了座標格納用

    public GameObject cardHand;

    bool isAttack = false;
    bool isHit = false;
    public bool isDown = false;

    // 受取用ステータス
    public int lv;
    public int stamina;
    public int hp;
    public int maxHP;
    public int hpr;
    public int strength;
    public int magic;
    public int nextExp;
    public bool life = true;

    // AP
    public int ap = 0;

    // バトル制御変数
    public int power;
    float dmg;
    float attackTimer;
    float attackStartTimer;
    bool attackStart = false;
    float delayTime;
    float damageResetTime = 0.3f;
    float hitTimer = 0f;
    bool charge = false;
    int dmgBonus = 0;


    void Start()
    {
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        animator = GetComponent<Animator>(); // アニメーターの取得
        directionText.enabled = false; // direction表記を消す
        moveText.enabled = true; // moveを表示

        messageTextManager = this.GetComponent<MessageTextManager>();

        phasecount = 0;

        // playerステータスの初期化
        lv = GameManager.instance.playerlv;
        hp = GameManager.instance.hp;
        maxHP = GameManager.instance.maxHP;
        hpr = GameManager.instance.hpr;
        stamina = GameManager.instance.stamina;
        strength =  GameManager.instance.strength;
        magic = GameManager.instance.magic;
        nextExp = GameManager.instance.nextExp;
        life = GameManager.instance.life;

        for (int i = 0; i < 6; i++)
        {
            skillObject[i] = this.transform.GetChild(i + 1).gameObject;
        }

        StartGame();
        buttonHold = false;
        isCardDraw = true;
        isCardSelect = false;
        isActionturn = false;
        isAttack = false;
    }

    void StartGame()
    {
        // カードを5枚引く
        for (int i=0; i < 4; i++)
        {
            CreateCard(cardHandTransform);
        }
    }

    void Update()
    {
        if (isCardDraw)
        {
            if(phasecount == 1)
            {
                CreateCard(cardHandTransform);
                phasecount++;
            }
        }

        if (isActionturn)
        {
            if(phasecount == 5)
            {
                // ターン開始時にスタミナがある場合、HP回復
                if(stamina > 0)
                {
                    hp += hpr;
                    if(hp >= maxHP)
                    {
                        hp = maxHP;
                    }
                }
                uiManager.HPTextUpdate();
                phasecount++;
            }

            // 移動処理
            if (isMoving)
            {
                Vector3 mpos = new Vector3(0, 0, moveSpeed); // 移動量
                this.transform.Translate(mpos); // 移動
                counter++; // 移動回数

                if (counter >= 1 / moveSpeed)
                {
                    //　移動終了
                    this.transform.position = targetPosition; // 座標のずれをグローバル座標で上書き
                    counter = 0;
                    ap--;
                    uiManager.APTextUpdate();
                    
                    //　スタミナが0のとき歩く度にダメージ
                    if(stamina < 1)
                    {
                        hp--;

                        if (hp < 1)
                        {
                            hp = 1;
                        }

                        uiManager.HPTextUpdate();
                    }
                    else if(stamina > 0)
                    {
                        stamina--;
                        uiManager.StaminaTextUpdate();
                    }

                    // チャージ殴りボーナス
                    if (charge)
                    {
                        dmgBonus++;

                        if (dmgBonus > 6)
                        {
                            dmgBonus = 6;
                        }
                    }

                    SkillChange(ap);

                    // 移動ボタンを押したままの処理
                    if (buttonHold)
                    {
                        if (MoveCheck() && ap > 0)
                        {
                            isMoving = true;

                            // 向いている方向に合わせて移動先を設定
                            switch (transform.localEulerAngles.y)
                            {
                                case 0f:
                                    targetPosition = this.transform.position + new Vector3(0, 0, 1.0f);
                                    break;
                                case 90f:
                                    targetPosition = this.transform.position + new Vector3(1.0f, 0, 0);
                                    break;
                                case 180f:
                                    targetPosition = this.transform.position + new Vector3(0, 0, -1.0f);
                                    break;
                                case 270f:
                                    targetPosition = this.transform.position + new Vector3(-1.0f, 0, 0);
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            animator.SetBool("Walk", false);
                            isMoving = false;
                        }
                    }
                    else
                    {
                        animator.SetBool("Walk", false);
                        isMoving = false;
                    }
                }
            }

            // 攻撃モーション中の処理
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

                if(attackStartTimer < attackTimer && !attackStart)
                {
                    AttackStart();
                }

                if (damageResetTime < attackTimer)
                {
                    attackController.DamageReset();
                }
            }
        }

        // 被ダメージ処理
        if (isHit)
        {
            if (delayTime < hitTimer)
            {
                isHit = false;

                if (hp < 1)
                {
                    if (!isDown)
                    {
                        isDown = true;
                    }

                    hp = 0;
                    animator.SetBool("Down", true);
                }
                else
                {
                    animator.SetTrigger("Damage");
                }

                if(hpr < 1)
                {
                    hpr = 0;
                }
                uiManager.HPTextUpdate();
                uiManager.HPrTextUpdate();
            }
            else
            {
                hitTimer += Time.deltaTime;
            }
        }

        // 撃破されたあとの処理
        if (isDown)
        {
            GameManager.instance.PlayerDown();
        }

        // LvUP処理
        if(nextExp < 1)
        {

            lv++;
            maxHP += 4;
            hpr++;
            strength++;
            magic += 3;

            messageTextManager.OpenLevelUPText();
            uiManager.LevelTextUpdate();

            nextExp = 10 + ( 100 * ( lv / 10)) + lv;
        }
    }

    // 移動/向き変更切り替えボタン
    public void OnSwitching()
    {
        if(isMoving == false)
        {
            switch (isDirection)
            {
                case true:
                    isDirection = false;
                    directionText.enabled = false;
                    moveText.enabled = true;
                    break;

                case false:
                    isDirection = true;
                    moveText.enabled = false;
                    directionText.enabled = true;
                    break;
            }
        }
    }


    // 上ボタン押したとき
    public void InputUP()
    {
        if (!isMoving && ap > 0 && !isAttack)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            if(isDirection == false)
            {
                if (MoveCheck())
                {
                    isMoving = true;
                    targetPosition = this.transform.position + new Vector3(0, 0, 1.0f); // グローバル座標で最終移動先座標を記録
                }
            }

            buttonHold = true;
        }
    }


    //　下ボタン押したとき
    public void InputDown()
    {
        if (!isMoving && ap > 0 && !isAttack)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 180f, 0));
            if (isDirection == false)
            {
                if (MoveCheck())
                {
                    isMoving = true;
                    targetPosition = this.transform.position + new Vector3(0, 0, -1.0f); // グローバル座標で最終移動先座標を記録
                }
            }

            buttonHold = true;
        }
    }

    //　右ボタン押したとき
    public void InputRight()
    {
        if (!isMoving && ap > 0 && !isAttack)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 90f, 0));
            if (isDirection == false)
            {
                if (MoveCheck())
                {
                    isMoving = true;
                    targetPosition = this.transform.position + new Vector3(1.0f, 0, 0); // グローバル座標で最終移動先座標を記録
                }
            }

            buttonHold = true;
        }
    }

    //　左ボタン押したとき
    public void InputLeft()
    {
        if (!isMoving && ap > 0 && !isAttack)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 270f, 0));
            if (isDirection == false)
            {
                if (MoveCheck())
                {
                    isMoving = true;
                    targetPosition = this.transform.position + new Vector3(-1.0f, 0, 0); // グローバル座標で最終移動先座標を記録
                }
            }

            buttonHold = true;
        }
    }

    //　移動ボタンを離したとき
    public void ButtonOff()
    {
        buttonHold = false;
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
            if (hit.collider.tag == "Enemy")
            {
                return false;
            }
            else if(hit.collider.tag == "Blocking")
            {
                return false;
            }
        }

        animator.SetBool("Walk", true); // 移動アニメ開始
        return true;
    }

    // オブジェクト接触の判定
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            ItemManager itemManager = other.gameObject.GetComponent<ItemManager>();

            stamina += itemManager.stamina;
            maxHP += itemManager.maxhp;
            hpr += itemManager.hpr;
            strength += itemManager.strength;
            magic += itemManager.magic;
            ap += itemManager.ap;

            messageTextManager.OpenItemText(itemManager.itemName);

            if (!charge)
            {
                charge = itemManager.charge;
            }

            if(stamina > 100)
            {
                stamina = 100;
            }

            uiManager.GetItem();
            uiManager.ChargePanelChenge(charge);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Goal")
        {
            Invoke("Restart", 1f);

            enabled = false;
        }
    }

    // Playerが非常時に働く関数
    private void OnDisable()
    {
        // シーン遷移のためのデータ受け渡し
        GameManager.instance.playerlv = lv;
        GameManager.instance.hp = hp;
        GameManager.instance.maxHP = maxHP;
        GameManager.instance.hpr = hpr;
        GameManager.instance.stamina = stamina;
        GameManager.instance.strength = strength;
        GameManager.instance.magic = magic;
        GameManager.instance.nextExp = nextExp;
        GameManager.instance.life = life;
    }

    // 次のステージへの遷移関数
    public void Restart()
    {
        GameManager.instance.nowGame = true;
        GameManager.instance.depth++;
        SceneManager.LoadScene("MainGame");
    }

    // カード生成関数
    void CreateCard(Transform hand)
    {
        CardController card = Instantiate(cardPrefab, cardHandTransform, false);
        int cardID = Random.Range(1, 7);
        card.InitCard(cardID);
    }

    //　AP変動によるスキルの変更
    public void SkillChange(int ap)
    {
        for(int i = 0; i < 6; i++)
        {
            if (i == ap-1)
            {
                skillObject[i].gameObject.SetActive(true);
            }
            else
            {
                skillObject[i].gameObject.SetActive(false);
            }

            switch (ap)
            {
                case 1:
                    if (charge)
                    {
                        switch (dmgBonus)
                        {
                            case 1:
                                dmg = strength + magic;
                                break;
                            case 2:
                                dmg = strength * 1.03f + magic;
                                break;
                            case 3:
                                dmg = strength * 1.07f + magic;
                                break;
                            case 4:
                                dmg = strength * 1.15f + magic;
                                break;
                            case 5:
                                dmg = (strength + magic) * 1.2f;
                                break;
                            case 6:
                                dmg = (strength * 2 + magic) * 1.25f;
                                break;
                            default:
                                dmg = strength + magic;
                                break;
                        }
                    }
                    else
                    {
                        dmg = strength;
                    }
                    break;
                case 2:

                    if (charge)
                    {
                        dmg = magic + magic;
                    }
                    else
                    {
                        dmg = magic;
                    }
                    break;
                case 3:
                    dmg = 0;
                    break;
                case 4:
                    if (charge)
                    {
                        dmg = magic * 1.03f + magic;
                    }
                    else
                    {
                        dmg = magic * 1.03f;
                    }
                    break;
                case 5:
                    if (charge)
                    {
                        dmg = (magic + strength * 0.1f) * 1.05f + magic;
                    }
                    else
                    {
                        dmg = (magic + strength * 0.1f) * 1.05f;
                    }
                    break;
                case 6:
                    if (charge)
                    {
                        dmg = magic * 1.12f + magic;
                    }
                    else
                    {
                        dmg = magic * 1.12f;
                    }
                    break;
                default:
                    dmg = 0;
                    break;
            }
            uiManager.ActionTextUpdate(ap, (int)dmg, dmgBonus);
        }
    }

    // アクションボタンを押したとき
    public void ActionTap()
    {
        if(!isAttack && !isMoving)
        {
            switch (ap)
            {
                case 1:
                    animator.SetTrigger("Skill1");
                    delayTime = 1.8f;
                    attackStartTimer = 0.0f;
                    damageResetTime = 0.3f;

                    charge = false;
                    AttackTriggerON();

                    if (dmgBonus > 4)
                    {
                        strength += 2;
                    }

                    break;
                case 2:
                    animator.SetTrigger("Skill2");
                    delayTime = 1.8f;
                    attackStartTimer = 0.2f;
                    damageResetTime = attackStartTimer + 0.3f;
                    charge = false;
                    AttackTriggerON();
                    break;
                case 3:
                    animator.SetTrigger("Skill3");
                    delayTime = 1.8f;
                    attackStartTimer = 0.0f;
                    damageResetTime = attackStartTimer + 0.3f;
                    charge = true;
                    AttackTriggerON();
                    break;
                case 4:
                    animator.SetTrigger("Skill4");
                    delayTime = 1.8f;
                    attackStartTimer = 0.2f;
                    damageResetTime = attackStartTimer + 0.3f;
                    charge = false;
                    AttackTriggerON();
                    break;
                case 5:
                    animator.SetTrigger("Skill5");
                    delayTime = 2.0f;
                    attackStartTimer = 0.3f;
                    damageResetTime = attackStartTimer + 0.3f;
                    charge = false;
                    AttackTriggerON();
                    break;
                case 6:
                    animator.SetTrigger("Skill6");
                    delayTime = 2.5f;
                    attackStartTimer = 0.5f;
                    damageResetTime = attackStartTimer + 0.3f;
                    charge = false;
                    AttackTriggerON();
                    break;
                default:
                    break;
            }
        }
    }

    private void AttackTriggerON()
    {
        attackController = skillObject[ap - 1].GetComponent<AttackController>();
        isAttack = true;
        attackStart = false;
        attackTimer = 0;
        dmgBonus = 0;
        uiManager.ChargePanelChenge(charge);
        attackController.PlayerEffecktPlay();
    }

    private void AttackStart()
    {
        attackController.PlayerAttack((int)dmg);
        attackStart = true;
    }

    private void AttackTriggerOFF()
    {
        skillObject[ap - 1].gameObject.SetActive(false);
        ap = 0;
        isAttack = false;
        isMoving = false;
        uiManager.APTextUpdate();
        attackController.DamageReset();
    }

    // 被弾判定
    private void OnTriggerStay(Collider other)
    {
        if (!isHit)
        {
            if (other.tag == "EnemyAtJudg")
            {
                AttackController damageScript = other.GetComponent<AttackController>();
                if (damageScript != null)
                {
                    if (damageScript.attackflag)
                    {
                        hp = damageScript.PlayerDamage(hp);
                        hpr = damageScript.PlayerHPrDamage(hpr);

                        isHit = true;
                        delayTime = 0.5f;
                        hitTimer = 0;
                    }
                }
            }
        }
    }

    // 復活
    public void PlayerRevival()
    {
        life = false;

        hp = maxHP;
        hpr += lv;
        charge = false;

        stamina += 30;
        if(stamina > 100)
        {
            stamina = 100;
        }

        isDown = false;
        animator.SetBool("Down", false);
        messageTextManager.OnRevival();
    }
}
