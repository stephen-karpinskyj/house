using UnityEngine;

public class Torch : MonoBehaviour
{
    [SerializeField]
    private float turnSpeed = 10f;

    [SerializeField]
    private Light torchLight;
    
    private Quaternion rotationOffset;
    private StateSubscriber<bool> playerDragging;

    private void Awake()
    {
        if (!torchLight)
        {
            torchLight = GetComponentInChildren<Light>();
        }
        
        rotationOffset = transform.localRotation;
    }

    private void Update()
    {
        Quaternion nextRotation = State.Instance.Player.WorldRotation.Value * rotationOffset;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, nextRotation, Time.deltaTime * turnSpeed);
    }

    private void OnEnable()
    {
        playerDragging.Subscribe(State.Instance.Player.Dragging, HandlePlayerDraggingChange);
    }

    private void OnDisable()
    {
        playerDragging.Unsubscribe();
    }

    private void HandlePlayerDraggingChange(bool value)
    {
        torchLight.enabled = value;
    }
}
