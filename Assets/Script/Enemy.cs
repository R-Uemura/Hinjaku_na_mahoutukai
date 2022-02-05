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
    Vector3 targetPosition; // �ړ��������W�i�[�p

    private Transform playerPosition;
    public GameObject enemyAttackRange;
    AttackController attackController;

    // �Q�[���X�e�[�^�X
    public string EnemyName = "";
    public int lv = 1;
    public int hp = 5;
    public int attack = 1;
    public int hprattack = 0;
    public int maxActionCount = 1;
    public int acquiredExp = 10;
    public int acquiredScore = 3;
    private int actionCount = 0;

    // ���x���ɂ��X�e�[�^�X�㏸�l
    public int hpRiseValue = 5;
    public int attackRiseValue = 1;
    public float hprattackRiseValue = 0.1f;

    // �n�C���x���A�b�v���̃X�e�[�^�X�㏸
    public int deepflag = 25;
    public int deephp = 100;
    public int deepattack = 5;
    public int deephprattack = 30;
    public int deepmaxActionCount = 1;

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
                // animator.SetBool("Walk", false);
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

    // �����ύX
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

    // player����ʏ�ɂ���ꍇ
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
                        player.score += dmg;

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
            delayTime = 1.5f;
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

            EnemyName += "��";
        }
    }
}
