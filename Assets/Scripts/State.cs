using UnityEngine;

public class State : BehaviourSingleton<State>
{
    public StateEntry<Vector3> PlayerPosition = new StateEntry<Vector3>();
}
