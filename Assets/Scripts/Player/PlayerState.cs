using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    public StateEntry<bool> Grabbed = new StateEntry<bool>();
    
    public StateEntry<LinkedList<Vector3>> WorldPositionPath = new StateEntry<LinkedList<Vector3>>(new LinkedList<Vector3>());
    public StateEntry<Vector3> WorldPosition = new StateEntry<Vector3>();
    public StateEntry<Quaternion> WorldRotation = new StateEntry<Quaternion>();
}
