using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopExit : Interactable
{
    [SerializeField] Transform tpPosition;

    public override void Interact(Transform player)
    {
        GoToWorld(player);
    }

    void GoToWorld(Transform player)
    {
        CameraManager.Instance.ChangeCamera(CameraManager.CameraStates.PlayerCam);
        StartCoroutine(TPPlayer(player, 1));
    }

    IEnumerator TPPlayer(Transform player, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        player.position = tpPosition.position;
    }
}
