using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float xMultiplier = 1f;

    [SerializeField, Range(0f, 1f)]
    private float zMultiplier = 1f;

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
        float x = value.x * xMultiplier;
        float z = value.z * zMultiplier;
        
        transform.localPosition = new Vector3(x, transform.localPosition.y, z);
        cam.lensShift = new Vector2(x * shiftFactor.x, z * shiftFactor.y);
    }
}
