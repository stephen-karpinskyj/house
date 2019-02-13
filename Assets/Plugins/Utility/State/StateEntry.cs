public class StateEntry<T>
{
    private T value;

    public T Value
    {
        get => value;
        
        set
        {
            this.value = value;
            ForceUpdate();
        }
    }
    
    public event StateChangeDelegate<T> OnChange = delegate { };
    
    public StateEntry(T value = default)
    {
        Value = value;
    }
    
    public void ForceUpdate()
    {
        OnChange(Value);
    }
}
