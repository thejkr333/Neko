using UnityEngine;

public enum Items
{
    DoubleJump, WallSlide, Dash, Shield, Antman, PixieCheckPoint, PixieChangeMinds
}
public class Item : MonoBehaviour
{
    public Items ID;
    public Sprite Sprite;
}