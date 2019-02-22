using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class Pool<T>
{
    private List<T> available;
    private List<T> used;

    public delegate T OnCreateCallback();
    public delegate void OnUnuseCallback(T obj);

    public IEnumerable<T> Available => available;
    public int AvailableCount => available.Count;

    public IEnumerable<T> Used => used;
    public int UsedCount => used.Count;

    public Pool()
    {
        available = new List<T>();
        used = new List<T>();
    }

    public Pool(IEnumerable<T> elements)
    {
        available = new List<T>(elements);
        used = new List<T>();
    }

    public T FindAvailable(Predicate<T> match)
    {
        return available.Find(match);
    }

    public List<T> FindAllAvailable(Predicate<T> match)
    {
        return available.FindAll(match);
    }

    public T FindUsed(Predicate<T> match)
    {
        return used.Find(match);
    }

    public List<T> FindAllUsed(Predicate<T> match)
    {
        return used.FindAll(match);
    }

    public void Insert(T obj, bool asAvailable = true)
    {
        Debug.Assert(!available.Contains(obj));
        Debug.Assert(!used.Contains(obj));

        if (asAvailable)
        {
            available.Add(obj);
        }
        else
        {
            used.Add(obj);
        }
    }

    public void Use(T obj)
    {
        Debug.Assert(!used.Contains(obj));

        available.Remove(obj);
        used.Add(obj);
    }

    public T Use(OnCreateCallback onCreate = null)
    {
        if (AvailableCount <= 0)
        {
            Debug.Assert(onCreate != null);

            available.Add(onCreate());
        }

        var toUse = available[0];

        Use(toUse);

        return toUse;
    }

    public bool TryUseRandom(out T obj)
    {
        obj = default;

        if (AvailableCount <= 0)
        {
            return false;
        }

        obj = available[Random.Range(0, AvailableCount)];

        Use(obj);

        return true;
    }

    public void Unuse(T obj)
    {
        Debug.Assert(used.Contains(obj));
        Debug.Assert(!available.Contains(obj));

        used.Remove(obj);
        available.Add(obj);
    }

    public void UnuseAll(OnUnuseCallback onUnuse = null)
    {
        foreach (var obj in used)
        {
            Debug.Assert(!available.Contains(obj));

            available.Add(obj);

            onUnuse?.Invoke(obj);
        }

        used.Clear();
    }
}