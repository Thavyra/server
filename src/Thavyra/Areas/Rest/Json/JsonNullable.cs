using System.Diagnostics.CodeAnalysis;

namespace Thavyra.Rest.Json;

/// <summary>
/// JSON property which should be written, even if set to null.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct JsonNullable<T>
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

    public T? Value => IsNull ? default : Value;

    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsNull => _isNull;

    public static JsonNullable<T> Of(T value) => new(value);
    public static JsonNullable<T> Null() => new(isNull: true);
}