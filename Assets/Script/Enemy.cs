using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    private BoxCollider boxCollider;
    Player player;

    // �ړ��֌W�ϐ�
    bool isMoving = false;
    public float moveSpeed = 0.1f; // �ړ��X�s�[�h
    int counter = 0; // �ړ��񐔃J�E���^�[
    public Vector3 targetPosition; // �ړ��������W�i�[�p

    private Transform playerPosition;
    public GameObject enemyAttackRange;
    AttackController attackController;
    EnemyHPtext enemyHPtext;

    // �Q�[���X�e�[�^�X
    public string EnemyName = "";
    public int lv = 1;
    public int hp = 5;
    public int attack = 1;
    public int hprattack = 0;
    public int maxActionCount = 1;
    public int acquiredExp = 10;
    private int actionCount = 0;

    // ���x���ɂ��X�e�[�^�X�㏸�l(+�t��)
    public int boostflag = 10;
    public int boostHpRiseValue = 5;
    public int boostAttackRiseValue = 3;
    public float boostHprattackRiseValue = 0.1f;

    // �n�C���x���A�b�v���̃X�e�[�^�X�㏸(���t��)
    public int hiLevelflag = 25;
    public int hiLevelhp = 100;
    public int hiLevelattack = 5;
    public int hiLevelhprattack = 30;
    public int hiLevelmaxActionCount = 1;
    public int hiLevelacquiredExp = 20;

    // �n�C���x���A�b�v���̃X�e�[�^�X�㏸(���t��)
    public int deepflag = 50;
    public int deephp = 100;
    public int deepattack = 5;
    public float deephprattack = 5.0f;

    // �^�[������ϐ�
    bool isAttack = false;
    public bool turnEnd = false;

    // �o�g������ϐ�
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
        GameManager.instance.AddEnemy(this); // �������Q�[���}�l�[�W���[�̃��X�g�ɓn��

        animator = GetComponent<Animator>(); // �A�j���[�^�[�̎擾

        playerPosition = GameObject.FindGameObjectWithTag("Player").transform; // tag���v���C���[�̃I�u�W�F�N�g������

        boxCollider = GetComponent<BoxCollider>();

        attackController = enemyAttackRange.GetComponent<AttackController>();
        player = GameObject.Find("Player").GetComponent<Player>();

        int stage = GameManager.instance.depth;

        EnemyLvUP(stage);

        enemyHPtext = this.gameObject.GetComponent<EnemyHPtext>();
        enemyHPtext.GetEnemyMaxHP(hp);

        turnEnd = true;
    }

    void Update()
    {
        // �ړ�����
        if (isMoving)
        {
            if(maxActionCount == actionCount)
            {
                animator.SetBool("Walk", true); // �ړ��A�j���J�n
            }

            Vector3 mpos = new Vector3(0, 0, moveSpeed); // �ړ���
            this.transform.Translate(mpos); // �ړ�
            counter++; // �ړ���

            if (counter >= 1 / moveSpeed)
            {
                //�@�ړ��I��
                this.transform.position = targetPosition; // ���W�̂�����O���[�o�����W�ŏ㏑��
                counter = 0;
                isMoving = false;
                actionCount--;

                TurnCheck();
            }
        }

        // �U���\�����f����
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

        //�@�U������
        if (isAttack)
        {
            //�@�U�����o���ԏ���
            if (delayTime > attackTimer)
            {
                attackTimer += Time.deltaTime;
            }
            else
            {
                AttackTriggerOFF();
            }

            //�@AttackController�̃_���[�W���������
            if (damageResetTime < attackTimer)
            {
                attackController.DamageReset();
            }
        }

        // ��_���[�W����
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
                    boxCollider.enabled = false; // ����������A�ړ��̎ז��ɂȂ�Ȃ�����

                    enemyHPtext.EnemyHPtexthide();
                    EnemyName = "";

                    player.nextExp -= acquiredExp;
                    GameManager.instance.totalEnemy--;
                }
                else
                {
                    animator.SetTrigger("Hit");
                    isHit = false;
                    enemyHPtext.EnemyHPtextUpdate(hp);
                }
            }
            else
            {
                hitTimer += Time.deltaTime;
            }
        }

        // ���j���ꂽ���Ƃ̏���
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

    // �ړ��۔���
    bool MoveCheck()
    {
        // Ray�錾
        RaycastHit hit;

        Ray ray = new Ray(transform.position, transform.forward);

        //�@ray�̔���
        if (Physics.Raycast(ray, out hit, 1.0f))
        {
            // ��Q�������邩��ړ����Ȃ�
            if (hit.collider.tag == "Enemy" || hit.collider.tag == "Blocking" || hit.collider.tag == "Player")
            {
                return false;
            }
        }
        return true;
    }

    // �s�����߇@�U�����聨�A�ړ�
    // �U���ł����ꍇ�͈ړ������ɍU������
    public void EnemyActionPhase()
    {
        if(!isAttackCheckPhese && !attackCheckPheseEnd) // �U������J�n�̍��}
        {
            AttackCheckStart();
        }
        else if (attackCheckPheseEnd) // �U������I����̈ړ�����
        {
            if(!isMoving && !isAttack && actionCount > 0)
            {
                if (playerPosition.position.z - transform.position.z > 0)
                {
                    UPmove();
                }
                else if (playerPosition.position.x - transform.position.x > 0)
                {
                    Rightmove();
                }
                else if (playerPosition.position.x - transform.position.x < 0)
                {
                    Leftmove();
                }
                else if (playerPosition.position.z - transform.position.z < 0)
                {
                    Downmove();
                }

            }
        }

    }

    // �����ύX
    private void EnemyDirection()
    {
        if (playerPosition.position.z - transform.position.z > 0)
        {
            // �����
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if (playerPosition.position.x - transform.position.x > 0)
        {
            // �E����
            transform.rotation = Quaternion.Euler(new Vector3(0, 90f, 0));
        }
        else if (playerPosition.position.x - transform.position.x < 0)
        {
            // ������
            transform.rotation = Quaternion.Euler(new Vector3(0, 270f, 0));
        }
        else if (playerPosition.position.z - transform.position.z < 0)
        {
            // ������
            transform.rotation = Quaternion.Euler(new Vector3(0, 180f, 0));
        }
    }

    // player����ʏ�ɂ���ꍇ
    private void UPmove()
    {
        EnemyDirection();
        isMoving = MoveCheck();
        if (!isMoving)
        {
            if (playerPosition.position.z - transform.position.z < 0)
            {
                // ������
                transform.rotation = Quaternion.Euler(new Vector3(0, 180f, 0));
                targetPosition = this.transform.position + new Vector3(0, 0, -1.0f);
            }
            else if (playerPosition.position.x - transform.position.x >= 0)
            {
                // �E����
                transform.rotation = Quaternion.Euler(new Vector3(0, 90f, 0));
                targetPosition = this.transform.position + new Vector3(1.0f, 0, 0);
            }
            else if (playerPosition.position.x - transform.position.x < 0)
            {
                // ������
                transform.rotation = Quaternion.Euler(new Vector3(0, 270f, 0));
                targetPosition = this.transform.position + new Vector3(-1.0f, 0, 0);
            }

            isMoving = MoveCheck();
            if(!isMoving)
            {
                actionCount--;
                TurnCheck();
            }
        }
        else
        {
            targetPosition = this.transform.position + new Vector3(0, 0, 1.0f); // �O���[�o�����W�ōŏI�ړ�����W���L�^
        }
    }

    // player����ʉ��ɂ���ꍇ
    private void Downmove()
    {
        EnemyDirection();
        isMoving = MoveCheck();
        if (!isMoving)
        {
            if (playerPosition.position.z - transform.position.z > 0)
            {
                // �����
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                targetPosition = this.transform.position + new Vector3(0, 0, 1.0f);
            }
            else if (playerPosition.position.x - transform.position.x > 0)
            {
                // �E����
                transform.rotation = Quaternion.Euler(new Vector3(0, 90f, 0));
                targetPosition = this.transform.position + new Vector3(1.0f, 0, 0);
            }
            else if (playerPosition.position.x - transform.position.x <= 0)
            {
                // ������
                transform.rotation = Quaternion.Euler(new Vector3(0, 270f, 0));
                targetPosition = this.transform.position + new Vector3(-1.0f, 0, 0);
            }

            isMoving = MoveCheck();
            if (!isMoving)
            {
                actionCount--;
                TurnCheck();
            }
        }
        else
        {
            targetPosition = this.transform.position + new Vector3(0, 0, -1.0f); // �O���[�o�����W�ōŏI�ړ�����W���L�^
        }
    }

    // player����ʉE�ɂ���ꍇ
    private void Rightmove()
    {
        EnemyDirection();
        isMoving = MoveCheck();
        if (!isMoving)
        {
            if (playerPosition.position.z - transform.position.z > 0)
            {
                // �����
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                targetPosition = this.transform.position + new Vector3(0, 0, 1.0f);
            }
            else if (playerPosition.position.z - transform.position.z <= 0)
            {
                // ������
                transform.rotation = Quaternion.Euler(new Vector3(0, 180f, 0));
                targetPosition = this.transform.position + new Vector3(0, 0, -1.0f);
            }
            else if (playerPosition.position.x - transform.position.x < 0)
            {
                // ������
                transform.rotation = Quaternion.Euler(new Vector3(0, 270f, 0));
                targetPosition = this.transform.position + new Vector3(-1.0f, 0, 0);
            }

            isMoving = MoveCheck();
            if (!isMoving)
            {
                actionCount--;
                TurnCheck();
            }
        }
        else
        {
            targetPosition = this.transform.position + new Vector3(1.0f, 0, 0); // �O���[�o�����W�ōŏI�ړ�����W���L�^
        }
    }

    // player����ʍ��ɂ���ꍇ
    private void Leftmove()
    {
        EnemyDirection();
        isMoving = MoveCheck();
        if (!isMoving)
        {
            if (playerPosition.position.z - transform.position.z >= 0)
            {
                // �����
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                targetPosition = this.transform.position + new Vector3(0, 0, 1.0f);
            }
            else if (playerPosition.position.z - transform.position.z < 0)
            {
                // ������
                transform.rotation = Quaternion.Euler(new Vector3(0, 180f, 0));
                targetPosition = this.transform.position + new Vector3(0, 0, -1.0f);
            }
            else if (playerPosition.position.x - transform.position.x > 0)
            {
                // �E����
                transform.rotation = Quaternion.Euler(new Vector3(0, 90f, 0));
                targetPosition = this.transform.position + new Vector3(1.0f, 0, 0);
            }

            isMoving = MoveCheck();
            if (!isMoving)
            {
                actionCount--;
                TurnCheck();
            }
        }
        else
        {
            targetPosition = this.transform.position + new Vector3(-1.0f, 0, 0); // �O���[�o�����W�ōŏI�ړ�����W���L�^
        }
    }

    //�@�^�[���J�n���̍s���t���O����������
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

    // ��e����
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

                        isHit = true;
                        delayTime = 0.5f;
                        hitTimer = 0;
                    }
                }
            }
        }
    }

    // �U���J�n
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
            delayTime = 2.0f;
        }
    }

    // �U���I��
    private void AttackTriggerOFF()
    {
        isAttack = false;
        isMoving = false;
        actionCount--;
        attackController.DamageReset();
        TurnCheck();
    }

    // �U���ł���ʒu�Ƀv���C���[�����邩�m�F���J�n����֐�
    private void AttackCheckStart()
    {
        if (Mathf.Abs(playerPosition.position.z - transform.position.z) < 1) // z�̍��̐�Βl��1��菬����
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
        else if (Mathf.Abs(playerPosition.position.x - transform.position.x) < 1) // x�̍��̐�Βl��1��菬����
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

        // �A�^�b�N�`�F�b�N���J�n����bool����
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

    // �K�w�ɂ��X�e�[�^�X�̏㏸
    public void EnemyLvUP(int depth)
    {
        int playerLv = GameManager.instance.playerlv;

        if (playerLv > boostflag)
        {
            hp += (playerLv - boostflag) * boostHpRiseValue;
            attack += (playerLv - boostflag) * boostAttackRiseValue;
            hprattack += (int)( (playerLv - boostflag) * boostHprattackRiseValue);
        }

        if (depth > hiLevelflag)
        {
            hp += hiLevelhp;
            attack += hiLevelattack;
            hprattack += hiLevelhprattack;
            maxActionCount += hiLevelmaxActionCount;
            acquiredExp += hiLevelacquiredExp;
        }

        if (depth > deepflag)
        {
            hp += (depth - deepflag) * deephp;
            attack += (depth - deepflag) * deepattack;
            hprattack += (int)((depth - deepflag) * deephprattack);
        }

        if(depth > deepflag)
        {
            EnemyName += "��";
        }
        else if(depth > hiLevelflag)
        {
            EnemyName += "��";
        }
        else if(playerLv > boostflag)
        {
            EnemyName += "�{";
        }
    }
}
