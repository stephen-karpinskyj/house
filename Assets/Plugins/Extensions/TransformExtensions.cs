using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static List<Transform> GetDirectDescendents(this Transform t)
    {
        var children = new List<Transform>();

        for (var i = 0; i < t.childCount; i++)
        {
            children.Add(t.GetChild(i));
        }

        return children;
    }
}
