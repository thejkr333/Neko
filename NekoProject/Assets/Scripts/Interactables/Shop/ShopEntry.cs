using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopEntry : Interactable
{
    [SerializeField] Transform tpPosition;

    [SerializeField] CameraManager cameraManager;

    public override void Interact(Transform player)
    {
        GoToShop(player);
    }

    void GoToShop(Transform player)
    {
        cameraManager.ChangeCamera(CameraManager.CameraStates.ShopCam, true);
        StartCoroutine(TPPlayer(player, 1));
    }

    IEnumerator TPPlayer(Transform player, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        player.position = tpPosition.position;
    }
}
