using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLogic : MonoBehaviour
{
    RoomManager roomManager;

    [SerializeField] Transform[] exits;

    private void Awake()
    {
        roomManager = GetComponentInParent<RoomManager>();
    }

    public void ExitTrigger(OnTriggerDelegation delegation)
    {
        RoomManager.Direction directionExited = delegation.Caller.GetComponent<RoomExitTrigger>().direction;

        roomManager.RoomExited();
    }
}
