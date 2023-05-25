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
        DontDestroyOnLoad(gameObject);
        anim = GetComponent<Animator>();
    }

    public void ChangeCamera(CameraStates cameraState)
    {
        currentCameraState = cameraState;
        StartCoroutine(Co_Transition());
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
