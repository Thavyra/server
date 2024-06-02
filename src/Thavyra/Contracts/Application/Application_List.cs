namespace Thavyra.Contracts.Application;

public record Application_List
{
    public int? Count { get; set; }
    public int? Offset { get; set; }
}