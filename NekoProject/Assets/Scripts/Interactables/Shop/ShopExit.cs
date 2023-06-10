////////////////////////////////////////////////////////////////////////////////////////////////////////
//Author : Ruben Vidorreta
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopExit : Interactable
{
    [SerializeField] Transform tpPosition;

    [SerializeField] CameraManager cameraManager;

    public override void Interact(Transform player)
    {
        GoToWorld(player);
    }

    void GoToWorld(Transform player)
    {
        cameraManager.ChangeCamera(CameraManager.CameraStates.PlayerCam, true);
        StartCoroutine(TPPlayer(player, 1));
    }

    IEnumerator TPPlayer(Transform player, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        player.position = tpPosition.position;
    }
}
