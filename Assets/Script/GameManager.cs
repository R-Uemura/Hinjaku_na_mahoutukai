using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    // �ŏ��̏���
    public static GameManager instance;  // �V���O���g�������Q�[���f�[�^���Ǘ�����I�u�W�F�N�g�̂��߁A��������Ƃ悭�Ȃ�
    BoardManager boardManager;

    public int depth = 1; // �i�s�X�e�[�W��
    public Text depthText;
    public GameObject depthImage;

    private List<Enemy> enemies; //�@�G���Ǘ����郊�X�g

    Player player;
    UIManager uIManager;

    bool playerTrun = true;
    bool enemyTrun = false;
    int enemycount = 0;

    // Plyaer�X�e�[�^�X
    public int playerlv = 1;
    public int stamina = 100;
    public int hp = 100;
    public int maxHP = 100;
    public int hpr = 5;
    public int strength = 3;
    public int magic = 12;
    public int getExp = 0;
    public int score = 0;
    public bool life = true;


    // Start�֐�������ɌĂт����
    private void Awake()
    {
        // GameManeger�̏d�����`�F�b�N
        if (instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);�@// ���̃I�u�W�F�N�g�͔j�󂳂�Ȃ��悤�ɂȂ�

        enemies = new List<Enemy>(); // ���X�g�̃C���X�^���X��

        Application.targetFrameRate = 30; // FPS30�ɌŒ�

        boardManager = GetComponent<BoardManager>();

        // Map����
        Initgame();
    }

    // �Q�[���J�n��Runtime�ɂ��Call���Ăяo����āAOnSceneLoded���o�^�����B
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] // �Q�[���J�n���ɂ��Ȃ炸�P�x�����Ăяo�����
    static public void Call()
    {
        SceneManager.sceneLoaded += OnSceneLoded;
    }

    // �v���C���[��Goal�ɂ����Ƃ��ɌĂяo����āA�}�b�v��������������
    static private void OnSceneLoded(Scene next,LoadSceneMode a)
    {
        instance.depth++;
        instance.Initgame();

    }

    // �Q�[�������p�̊֐�
    public void Initgame()
    {

        playerTrun = true;
        enemyTrun = false;

        depthImage = GameObject.Find("DepthImage");
        depthText = GameObject.Find("DepthText").GetComponent<Text>();
        depthText.text = "Depth : " + depth;

        depthImage.SetActive(true);

        Invoke("HideDepthImage", 2f); // 2�b��ɃL�����o�X������

        enemies.Clear(); // ������
        player = GameObject.Find("Player").GetComponent<Player>();
        uIManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        boardManager.SetupSecene(depth);
        uIManager.PlayerTurnTextUpdate();
    }

    // �X�e�[�W�̃C���[�W�摜������
    public void HideDepthImage()
    {
        depthImage.SetActive(false);
    }

    void Update()
    {

        // �v���C���[��AP0�ɂȂ�����^�[�����I��������
        if(player.isActionturn && player.ap < 1)
        {
            player.isActionturn = false;
            player.phasecount++;
            playerTrun = false;
            enemyTrun = true;
            enemycount = 0;
            uIManager.EnemyTurnTextUpdate();
        }

        // �G�̃^�[������
        if (enemyTrun)
        {
            if (player.isDown) // Player��Down�����̂�Enemy�^�[�����I��
            {
                enemyTrun = false;
            }

            if(enemycount == 0)
            {
                if (enemies.Count == enemycount)
                {
                    StartCoroutine("Waiting", 0.1f);

                    TurnChenge();
                }
                else
                {
                    enemycount++;
                    enemies[enemycount - 1].StartEnemyTurn();
                    uIManager.EnemyNameTextUPdate(enemies[enemycount - 1].EnemyName);
                }
            }
            else if(enemies[enemycount - 1].turnEnd)
            {
                if (enemies.Count == enemycount)
                {
                    StartCoroutine("Waiting", 0.1f);

                    TurnChenge();
                }
                else
                {
                    enemycount++;
                    enemies[enemycount - 1].StartEnemyTurn();
                    uIManager.EnemyNameTextUPdate(enemies[enemycount - 1].EnemyName);
                }
            }
            enemies[enemycount - 1].EnemyActionPhase();
        }

    }

    // �Ǘ����X�g�ɒǉ�����֐�
    public void AddEnemy(Enemy script)
    {
        enemies.Add(script); // Enemy�̃X�N���v�g�����X�g�Ɋi�[����
    }

    // �^�[���؂�ւ�
    private void TurnChenge()
    {
        enemyTrun = false;
        playerTrun = true;
        player.phasecount = 0;
        player.isCardDraw = true;
        uIManager.PlayerTurnTextUpdate();
    }

    // �R���[�`��
    private IEnumerable Waiting(float num)
    {
        yield return new WaitForSeconds(num);
    }

    // Player���|�ꂽ���̏���
    private void PlayerDown()
    {

    }

}
