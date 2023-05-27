using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Boosters { None, x2Damage, CoinAttract, ExtraHealth }
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Boosters", order = 1)]
public class BoosterData : ScriptableObject
{
    public Boosters ID;
    public Sprite Sprite;
}
