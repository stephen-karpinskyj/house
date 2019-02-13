using UnityEngine;

public struct StateSubscriber<T>
{
    private StateChangeDelegate<T> changeHandler;

    public StateEntry<T> Entry { get; private set; }

    public T Value
    {
        get => Entry != null ? Entry.Value : default;
        set
        {
            Debug.Assert(Entry != null);
            Entry.Value = value;
        }
    }

    public void Subscribe(StateEntry<T> entry, StateChangeDelegate<T> changeHandler)
    {
        Entry = entry;
        this.changeHandler = changeHandler;

        Debug.Assert(Entry != null);
        Debug.Assert(changeHandler != null);

        Entry.OnChange += changeHandler;
        changeHandler(Value);
    }
    
    public void Unsubscribe()
    {
        Debug.Assert(Entry != null);
        Debug.Assert(changeHandler != null);

        Entry.OnChange -= changeHandler;
    }
}
