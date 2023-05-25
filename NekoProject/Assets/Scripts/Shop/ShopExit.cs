using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopExit : Interactable
{
    [SerializeField] GameObject highLightObject;
    [SerializeField] Transform tpPosition;

    public override void Interact(Transform player)
    {
        GoToWorld(player);
    }

    public override void StartHighLight()
    {
        highLightObject.SetActive(true);
    }

    public override void StopHighLight()
    {
        highLightObject.SetActive(false);
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
