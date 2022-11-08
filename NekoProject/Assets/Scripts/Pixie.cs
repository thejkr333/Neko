using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixie : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float maxDistance, smoothTime;
    [SerializeField] AnimationCurve speed;

    Vector3 velocity;
    float distance = 20f;
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
        float evalValue;
        if (Vector3.Distance(tr.position, target.position) > distance) evalValue = 1;
        else evalValue = Vector3.Distance(tr.position, target.position) / distance;

        tr.position = Vector3.SmoothDamp(tr.position, target.position, ref velocity, smoothTime, speed.Evaluate(evalValue));

        if (Input.GetKeyDown(KeyCode.Q) && Vector3.Distance(tr.position, target.position) <= 0.5f)
        {
            //anim de plantarse

            states = States.Checkpoint;
        }
    }

    void Checkpoint()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Vector3.Distance(tr.position, target.position) > maxDistance)
        {
            //anim de salir de checkpoint

            states = States.Following;
        }
    }

    void ChangeMinds()
    {

    }
}
