using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �J�[�h�f�[�^�Ƃ��̏���
public class CardModel
{
    public int ap;
    public int stamina;
    public int hpr;

    public CardModel(int cardID)
    {
        // �J�[�h��Resources�t�H���_����J�[�h�f�[�^���擾���Đ���
        CardEntity cardEntity = Resources.Load<CardEntity>("CardEntityList/Card"+ cardID);
        ap = cardEntity.ap;
        stamina = cardEntity.stamina;
        hpr = cardEntity.hpr;
    }
}
