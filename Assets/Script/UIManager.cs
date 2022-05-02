using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    public GameObject cardCanvas;
    public GameObject canvas;
    public GameObject chargePanel;
    public GameObject gameOverCanvas;
    public GameObject revivalButton;

    public Text lvText;
    public Text stminaText;
    public Text hpText;
    public Text maxhpText;
    public Text hprText;
    public Text strengthText;
    public Text magicText;
    public Text apText;

    public Text actionText;
    public Text powerText;

    public Text playerTurnText;
    public Text enemyTurnText;
    public Text enemeyNameText;

    public Text reachDepthText;

    Player player;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        // cardCanvas = GameObject.Find("CardCanvas");
        // canvas = GameObject.Find("Canvas");
        // chargePanel = GameObject.Find("ChargePanel");
        chargePanel.SetActive(false);
        // gameOverCanvas = GameObject.Find("GameOverCanvas");
        gameOverCanvas.SetActive(false);
    }

    void Update()
    {
        if(player.isCardDraw)
        {
            if(player.phasecount == 0)
            {
                cardCanvas.SetActive(true);
                canvas.SetActive(false);
                player.phasecount++;
            }
            else if(player.phasecount == 2)
            {
                player.isCardDraw = false;
                player.isCardSelect = true;
                player.phasecount++;
            }
        }
        else if (player.phasecount == 4 && player.isActionturn)
        {
            cardCanvas.SetActive(false);
            canvas.SetActive(true);
            LevelTextUpdate();
            StaminaTextUpdate();
            HPTextUpdate();
            HPrTextUpdate();
            APTextUpdate();
            player.phasecount++;
        }
    }

    public void LevelTextUpdate()
    {
        lvText.text = "" + player.lv;
        maxhpText.text = "/" + player.maxHP;
        hprText.text = "" + player.hpr;
        strengthText.text = "" + player.strength;
        magicText.text = "" + player.magic;
    }

    public void StaminaTextUpdate()
    {
        stminaText.text = "" + player.stamina;
    }

    public void HPTextUpdate()
    {
        hpText.text = "" + player.hp;
    }

    public void HPrTextUpdate()
    {
        hprText.text = "" + player.hpr;
    }

    public void APTextUpdate()
    {
        apText.text = "" + player.ap;
    }

    public void GetItem()
    {
        StaminaTextUpdate();
        HPrTextUpdate();
        strengthText.text = "" + player.strength;
        magicText.text = "" + player.magic;
        maxhpText.text = "/" + player.maxHP;
    }

    public void ActionTextUpdate(int ap, int power, int dmgBonus)
    {
        switch (ap)
        {
            case 1:
                switch (dmgBonus)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        actionText.text = "<size=21>チャージスイング</size>";
                        break;
                    case 5:
                        actionText.text = "<size=21>ダッシュスイング</size>";
                        break;
                    case 6:
                        actionText.text = "<size=21>オーバーストライク</size>";
                        break;
                    default:
                        actionText.text = "杖で殴る";
                        break;
                }
                break;
            case 2:
                actionText.text = "マジックショット";
                break;
            case 3:
                actionText.text = "チャージ";
                break;
            case 4:
                actionText.text = "<size=21>ライトニングボール</size>";
                break;
            case 5:
                actionText.text = "<size=21>ファイアハリケーン</size>";
                break;
            case 6:
                actionText.text = "<size=21>ディメンジョンボム</size>";
                break;
            default:
                actionText.text = "No Action";
                break;
        }

        PowerTextUpdate(power);
    }

    public void PlayerTurnTextUpdate()
    {
        enemyTurnText.enabled = false;
        enemeyNameText.enabled = false;
        playerTurnText.enabled = true;
    }

    public void EnemyTurnTextUpdate()
    {
        enemyTurnText.enabled = true;
        playerTurnText.enabled = false;
    }

    public void EnemyNameTextUPdate(string name)
    {
        enemeyNameText.enabled = true;
        enemeyNameText.text = name;
    }

    public void ChargePanelChenge(bool charge)
    {
        chargePanel.SetActive(charge);
    }

    public void SetGameOverCanvas(bool gameover)
    {
        canvas.SetActive(!gameover);
        gameOverCanvas.SetActive(gameover);

        if (player.life)
        {
            revivalButton.SetActive(true);
        }
        else
        {
            revivalButton.SetActive(false);
        }
    }

    public void ReachDepthTextUpdate(int depth)
    {
        reachDepthText.text = "Depth:" + depth;
    }

    public void PowerTextUpdate(int power)
    {
        if(power > 1)
        {
            powerText.text = "威力：" + power;
        }
        else
        {
            powerText.text = "";
        }
    }

    public void OnClickTitleButton()
    {
        GameManager.instance.reStart = true;
        SceneManager.LoadScene("Title");
    }

    public void OnClickRevivalButton()
    {
        SetGameOverCanvas(false);
        player.PlayerRevival();
        Invoke("RevivalStart", 2f);
    }

    private void RevivalStart()
    {
        GameManager.instance.TurnChenge();
    }
}
