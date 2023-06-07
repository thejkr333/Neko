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

    public void CreateRock()
    {
        boss.CreateRock();
    }

    public void StopBoss()
    {
        boss.StopBoss();
    }

    public void WaveAttack()
    {
        boss.WaveAttack();
    }

    public void ThudSound()
    {
        AudioManager.Instance.PlaySound("Thud", Random.Range(.5f, 1.5f));
    }
}
