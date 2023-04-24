using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixie : MonoBehaviour
{
    [SerializeField] Transform followTarget;
    [SerializeField] float maxDistance, smoothTime, minDistanceForCheckPoint;
    [SerializeField] AnimationCurve speed;
    [SerializeField] LayerMask ground;
    Transform player;
    PlayerController playerController;

    Vector3 velocity, tpTarget;
    float distance = 20f, evalValue;
    Transform tr;

    float pressTime;
    float pressTolerance = .2f;
    public enum States { Following, Checkpoint, ChangeMinds}
    public States states;

    [Header("CHANGE MINDS")]
    [SerializeField] float movSpeed;
    Rigidbody2D rb;
    CircleCollider2D circleCollider;

    private void Awake()
    {
        player = transform.parent;
        playerController = player.GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        tr = transform;

        states = States.Following;
    }

    // Update is called once per frame
    void Update()
    {
        switch(states)
        {
            case States.Following:
                Follow();
                break;
            case States.Checkpoint:
                Checkpoint();
                break;
            case States.ChangeMinds:
                ChangeMinds();
                break;
        }
    }

    public void ChangeStates(States nextState)
    {
        switch (nextState)
        {
            case States.Following:
                if(states == States.Checkpoint)
                {
                    transform.parent = player;
                }
                break;

            case States.Checkpoint:
                if (states == States.Following)
                {
                    transform.parent = null;
                }
                if (states == States.ChangeMinds)
                {
                    playerController.enabled = true;
                    circleCollider.enabled = false;
                }
                pressTime = 0;
                break;

            case States.ChangeMinds:
                if (states == States.Checkpoint)
                {
                    playerController.enabled = false;
                    circleCollider.enabled = true;
                }
                break;
        }

        states = nextState;
    }

    void Follow()
    {
        if(Vector3.Distance(tr.position, followTarget.position) > .5f) Move(followTarget.position);

        if (Input.GetKeyDown(KeyCode.R) && Vector3.Distance(tr.position, followTarget.position) <= minDistanceForCheckPoint)
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, ground);
            if (raycastHit2D)
            {
                //anim de plantarse
                tpTarget = raycastHit2D.point;

                ChangeStates(States.Checkpoint);
            }
        }
    }

    void Checkpoint()
    {
        if(Vector3.Distance(tr.position, tpTarget) > .5f) Move(tpTarget);

        if (Input.GetKey(KeyCode.R))
        {
            pressTime += Time.deltaTime;

            if (pressTime > pressTolerance) ChangeStates(States.ChangeMinds);
        }
        else if (Input.GetKeyUp(KeyCode.R) && pressTime <= pressTolerance)
        {
            if (Vector3.Distance(tr.position, followTarget.position) > maxDistance)
            {
                //anim de salir de checkpoint

                ChangeStates(States.Following);
            }
        }
        else
        {
            pressTime = 0;
        }
    }

    void ChangeMinds()
    {
        float _x = Input.GetAxis("Horizontal");
        float _y = Input.GetAxis("Vertical");

        if (_x != 0) _y = 0;
        if (_y != 0) _x = 0;

        Vector2 _moveDir = new Vector2(_x, _y).normalized;
        rb.velocity = _moveDir * movSpeed;

        if (Input.GetKeyDown(KeyCode.R)) ChangeStates(States.Checkpoint);
    }


    void Move(Vector3 target)
    {
        if (Vector3.Distance(tr.position, target) > distance) evalValue = 1;
        else evalValue = Vector3.Distance(tr.position, target) / distance;

        tr.position = Vector3.SmoothDamp(tr.position, target, ref velocity, smoothTime, speed.Evaluate(evalValue));
    }
}
