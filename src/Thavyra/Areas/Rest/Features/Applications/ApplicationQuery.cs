namespace Thavyra.Rest.Features.Applications;

public abstract class ApplicationQuery
{
    public static bool TryParse(string? value, out ApplicationQuery? result)
    {
        result = value switch
        {
            _ when Guid.TryParse(value, out var g) => new ApplicationIdQuery(g),
            "@me" => new SelfQuery(),
            _ => null
        };

        return result is not null;
    }
}

public class ApplicationIdQuery : ApplicationQuery
{
    public ApplicationIdQuery(Guid applicationId)
    {
        ApplicationId = applicationId;
    }

    public Guid ApplicationId { get; }
}

public class SelfQuery : ApplicationQuery;