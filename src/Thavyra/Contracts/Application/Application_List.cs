namespace Thavyra.Contracts.Application;

/// <summary>
/// Lists all applications within the specified count and offset.
/// </summary>
/// <returns><see cref="Multiple{T}"/> of <see cref="Application"/></returns>
public record Application_List
{
    public int? Count { get; set; }
    public int? Offset { get; set; }
}