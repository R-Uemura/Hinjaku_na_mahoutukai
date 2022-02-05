using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    // 最初の処理
    public static GameManager instance;  // シングルトン化＝ゲームデータを管理するオブジェクトのため、複数あるとよくない
    BoardManager boardManager;

    public int depth = 1; // 進行ステージ数
    public Text depthText;
    public GameObject depthImage;

    private List<Enemy> enemies; //　敵を管理するリスト

    Player player;
    UIManager uIManager;

    bool playerTrun = true;
    bool enemyTrun = false;
    int enemycount = 0;

    // Plyaerステータス
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


    // Start関数よりも先に呼びされる
    private void Awake()
    {
        // GameManegerの重複をチェック
        if (instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);　// このオブジェクトは破壊されないようになる

        enemies = new List<Enemy>(); // リストのインスタンス化

        Application.targetFrameRate = 30; // FPS30に固定

        boardManager = GetComponent<BoardManager>();

        // Map生成
        Initgame();
    }

    // ゲーム開始でRuntimeによりCallが呼び出されて、OnSceneLodedが登録される。
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] // ゲーム開始時にかならず１度だけ呼び出される
    static public void Call()
    {
        SceneManager.sceneLoaded += OnSceneLoded;
    }

    // プレイヤーがGoalについたときに呼び出されて、マップを自動生成する
    static private void OnSceneLoded(Scene next,LoadSceneMode a)
    {
        instance.depth++;
        instance.Initgame();

    }

    // ゲーム準備用の関数
    public void Initgame()
    {

        playerTrun = true;
        enemyTrun = false;

        depthImage = GameObject.Find("DepthImage");
        depthText = GameObject.Find("DepthText").GetComponent<Text>();
        depthText.text = "Depth : " + depth;

        depthImage.SetActive(true);

        Invoke("HideDepthImage", 2f); // 2秒後にキャンバスを消す

        enemies.Clear(); // 初期化
        player = GameObject.Find("Player").GetComponent<Player>();
        uIManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        boardManager.SetupSecene(depth);
        uIManager.PlayerTurnTextUpdate();
    }

    // ステージのイメージ画像を消す
    public void HideDepthImage()
    {
        depthImage.SetActive(false);
    }

    void Update()
    {

        // プレイヤーがAP0になったらターンを終了させる
        if(player.isActionturn && player.ap < 1)
        {
            player.isActionturn = false;
            player.phasecount++;
            playerTrun = false;
            enemyTrun = true;
            enemycount = 0;
            uIManager.EnemyTurnTextUpdate();
        }

        // 敵のターン処理
        if (enemyTrun)
        {
            if (player.isDown) // PlayerがDownしたのでEnemyターンを終了
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

    // 管理リストに追加する関数
    public void AddEnemy(Enemy script)
    {
        enemies.Add(script); // Enemyのスクリプトをリストに格納する
    }

    // ターン切り替え
    private void TurnChenge()
    {
        enemyTrun = false;
        playerTrun = true;
        player.phasecount = 0;
        player.isCardDraw = true;
        uIManager.PlayerTurnTextUpdate();
    }

    // コルーチン
    private IEnumerable Waiting(float num)
    {
        yield return new WaitForSeconds(num);
    }

    // Playerが倒れた時の処理
    private void PlayerDown()
    {

    }

}
