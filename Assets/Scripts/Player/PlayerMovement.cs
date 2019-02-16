using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private StateSubscriber<Vector3> playerWorldPosition;

    private void Awake()
    {
        State.Instance.Player.WorldPosition.Value = transform.localPosition;
    }

    private void OnEnable()
    {
        playerWorldPosition.Subscribe(State.Instance.Player.WorldPosition, HandlePlayerWorldPositionChange);
    }

    private void OnDisable()
    {
        playerWorldPosition.Unsubscribe();
    }

    private void HandlePlayerWorldPositionChange(Vector3 value)
    {
        transform.localPosition = value;
    }
}
