using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    PlayerController playerController;
    List<Interactable> interactableList = new();
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void Interact()
    {
        if (interactableList.Count == 0) return;

        interactableList[0].Interact(transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerController.Antman) return;

        if(collision.TryGetComponent(out Interactable interactable))
        {
            interactable.StartHighLight();
            interactableList.Add(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Interactable interactable))
        {
            interactable.StopHighLight();
            interactableList.Remove(interactable);
        }
    }
}
