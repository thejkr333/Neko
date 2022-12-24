using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    Rigidbody2D rb;

    [HideInInspector] public float speed;
    [HideInInspector] public Vector2 direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.right = direction.normalized;
        rb.velocity = transform.right * speed;
    }

    public void Rebound()
    {
        direction *= -1;
    }
}
