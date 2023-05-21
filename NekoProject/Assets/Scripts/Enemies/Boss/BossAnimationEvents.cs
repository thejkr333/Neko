using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimationEvents : MonoBehaviour
{
    Boss boss;
    private void Awake()
    {
        boss = GetComponentInParent<Boss>();
    }

    public void EndAttack()
    {
        boss.EndAttack();
    }

    public void JumpAttackMove()
    {
        boss.JumpAttackMove();
    }

    public void StartHit()
    {
        boss.StartHit();
    }

    public void StopHit()
    {
        boss.StopHit();
    }


    public void StopBoss()
    {
        boss.StopBoss();
    }
}
