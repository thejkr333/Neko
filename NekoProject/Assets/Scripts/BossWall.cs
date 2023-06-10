////////////////////////////////////////////////////////////////////////////////////////////////////////
//Author : Ruben Vidorreta
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWall : MonoBehaviour
{
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        GameManager.Instance.OnStartBossFight += Close;
    }

    void Close()
    {
        anim.SetTrigger("Move");
    }

    public void Sound()
    {
        AudioManager.Instance.PlaySound("Thump");
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnStartBossFight -= Close;
    }
}
