using UnityEngine;

public class PlayerState
{
    public StateEntry<Vector3> WorldPosition = new StateEntry<Vector3>();
    public StateEntry<Vector2> ViewportPosition = new StateEntry<Vector2>();
}
