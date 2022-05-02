using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class MessageTextManager : MonoBehaviour
{
    public Text lvUpText;
    public Text messagText;

    bool lvUp = false;
    float closetime;
    float opentime;

    bool messageflag = false;

    void Start()
    {
        lvUpText.enabled = false;
        messagText.enabled = false;
    }

    void Update()
    {
        if (lvUp)
        {
            if(closetime < opentime)
            {
                lvUpText.enabled = false;
                lvUp = false;
            }
            else
            {
                opentime += Time.deltaTime;
            }
        }

        if (messageflag)
        {
            if (closetime < opentime)
            {
                messagText.enabled = false;
                messageflag = false;
            }
            else
            {
                opentime += Time.deltaTime;
                messagText.color -= new Color(0.0f, 0.0f, 0.000f, 0.01f);
            }
        }
    }

    public void OpenLevelUPText()
    {
        lvUp = true;
        lvUpText.enabled = true;
        closetime = 3.5f;
        opentime = 0.0f;
        messageflag = false;
        messagText.enabled = false;
    }

    public void OpenItemText(string itemName)
    {
        switch (itemName)
        {
            case "りんご":
                messagText.text = "ST+30\nHPR+2";
                break;
            case "魔法のりんご":
                messagText.text = "MAG+1\nAP+1";
                break;
            case "ばなな":
                messagText.text = "ST+50\nHPR+1";
                break;
            case "魔法のばなな":
                messagText.text = "STR+1\nAP+1";
                break;
            case "魔法の帽子":
                messagText.text = "Charge\nMHP+1";
                break;
            default:
                break;
        }

        messagText.color = new Color(0.0f, 1.0f, 0.006f, 0.75f);
        messagText.enabled = true;
        messageflag = true;
        closetime = 2.0f;
        opentime = 0.0f;
        lvUp = false;
        lvUpText.enabled = false;
    }

    public void OnRevival()
    {
        messagText.text = "<size=25>Revival</size>";
        messagText.color = Color.yellow;
        messagText.enabled = true;
        messageflag = true;
        closetime = 2.0f;
        opentime = 0.0f;
        lvUp = false;
        lvUpText.enabled = false;
    }
}
