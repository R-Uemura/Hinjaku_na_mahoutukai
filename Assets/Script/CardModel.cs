using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// カードデータとその処理
public class CardModel
{
    public int ap;
    public int stamina;
    public int hpr;

    public CardModel(int cardID)
    {
        // カードをResourcesフォルダからカードデータを取得して生成
        CardEntity cardEntity = Resources.Load<CardEntity>("CardEntityList/Card"+ cardID);
        ap = cardEntity.ap;
        stamina = cardEntity.stamina;
        hpr = cardEntity.hpr;
    }
}
