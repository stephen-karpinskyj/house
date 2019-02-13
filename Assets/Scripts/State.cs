using UnityEngine;

public class State : BehaviourSingleton<State>
{
    public StateEntry<Vector2> PlayerPosition = new StateEntry<Vector2>(Vector2.up);
}
