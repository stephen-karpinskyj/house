public class State : BehaviourSingleton<State>
{
    public InputState Input = new InputState();
    public PlayerState Player = new PlayerState();
}
