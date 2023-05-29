using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 10);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out PlayerController playerController))
        {
            int dir = playerController.transform.position.x < transform.position.x ? -1 : 1;
            playerController.GetComponent<HealthSystem>().GetHurt(1, new Vector2(dir, 1));
        }
    }
}
