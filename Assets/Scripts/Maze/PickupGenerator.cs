using UnityEngine;

public class PickupGenerator : BaseMonoBehaviour
{
    [SerializeField]
    private int pickupCount = 8;
    
    private Pool<Pickup> pickupPool = new Pool<Pickup>();

    private void Awake()
    {
        foreach (Cell cell in FindObjectsOfType<Cell>())
        {
            if (cell.Pickup)
            {
                pickupPool.Insert(cell.Pickup);
            }
        }
    }

    private void Start()
    {
        for (int i = 0; i < pickupCount; i++)
        {
            if (pickupPool.TryUseRandom(out Pickup pickup))
            {
                pickup.Show();
            }
        }
    }
}
