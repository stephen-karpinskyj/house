using UnityEngine;

public class Cell : BaseMonoBehaviour
{
    [SerializeField]
    private GameObject centreObject = null;

    [SerializeField]
    private Pickup pickup = null;

    public Pickup Pickup => !centreObject.activeInHierarchy && pickup.CanShow ? pickup : null;
}
