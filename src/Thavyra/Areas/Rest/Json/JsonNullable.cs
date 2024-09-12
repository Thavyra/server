using System.Diagnostics.CodeAnalysis;

namespace Thavyra.Rest.Json;

/// <summary>
/// JSON property which should be written, even if set to null.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct JsonNullable<T> where T : notnull
{
    private readonly T? _value;
    private readonly bool _isNull = false;

    private JsonNullable(T value)
    {
        _value = value;
    }

    private JsonNullable(bool isNull)
    {
        _value = default;
        _isNull = true;
    }

    public bool IsNull => _isNull;

    public static implicit operator JsonNullable<T>(T? value) => value is null ? Null() : new(value);
    public static implicit operator T?(JsonNullable<T> value) => value._value;
    
    public static JsonNullable<T> Null() => new(isNull: true);
}