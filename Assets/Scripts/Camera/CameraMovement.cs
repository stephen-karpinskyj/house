using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private Vector2 shiftFactor = Vector2.one;
    
    private Camera cam;
    private StateSubscriber<Vector3> playerWorldPosition;

    private void Awake()
    {
        cam = GetComponent<Camera>();
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
        transform.localPosition = new Vector3(value.x, transform.localPosition.y, value.z);
        cam.lensShift = new Vector2(value.x * shiftFactor.x, value.z * shiftFactor.y);
    }
}
