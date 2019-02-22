using UnityEngine;
using UnityEngine.AI;

public static class NavMeshPathExtensions
{
    public static float CalculateDistance(this NavMeshPath path, Vector3[] corners, out int cornerCount)
    {
        float distance = 0f;
        
        cornerCount = path.GetCornersNonAlloc(corners);
        
        for (int i = 1; i < cornerCount; i++)
        {
            distance += Vector3.Distance(corners[i - 1], corners[i]);
        }

        return distance;
    }
}
