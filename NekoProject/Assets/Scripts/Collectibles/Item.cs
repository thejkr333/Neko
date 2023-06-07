using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    public Items ID;

    public override void Interact(Transform player)
    {
        if(player.TryGetComponent(out PlayerStorage playerStorage))
        {
            AudioManager.Instance.PlaySound("Save", 2);
            playerStorage.UnlockItem(ID);
            Destroy(gameObject);
        }
    }
}
