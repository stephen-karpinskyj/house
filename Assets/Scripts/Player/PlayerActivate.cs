using UnityEngine;

public class PlayerActivate : MonoBehaviour
{
    [SerializeField]
    private Material material = null;
    
    [SerializeField]
    private Color activatedEmissionColor = Color.white;

    [SerializeField]
    private float activatedScaleFactor = 1.2f;

    private Color deactivatedEmissionColor;
    private Vector3 deactivatedScale;
    private StateSubscriber<bool> playerDragging;

    private void Awake()
    {
        deactivatedEmissionColor = material.GetColor("_EmissionColor");
        deactivatedScale = transform.localScale;
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
        Color color = value ? activatedEmissionColor : deactivatedEmissionColor;
        material.SetColor("_EmissionColor", color);
        transform.localScale = value ? deactivatedScale * activatedScaleFactor : deactivatedScale;
    }
}
