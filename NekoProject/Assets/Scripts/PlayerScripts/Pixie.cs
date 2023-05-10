using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Pixie : MonoBehaviour
{
    [SerializeField] Transform followTarget;
    [SerializeField] float maxDistance, smoothTime, minDistanceForCheckPoint;
    [SerializeField] AnimationCurve speed;
    [SerializeField] LayerMask ground;
    Transform player;
    [SerializeField] PlayerController playerController;

    bool transitioning;

    Vector3 velocity, tpTarget;
    float distance = 20f, evalValue;
    Transform tr;

    float pressTime;
    float pressTolerance = .2f;
    public enum States { Following, Checkpoint, ChangeMinds}
    public States states;

    [SerializeField] float maxMoveDistance = .2f;

    [Header("CHANGE MINDS")]
    [SerializeField] float movSpeed;
    Rigidbody2D rb;
    CircleCollider2D circleCollider;

    [SerializeField] CinemachineVirtualCamera virtualCamera;

    Noise noise;

    bool pressingR;

    private void Awake()
    {
        player = playerController.transform;
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.enabled = false;
        transitioning = false;

        noise = GetComponent<Noise>();
        noise.enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        tr = transform;
        tr.position = followTarget.position;

        states = States.Following;
    }

    // Update is called once per frame
    void Update()
    {
        if (transitioning)
        {
            Transition();
            return;
        }

        switch (states)
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
                    
                }
                break;

            case States.Checkpoint:
                if (states == States.Following)
                {

                }
                if (states == States.ChangeMinds)
                {
                    rb.velocity = Vector2.zero;
                    playerController.enabled = true;
                    circleCollider.enabled = false;
                    virtualCamera.Follow = player;
                    pressingR = Input.GetKeyDown(KeyCode.R);
                }
                pressTime = 0;
                break;

            case States.ChangeMinds:
                if (states == States.Checkpoint)
                {
                    playerController.enabled = false;
                    circleCollider.enabled = true;
                    virtualCamera.Follow = tr;
                }
                break;
        }

        states = nextState;
    }

    void Follow()
    {
        float _distanceToTarget = Vector3.Distance(tr.position, followTarget.position);
      
        if (_distanceToTarget < .5f)
        {
            if (!noise.enabled)
            {
                noise.InitialPosition = tr.position;
                noise.enabled = true;
            }
        }
        else
        {
            if (noise.enabled && _distanceToTarget > 3f)
            {
                noise.enabled = false;
            }

            if(!noise.enabled)
            {
                MoveSmooth(followTarget.position);
            }
        }

        if (playerController.Dir == 1) tr.localScale = Vector3.one;
        else tr.localScale = new Vector3(-1, 1, 1);


        if (Input.GetKeyDown(KeyCode.R) && _distanceToTarget <= minDistanceForCheckPoint)
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, Vector2.down, 5f, ground);
            if (raycastHit2D)
            {
                //anim de plantarse
                tpTarget = raycastHit2D.point;

                pressingR = true;
                noise.enabled = false;
                transitioning = true;
                transform.parent = null;
            }
        }
    }

    void Transition()
    {
        Move(tpTarget);
        if (Vector3.Distance(tr.position, tpTarget) < .2f) 
        {
            ChangeStates(States.Checkpoint);
            transitioning = false;
        }
        if (Input.GetKeyUp(KeyCode.R)) pressingR = false;
    }

    void Checkpoint()
    {
        if (Vector3.Distance(tr.position, followTarget.position) > maxDistance)
        {
            //anim de salir de checkpoint

            ChangeStates(States.Following);
        }

        if (Input.GetKeyUp(KeyCode.R)) pressingR = false;

        if (pressingR) return;

        if (Input.GetKey(KeyCode.R))
        {
            pressTime += Time.deltaTime;

            if (pressTime > pressTolerance && Vector2.Distance(tr.position, player.position) < 4) ChangeStates(States.ChangeMinds);
        }
        else if (Input.GetKeyUp(KeyCode.R) && pressTime <= pressTolerance)
        {
            //anim de salir de checkpoint

            ChangeStates(States.Following);
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

        if (_x > 0) tr.localScale = Vector3.one;
        else if (_x < 0) tr.localScale = new Vector3(-1, 1, 1);

        Vector2 _moveDir = new Vector2(_x, _y).normalized;
        rb.velocity = _moveDir * movSpeed;

        if (Input.GetKeyUp(KeyCode.R)) pressingR = false;

        if (pressingR) return;

        if (Input.GetKeyDown(KeyCode.R)) transitioning = true;
    }


    void MoveSmooth(Vector3 target)
    {
        if (Vector3.Distance(tr.position, target) > distance) evalValue = 1;
        else evalValue = Vector3.Distance(tr.position, target) / distance;

        tr.position = Vector3.SmoothDamp(tr.position, target, ref velocity, smoothTime, speed.Evaluate(evalValue));
    }

    void Move(Vector3 target)
    {
        tr.position = Vector3.MoveTowards(tr.position, target, maxMoveDistance);
    }
}
