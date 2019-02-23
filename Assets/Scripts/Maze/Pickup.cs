using UnityEngine;

public class Pickup : BaseMonoBehaviour
{
    [SerializeField]
    private GameObject view;

    [SerializeField]
    private float minPickupDistance = 0.2f;

    private StateSubscriber<Vector3> playerWorldPosition;

    public bool CanShow => gameObject.activeInHierarchy;
    
    public bool Showing => view.activeInHierarchy;

    private void Awake()
    {
        view.SetActive(false);
    }
    
    private void OnEnable()
    {
        playerWorldPosition.Subscribe(State.Instance.Player.WorldPosition, HandlePlayerWorldPositionChange);
    }

    private void OnDisable()
    {
        playerWorldPosition.Unsubscribe();
    }

    public void Show()
    {
        view.SetActive(true);
    }

    private void HandlePlayerWorldPositionChange(Vector3 value)
    {
        if (Showing)
        {
            float distance = Vector3.Distance(value, transform.position);

            if (distance <= minPickupDistance)
            {
                view.SetActive(false);
            }
        }
    }
}
