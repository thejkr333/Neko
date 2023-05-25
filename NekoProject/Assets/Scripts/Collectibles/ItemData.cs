using UnityEngine;

public enum Items
{
    DoubleJump, WallSlide, Dash, Shield, Antman, PixieCheckPoint, PixieChangeMinds
}
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Items", order = 1)]
public class ItemData : ScriptableObject
{
    public Items ID;
    public Sprite Sprite;
}