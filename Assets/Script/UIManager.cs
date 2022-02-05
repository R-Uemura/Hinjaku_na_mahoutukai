using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public GameObject cardCanvas;
    public GameObject canvas;

    public Text lvText;
    public Text stminaText;
    public Text hpText;
    public Text maxhpText;
    public Text hprText;
    public Text strengthText;
    public Text magicText;
    public Text apText;

    public Text actionText;

    public Text playerTurnText;
    public Text enemyTurnText;
    public Text enemeyNameText;

    Player player;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        cardCanvas = GameObject.Find("CardCanvas");
        canvas = GameObject.Find("Canvas");
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

    public void ActionTextUpdate(int ap)
    {
        switch (ap)
        {
            case 1:
                actionText.text = "Beating";
                break;
            case 2:
                actionText.text = "Magic 1";
                break;
            case 3:
                actionText.text = "Charge";
                break;
            case 4:
                actionText.text = "Magic 2";
                break;
            case 5:
                actionText.text = "Magic 3";
                break;
            case 6:
                actionText.text = "Magic 4";
                break;
            default:
                actionText.text = "No Action";
                break;
        }
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
}
