////////////////////////////////////////////////////////////////////////////////////////////////////////
//Author : Ruben Vidorreta
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLogic : MonoBehaviour
{
    RoomManager roomManager;

    //[SerializeField] Transform[] exits;

    [SerializeField] public Transform cameraConfiner;

    private void Awake()
    {
        roomManager = GetComponentInParent<RoomManager>();
    }

    public void ExitTrigger(OnTriggerDelegation delegation)
    {
        RoomManager.Direction directionExited = delegation.Caller.GetComponent<RoomExitTrigger>().direction;

        roomManager.RoomExited(this, directionExited);
    }
}
