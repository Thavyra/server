namespace Thavyra.Storage;

public abstract class GetFileResult;

public class GetFileSucceededResult : GetFileResult
{
    public GetFileSucceededResult(Stream stream)
    {
        Stream = stream;
    }

    public Stream Stream { get; }
}

public class FileNotFoundResult : GetFileResult;
