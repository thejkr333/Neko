using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance;
    public static CameraManager Instance => instance;

    Animator anim;
    public enum CameraStates { PlayerCam, ShopCam}
    CameraStates currentCameraState;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        // Initialization logic
        anim = GetComponent<Animator>();
    }

    public void ChangeCamera(CameraStates cameraState)
    {
        currentCameraState = cameraState;
        anim.Play(currentCameraState.ToString());
    }
}
