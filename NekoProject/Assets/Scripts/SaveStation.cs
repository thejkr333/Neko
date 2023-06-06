using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveStation : Interactable
{
    public override void Interact(Transform player)
    {
        GameManager.Instance.SaveGame();
        StopHighLight();
    }
}
