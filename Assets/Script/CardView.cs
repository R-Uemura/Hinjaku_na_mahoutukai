using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �J�[�h�̌����ڂ𐧌䂷��N���X
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
