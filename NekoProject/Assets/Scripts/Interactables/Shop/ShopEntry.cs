using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopEntry : Interactable
{
    [SerializeField] Transform tpPosition;

    public override void Interact(Transform player)
    {
        GoToShop(player);
    }

    void GoToShop(Transform player)
    {
        CameraManager.Instance.ChangeCamera(CameraManager.CameraStates.ShopCam);
        StartCoroutine(TPPlayer(player, 1));
    }

    IEnumerator TPPlayer(Transform player, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        player.position = tpPosition.position;
    }
}
