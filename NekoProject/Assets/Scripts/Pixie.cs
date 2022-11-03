using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixie : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float maxDistance, maxSpeed, smoothTime;

    Vector3 velocity;
    Transform tr;


    public enum States { Following, Checkpoint, ChangeMinds}
    public States states;
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
        Vector3.SmoothDamp(tr.position, target.position, ref velocity, smoothTime, maxSpeed);
    }

    void Checkpoint()
    {

    }

    void ChangeMinds()
    {

    }
}
