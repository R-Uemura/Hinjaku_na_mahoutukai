using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CardEntity", menuName ="Create CardEntity")]
// カードデータそのもの
public class CardEntity : ScriptableObject
{
    public int ap;
    public int stamina;
    public int hpr;
}
