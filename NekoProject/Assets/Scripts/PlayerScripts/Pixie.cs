////////////////////////////////////////////////////////////////////////////////////////////////////////
//Author : Ruben Vidorreta
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using UnityEngine.InputSystem;

public class Pixie : MonoBehaviour, NekoInput.IPixieActions
{
    [SerializeField] Transform followTarget;
    [SerializeField] float maxDistance, smoothTime, minDistanceForCheckPoint;
    [SerializeField] AnimationCurve speed;
    [SerializeField] LayerMask ground;
    Transform player;
    [SerializeField] PlayerController playerController;
    PlayerStorage playerStorage;
    Animator anim;

    bool transitioning;

    Vector3 velocity, tpTarget;
    float distance = 20f, evalValue;
    Transform tr;

    float pressTime;
    float pressTolerance = .2f;
    public enum States { Following, Checkpoint, ChangeMinds}
    public States states;


    [Header("FOLLOW")]
    float distanceToTarget;

    [Header("CHANGE MINDS")]
    [SerializeField] float movSpeed;
    Rigidbody2D rb;
    CircleCollider2D circleCollider;

    [SerializeField] CinemachineVirtualCamera virtualCamera;

    Noise noise;

    private NekoInput controlsInput;
    bool canChangedStates;
    float changeStatesCD = .5f, changeStatesTimer;

    private void Awake()
    {
        controlsInput = new NekoInput();
        controlsInput.Pixie.SetCallbacks(this);

        player = playerController.transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
        playerStorage = player.GetComponent<PlayerStorage>();
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

        GameManager.Instance.EnablePixieInput += OnEnableInput;
        GameManager.Instance.DisablePixieInput += OnDisableInput;
    }

    private void OnEnable() => OnEnableInput();
    private void OnDisable() => OnDisableInput();
    void OnEnableInput() => controlsInput.Pixie.Enable();
    void OnDisableInput() => controlsInput.Pixie.Disable();
    public void OnPixie(InputAction.CallbackContext context)
    {
        if (transitioning || !canChangedStates || playerController.Antman) return;

        if (context.started)
        {
            switch (states)
            {
                case States.Following:
                    if (!playerStorage.ItemsUnlockedInfo[Items.PixieCheckPoint]) return;
                    distanceToTarget = Vector3.Distance(tr.position, followTarget.position);
                    if (distanceToTarget <= minDistanceForCheckPoint)
                    {
                        RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, Vector2.down, 5f, ground);
                        if (raycastHit2D)
                        {
                            anim.SetTrigger("Transform");
                            tpTarget = raycastHit2D.point;

                            noise.enabled = false;
                            transitioning = true;
                            transform.parent = null;
                        }
                    }
                    break;
                case States.Checkpoint:
                    if (!playerStorage.ItemsUnlockedInfo[Items.PixieChangeMinds]) return;
                    break;
                case States.ChangeMinds:
                    transitioning = true;
                    break;
            }
        }
        else if (context.performed)
        {
            if (states == States.Checkpoint)
            {
                if (!playerStorage.ItemsUnlockedInfo[Items.PixieChangeMinds]) return;
                ChangeStates(States.ChangeMinds);
            }
        }
        else if (context.canceled)
        {
            if (states == States.Checkpoint)
            {
                anim.SetTrigger("Transform");
                ChangeStates(States.Following);
            }
        }
    }

    private void Update()
    {
        distanceToTarget = Vector3.Distance(tr.position, followTarget.position);

        if (!canChangedStates)
        {
            changeStatesTimer += Time.deltaTime;
            if(changeStatesTimer > changeStatesCD) 
            { 
                changeStatesTimer = 0;
                canChangedStates = true;
            }
        }
    }

    private void ChangeMindsInput()
    {
        if (Input.GetKeyDown(KeyCode.R)) transitioning = true;
    }

    private void CheckpointInput()
    {
        if (playerStorage.ItemsUnlockedInfo[Items.PixieChangeMinds])
        {
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
                //pressingR = false;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                //anim de salir de checkpoint

                ChangeStates(States.Following);
            }
        }
    }

    private void FollowInput()
    {
        distanceToTarget = Vector3.Distance(tr.position, followTarget.position);

        if (!playerStorage.ItemsUnlockedInfo[Items.PixieCheckPoint]) return;

        if (Input.GetKeyDown(KeyCode.R) && distanceToTarget <= minDistanceForCheckPoint)
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, Vector2.down, 5f, ground);
            if (raycastHit2D)
            {
                //anim de plantarse
                tpTarget = raycastHit2D.point;

                noise.enabled = false;
                transitioning = true;
                transform.parent = null;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
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
        canChangedStates = false;

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
                    GameManager.Instance.EnablePlayerInputs();
                    circleCollider.enabled = false;
                    virtualCamera.Follow = player;
                    //pressingR = Input.GetKeyDown(KeyCode.R);
                }
                pressTime = 0;
                break;

            case States.ChangeMinds:
                if (states == States.Checkpoint)
                {
                    GameManager.Instance.DisablePlayerInputs();
                    circleCollider.enabled = true;
                    virtualCamera.Follow = tr;
                }
                break;
        }

        states = nextState;
    }

    void Follow()
    { 
        if (distanceToTarget < .5f)
        {
            if (!noise.enabled)
            {
                noise.InitialPosition = tr.position;
                noise.enabled = true;
            }
        }
        else
        {
            if (noise.enabled && distanceToTarget > 3f)
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
    }

    void Transition()
    {
        Move(tpTarget);
        if (Vector3.Distance(tr.position, tpTarget) < .2f) 
        {
            ChangeStates(States.Checkpoint);
            transitioning = false;
        }
        //if (Input.GetKeyUp(KeyCode.R)) pressingR = false;
    }

    void Checkpoint()
    {
        if (Vector3.Distance(tr.position, followTarget.position) > maxDistance)
        {
            //anim de salir de checkpoint

            ChangeStates(States.Following);
        }
    }

    void ChangeMinds()
    {
        //float _x = Input.GetAxisRaw("Horizontal");
        //float _y = Input.GetAxisRaw("Vertical");

        Vector2 _inputDir = controlsInput.Pixie.PixieMovement.ReadValue<Vector2>();

        if (_inputDir.x > 0) tr.localScale = Vector3.one;
        else if (_inputDir.x < 0) tr.localScale = new Vector3(-1, 1, 1);

        Vector2 _moveDir = _inputDir.normalized;
        rb.velocity = _moveDir * movSpeed;

        //if (Input.GetKeyUp(KeyCode.R)) pressingR = false;

        //if (pressingR) return;
    }

    void FourDirectionsMovement()
    {
        float _x = Input.GetAxis("Horizontal");
        float _y = Input.GetAxis("Vertical");

        if (_x != 0) _y = 0;
        if (_y != 0) _x = 0;

        if (_x > 0) tr.localScale = Vector3.one;
        else if (_x < 0) tr.localScale = new Vector3(-1, 1, 1);

        Vector2 _moveDir = new Vector2(_x, _y).normalized;
        rb.velocity = _moveDir * movSpeed;
    }


    void MoveSmooth(Vector3 target)
    {
        if (Vector3.Distance(tr.position, target) > distance) evalValue = 1;
        else evalValue = Vector3.Distance(tr.position, target) / distance;

        tr.position = Vector3.SmoothDamp(tr.position, target, ref velocity, smoothTime, speed.Evaluate(evalValue));
    }

    void Move(Vector3 target)
    {
        tr.position = Vector3.MoveTowards(tr.position, target, speed.Evaluate(1) * Time.deltaTime);
    }


    public void OnPixieMovement(InputAction.CallbackContext context)
    {
        
    }
}
