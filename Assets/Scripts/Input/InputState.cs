using UnityEngine;

public class InputState
{
    public StateEntry<int> WorldPositionsCount = new StateEntry<int>();
    public StateEntry<Vector3[]> WorldPositions = new StateEntry<Vector3[]>(new Vector3[64]);
    
    /// <summary>
    /// The difference between input world position and player world position at the point of grabbing the player.
    /// </summary>
    public StateEntry<Vector3> WorldPositionPlayerOffset = new StateEntry<Vector3>();
}
