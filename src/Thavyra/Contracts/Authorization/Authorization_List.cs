namespace Thavyra.Contracts.Authorization;

public record Authorization_List
{
    public int? Count { get; init; }
    public int? Offset { get; init; }
}