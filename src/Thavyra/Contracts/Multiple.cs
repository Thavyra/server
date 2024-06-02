namespace Thavyra.Contracts;

public record Multiple<T>
{
    public required IReadOnlyList<T> Items { get; set; }
}