using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomExitTrigger : OnTrigger2DDelegator
{
    [SerializeField] public RoomManager.Direction direction;
}