using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    CardView view; // 見かけに関することを操作
    CardModel model; // データに関することを操作

    Player player;

    private void Awake()
    {
        view = GetComponent<CardView>();
    }

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // カード生成時に呼ばれる関数
    public void InitCard(int cardID)
    {
        model = new CardModel(cardID);
        view.Show(model);
    }

    public void OnTap()
    {
        if (player.isCardSelect)
        {
            player.ap = model.ap;
            player.stamina -= model.stamina;
            player.hpr += model.hpr;

            if (player.stamina < 1)
            {
                player.stamina = 0;
            }
            else if (player.stamina > 100)
            {
                player.stamina = 100;
            }
            if (player.hpr < 1)
            {
                player.hpr = 0;
            }

            Destroy(this.gameObject);
            player.phasecount++;
            player.isCardSelect = false;
            player.isActionturn = true;

            player.SkillChange(player.ap);
        }
    }
}
