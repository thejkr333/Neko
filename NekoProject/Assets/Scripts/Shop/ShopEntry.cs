using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopEntry : Interactable
{
    [SerializeField] GameObject highLightObject;

    public override void Interact()
    {
        GoToShop();
    }

    public override void StartHighLight()
    {
        highLightObject.SetActive(true);
    }

    public override void StopHighLight()
    {
        highLightObject.SetActive(false);
    }

    void GoToShop()
    {
        CameraManager.Instance.ChangeCamera(CameraManager.CameraStates.ShopCam);
    }
}
