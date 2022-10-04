using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    PlayerController parent;

    private void Awake()
    {
        parent = transform.GetComponentInParent<PlayerController>();
    }

    public void EndDash()
    {
        parent.EndDash();
    }

    public void EndAttack()
    {
        parent.EndAttack();
    }
}
