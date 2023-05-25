using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    List<Interactable> interactableList = new();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (interactableList.Count == 0) return;

            interactableList[0].Interact(transform);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
