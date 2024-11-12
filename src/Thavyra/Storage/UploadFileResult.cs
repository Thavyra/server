namespace Thavyra.Storage;

public abstract class UploadFileResult;

public class UploadFileSucceededResult : UploadFileResult;

public class UploadFileFailedResult : UploadFileResult;

public class InvalidFileFormatResult : UploadFileFailedResult;
