namespace Thavyra.Contracts;

public readonly struct Change<T>
{
    private Change(T value)
    {
        Value = value;
        IsChanged = true;
    }
    
    public T Value { get; }
    public bool IsChanged { get; }

    public static implicit operator Change<T>(T value) => new(value);
}