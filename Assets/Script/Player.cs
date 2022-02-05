using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    // �J�[�h�̐���
    [SerializeField] Transform cardHandTransform; // �O���f�[�^�̈ʒu���󂯎��
    [SerializeField] CardController cardPrefab; // �O���f�[�^�̃v���n�u���󂯎��

    public Animator animator;
    public GameObject[] skillObject;

    AttackController attackController;
    UIManager uiManager;

    // �^�[���R���g���[���ϐ�
    public int phasecount;
    public bool isCardDraw = false;
    public bool isCardSelect = false;
    public bool isActionturn = false;

    // �ړ��֌W�ϐ�
    bool isMoving = false;
    bool isDirection = false;
    public Text moveText;
    public Text directionText;

    public float moveSpeed = 0.05f; // �ړ��X�s�[�h
    int counter = 0; // �ړ��񐔃J�E���^�[
    Vector3 targetPosition; // �ړ��������W�i�[�p

    public GameObject cardHand;

    bool isAttack = false;
    bool isHit = false;
    public bool isDown = false;

    // ���p�X�e�[�^�X
    public int lv;
    public int stamina;
    public int hp;
    public int maxHP;
    public int hpr;
    public int strength;
    public int magic;
    public int getExp;
    public int score;
    public bool life = true;

    // AP
    public int ap = 0;

    // �o�g������ϐ�
    float dmg;
    float attackTimer;
    float delayTime;
    float damageResetTime = 0.3f;
    float hitTimer = 0f;


    void Start()
    {
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        animator = GetComponent<Animator>(); // �A�j���[�^�[�̎擾
        directionText.enabled = false; // direction�\�L������
        moveText.enabled = true; // move��\��

        phasecount = 0;

        // player�X�e�[�^�X�̏�����
        lv = GameManager.instance.playerlv;
        hp = GameManager.instance.hp;
        maxHP = GameManager.instance.maxHP;
        hpr = GameManager.instance.hpr;
        stamina = GameManager.instance.stamina;
        strength =  GameManager.instance.strength;
        magic = GameManager.instance.magic;
        getExp = GameManager.instance.getExp;
        score = GameManager.instance.score;
        life = GameManager.instance.life;

        for (int i = 0; i < 6; i++)
        {
            skillObject[i] = this.transform.GetChild(i + 1).gameObject;
        }

        StartGame();
        isCardDraw = true;
        isCardSelect = false;
        isActionturn = false;
        isAttack = false;
    }

    void StartGame()
    {
        // �J�[�h��5������
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
                // �^�[���J�n���ɃX�^�~�i������ꍇ�AHP��
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

            // �ړ�����
            if (isMoving)
            {
                animator.SetBool("Walk", true); // �ړ��A�j���J�n

                Vector3 mpos = new Vector3(0, 0, moveSpeed); // �ړ���
                this.transform.Translate(mpos); // �ړ�
                counter++; // �ړ���

                if (counter >= 1 / moveSpeed)
                {
                    //�@�ړ��I��
                    this.transform.position = targetPosition; // ���W�̂�����O���[�o�����W�ŏ㏑��
                    counter = 0;
                    animator.SetBool("Walk", false);
                    ap--;
                    uiManager.APTextUpdate();
                    SkillChange(ap);

                    //�@�X�^�~�i��0�̂Ƃ������x�Ƀ_���[�W
                    if(stamina < 1)
                    {
                        hp--;
                        uiManager.HPTextUpdate();
                    }else if(stamina > 0)
                    {
                        stamina--;
                        uiManager.StaminaTextUpdate();
                    }

                    isMoving = false;
                }
            }

            // �U�����[�V�������̏���
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

                if (damageResetTime < attackTimer)
                {
                    attackController.DamageReset();
                }
            }
        }

        // ��_���[�W����
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

        // ���j���ꂽ���Ƃ̏���
        if (isDown)
        {
            
        }

        // LvUP����
        if(getExp > 100)
        {
            getExp = 0;

            lv++;
            maxHP += 5;
            hpr++;
            strength++;
            magic += 5;

            score += lv;

            uiManager.LevelTextUpdate();
        }
    }

    // �ړ�/�����ύX�؂�ւ��{�^��
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


    // ��{�^���������Ƃ�
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
                    targetPosition = this.transform.position + new Vector3(0, 0, 1.0f); // �O���[�o�����W�ōŏI�ړ�����W���L�^
                }
            }
        }
    }

    //�@���{�^���������Ƃ�
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
                    targetPosition = this.transform.position + new Vector3(0, 0, -1.0f); // �O���[�o�����W�ōŏI�ړ�����W���L�^
                }
            }
        }
    }

    //�@�E�{�^���������Ƃ�
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
                    targetPosition = this.transform.position + new Vector3(1.0f, 0, 0); // �O���[�o�����W�ōŏI�ړ�����W���L�^
                }
            }
        }
    }

    //�@���{�^���������Ƃ�
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
                    targetPosition = this.transform.position + new Vector3(-1.0f, 0, 0); // �O���[�o�����W�ōŏI�ړ�����W���L�^
                }
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
            if (hit.collider.tag == "Enemy")
            {
                return false;
            }
            else if(hit.collider.tag == "Blocking")
            {
                return false;
            }
        }
        return true;
    }

    // �I�u�W�F�N�g�ڐG�̔���
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
            getExp += itemManager.getExp;

            if(stamina > 100)
            {
                stamina = 100;
            }

            uiManager.GetItem();

            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Goal")
        {
            Invoke("Restart", 1f);

            enabled = false;
        }
    }

    // Player����펞�ɓ����֐�
    private void OnDisable()
    {
        // �V�[���J�ڂ̂��߂̃f�[�^�󂯓n��
        GameManager.instance.playerlv = lv;
        GameManager.instance.hp = hp;
        GameManager.instance.maxHP = maxHP;
        GameManager.instance.hpr = hpr;
        GameManager.instance.stamina = stamina;
        GameManager.instance.strength = strength;
        GameManager.instance.magic = magic;
        GameManager.instance.getExp = getExp;
        GameManager.instance.score = score;
        GameManager.instance.life = life;
    }

    // ���̃X�e�[�W�ւ̑J�ڊ֐�
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    // �J�[�h�����֐�
    void CreateCard(Transform hand)
    {
        CardController card = Instantiate(cardPrefab, cardHandTransform, false);
        int cardID = Random.Range(1, 7);
        card.InitCard(cardID);
    }

    //�@AP�ϓ��ɂ��X�L���̕ύX
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
            uiManager.ActionTextUpdate(ap);
        }
    }

    // ��ʉE�̃{�^�����������Ƃ�
    public void ActionTap()
    {
        if(!isAttack && !isMoving && ap > 0)
        {
            switch (ap)
            {
                case 1:
                    animator.SetTrigger("Skill1");
                    dmg = strength;
                    AttackTriggerON(dmg);
                    break;
                case 2:
                    animator.SetTrigger("Skill2");
                    dmg = magic;
                    AttackTriggerON(dmg);
                    break;
                case 3:
                    animator.SetTrigger("Skill3");
                    dmg = 0;
                    AttackTriggerON(dmg);
                    break;
                case 4:
                    animator.SetTrigger("Skill4");
                    dmg = magic * 1.08f;
                    AttackTriggerON(dmg);
                    break;
                case 5:
                    animator.SetTrigger("Skill5");
                    dmg = (magic + strength * 0.80f) * 1.12f;
                    AttackTriggerON(dmg);
                    break;
                case 6:
                    animator.SetTrigger("Skill6");
                    dmg = magic * 1.40f;
                    AttackTriggerON(dmg);
                    break;
                default:
                    break;
            }
        }
    }

    private void AttackTriggerON(float dmg)
    {
        attackController = skillObject[ap - 1].GetComponent<AttackController>();
        attackController.Damager((int)dmg);
        isAttack = true;
        attackTimer = 0;
        delayTime = 1.2f;
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

    // ��e����
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
}
