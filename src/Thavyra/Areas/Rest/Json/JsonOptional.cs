namespace Thavyra.Rest.Json;

/// <summary>
/// Optional JSON property (excluded from ser/des if value is not set).
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct JsonOptional<T>
{
    private readonly T _value;
    private readonly bool _hasValue = false;

    private JsonOptional(T value)
    {
        _value = value;
        _hasValue = true;
    }

    public T Value
    {
        get
        {
            if (!HasValue)
            {
                throw new InvalidOperationException("Optional has not been set.");
            }
            
            return _value;
        }
    }
    
    public bool HasValue => _hasValue;

    public static implicit operator JsonOptional<T>(T value) => new(value);
}