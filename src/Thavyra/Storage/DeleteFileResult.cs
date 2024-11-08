namespace Thavyra.Storage;

public abstract class DeleteFileResult;

public class DeleteFileSucceededResult : DeleteFileResult;

public class DeleteFileNotFoundResult : DeleteFileResult;

public class DeleteFileFailedResult : DeleteFileResult;
