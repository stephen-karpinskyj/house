using UnityEngine;
using DG.Tweening;

public class Torch : BaseMonoBehaviour
{
    [SerializeField]
    private float turnSpeed = 10f;

    [SerializeField]
    private Light torchLight;

    [SerializeField]
    private float tweenDuration = 0.1f;
    
    private Quaternion rotationOffset;
    private float activeIntensity;
    private StateSubscriber<bool> playerGrabbed;

    private void Awake()
    {
        rotationOffset = transform.localRotation;
        activeIntensity = torchLight.intensity;
    }

    private void Update()
    {
        Quaternion nextRotation = State.Instance.Player.WorldRotation.Value * rotationOffset;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, nextRotation, Time.unscaledDeltaTime * turnSpeed);
    }

    private void OnEnable()
    {
        if (!torchLight)
        {
            torchLight = GetComponentInChildren<Light>();
        }

        playerGrabbed.Subscribe(State.Instance.Player.Grabbed, HandlePlayerGrabbedChange);
    }

    private void OnDisable()
    {
        playerGrabbed.Unsubscribe();
    }

    private void HandlePlayerGrabbedChange(bool value)
    {
        float intensity = value ? activeIntensity : 0f;
        torchLight.DOIntensity(intensity, tweenDuration);
    }
}
