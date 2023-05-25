using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopEntry : Interactable
{
    [SerializeField] GameObject highLightObject;
    [SerializeField] Transform tpPosition;

    public override void Interact(Transform player)
    {
        GoToShop(player);
    }

    public override void StartHighLight()
    {
        highLightObject.SetActive(true);
    }

    public override void StopHighLight()
    {
        highLightObject.SetActive(false);
    }

    void GoToShop(Transform player)
    {
        CameraManager.Instance.ChangeCamera(CameraManager.CameraStates.ShopCam);
        player.position = tpPosition.position;
    }
}
