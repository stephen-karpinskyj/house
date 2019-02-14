using UnityEngine;

public class PlayerMovementView : MonoBehaviour
{
    private StateSubscriber<Vector3> playerPosition;

    private void Awake()
    {
        State.Instance.PlayerPosition.Value = transform.localPosition;
    }

    private void OnEnable()
    {
        playerPosition.Subscribe(State.Instance.PlayerPosition, HandlePlayerPositionChange);
    }

    private void OnDisable()
    {
        playerPosition.Unsubscribe();
    }

    private void HandlePlayerPositionChange(Vector3 value)
    {
        transform.localPosition = value;
    }
}
