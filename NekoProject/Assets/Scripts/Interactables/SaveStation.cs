using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveStation : Interactable
{
    public override void Interact(Transform player)
    {
        AudioManager.Instance.PlaySound("Save");
        GameManager.Instance.SaveGame();
        if(player.TryGetComponent(out HealthSystem healthSystem))
        {
            healthSystem.currentHealth = healthSystem.maxHealth;
        }
        StopHighLight();
    }
}
