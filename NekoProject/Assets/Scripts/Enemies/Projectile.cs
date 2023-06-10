////////////////////////////////////////////////////////////////////////////////////////////////////////
//Author : Ruben Vidorreta
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    Rigidbody2D rb;

    [HideInInspector] public float speed;
    [HideInInspector] public Vector2 direction;

    bool hasRebounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.up = direction.normalized;
        rb.velocity = transform.up * speed;
    }

    void Rebound()
    {
        AudioManager.Instance.PlaySound("ShieldREbound");
        direction *= -1;
        hasRebounded = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Projectile projectile)) return;

        if(collision.TryGetComponent(out HealthSystem health))
        {
            if (hasRebounded || collision.TryGetComponent(out PlayerController playerController))
            {
                health.GetHurt(1, -direction);
            }
            else return;
        }
       
        if(collision.TryGetComponent(out Shield shield))
        {
            Rebound();           
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
