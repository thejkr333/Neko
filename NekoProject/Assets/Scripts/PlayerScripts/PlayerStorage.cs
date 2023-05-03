using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Items { }
public class PlayerStorage : MonoBehaviour
{
    public int Coins { get; private set; }
    public Dictionary<Items, bool> ItemsUnlockedInfo = new();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCoins(int amount = 1)
    {
        Coins += amount;
    }

    public void SubstractCoins(int amount = 1)
    {
        Coins -= amount;
    }
}
