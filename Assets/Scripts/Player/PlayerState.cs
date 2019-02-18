using UnityEngine;

public class PlayerState
{
    public StateEntry<Vector3> WorldPosition = new StateEntry<Vector3>();
    public StateEntry<Quaternion> WorldRotation = new StateEntry<Quaternion>();
    public StateEntry<bool> Dragging = new StateEntry<bool>();
}
