namespace Thavyra.Contracts;

public record Value<T> where T : struct
{
    public required T Item { get; set; }
}