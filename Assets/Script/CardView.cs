using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// カードの見た目を制御するクラス
public class CardView : MonoBehaviour
{
    [SerializeField] Text apText;
    [SerializeField] Text staminaText;
    [SerializeField] Text hprText;

    public void Show(CardModel cardModel)
    {
        apText.text = cardModel.ap.ToString();
        staminaText.text = cardModel.stamina.ToString();
        hprText.text = cardModel.hpr.ToString();
    }
}
