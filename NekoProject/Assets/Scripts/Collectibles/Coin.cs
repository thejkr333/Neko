using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] float attractionForce;
    [SerializeField] float maxMoveDistance;

    PlayerStorage playerStorage;
    Rigidbody2D rb;
    Transform tr;
    private void Awake()
    {
        tr = transform.parent;    
        rb = GetComponentInParent<Rigidbody2D>();
    }

    private void Update()
    {
        if (playerStorage == null || !GameManager.Instance.EquippedBoosters[Boosters.CoinAttract]) return;

        rb.gravityScale = 0;
        if(Vector2.Distance(tr.position, playerStorage.transform.position) < .5f)
        {
            playerStorage.AddCoins();
            Destroy(tr.gameObject);
            return;
        }

        //Vector2 dir = playerStorage.transform.position - transform.position;
        //rb.AddForce(dir.normalized * attractionForce, ForceMode2D.Force);
        MoveSmooth(playerStorage.transform.position);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out PlayerStorage storage))
        {
            playerStorage = storage;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerStorage storage))
        {
            storage.AddCoins();
            Destroy(tr.gameObject);
        }
    }

    void MoveSmooth(Vector3 target)
    {
        tr.position = Vector3.MoveTowards(tr.position, target, maxMoveDistance);
    }
}
