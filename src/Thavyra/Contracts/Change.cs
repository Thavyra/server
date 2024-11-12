namespace Thavyra.Contracts;

public readonly struct Change<T>
{
    private Change(T value)
    {
        Value = value;
        IsChanged = true;
    }
    
    public T Value { get; init; }
    public bool IsChanged { get; init; }

    public static implicit operator Change<T>(T value) => new(value);
}