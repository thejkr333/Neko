using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Animator anim;
    public enum CameraStates { PlayerCam, ShopCam, BossCam}
    CameraStates currentCameraState;

    Cinemachine.CinemachineStateDrivenCamera CinemachineStateDrivenCamera;

    private void Awake()
    {
        CinemachineStateDrivenCamera = GetComponent<Cinemachine.CinemachineStateDrivenCamera>();
        // Initialization logic
        DontDestroyOnLoad(gameObject);
        anim = GetComponent<Animator>();
        GameManager.Instance.OnStartBossFight += ChangeCameraToBoss;
    }

    private void Start()
    {
        anim.Play(CameraStates.PlayerCam.ToString());
    }

    void ChangeCameraToBoss()
    {
        ChangeCamera(CameraStates.BossCam);
    }

    public void ChangeCamera(CameraStates cameraState, bool fade = false)
    {
        currentCameraState = cameraState;
        if(fade) StartCoroutine(Co_Transition());
        else
        {
            anim.Play(currentCameraState.ToString());
        }
    }

    IEnumerator Co_Transition()
    {
        anim.Play("FadeIn");

        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > .95f);

        anim.Play(currentCameraState.ToString());

        yield return null;

        anim.Play("FadeOut");
    }
}
