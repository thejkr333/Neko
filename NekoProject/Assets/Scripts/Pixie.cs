using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixie : MonoBehaviour
{
    [SerializeField] Transform followTarget;
    [SerializeField] float maxDistance, smoothTime, minDistanceForCheckPoint;
    [SerializeField] AnimationCurve speed;
    [SerializeField] LayerMask ground;

    Vector3 velocity, tpTarget;
    float distance = 20f, evalValue;
    Transform tr;


    public enum States { Following, Checkpoint, ChangeMinds}
    [HideInInspector] public States states;
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

    void Follow()
    {
        if(Vector3.Distance(tr.position, followTarget.position) > .5f) Move(followTarget.position);

        if (Input.GetKeyDown(KeyCode.Q) && Vector3.Distance(tr.position, followTarget.position) <= minDistanceForCheckPoint)
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, ground);
            if (raycastHit2D)
            {
                //anim de plantarse
                tpTarget = raycastHit2D.point;

                states = States.Checkpoint;
            }
        }
    }

    void Checkpoint()
    {
        if(Vector3.Distance(tr.position, tpTarget) > .5f) Move(tpTarget);

        if (Input.GetKeyDown(KeyCode.Q) || Vector3.Distance(tr.position, followTarget.position) > maxDistance)
        {
            //anim de salir de checkpoint

            states = States.Following;
            return;
        }
    }

    void ChangeMinds()
    {

    }

    void Move(Vector3 target)
    {
        if (Vector3.Distance(tr.position, target) > distance) evalValue = 1;
        else evalValue = Vector3.Distance(tr.position, target) / distance;

        tr.position = Vector3.SmoothDamp(tr.position, target, ref velocity, smoothTime, speed.Evaluate(evalValue));
    }
}
