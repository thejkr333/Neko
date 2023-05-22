using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy")) return;

        if(collision.transform.TryGetComponent(out PlayerController playerController))
        {
            int dir = transform.position.x > playerController.transform.position.x ? 1 : -1;

            playerController.GetComponent<HealthSystem>().GetHurt(1, new Vector2(dir, 1));
        }

        Destroy(gameObject);
    }
}
