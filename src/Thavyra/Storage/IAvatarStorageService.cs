namespace Thavyra.Storage;

public interface IAvatarStorageService
{
    Task<UploadFileResult> UploadAvatarAsync(AvatarType type, Guid ownerId, string url,
        CancellationToken cancellationToken);
    Task<UploadFileResult> UploadAvatarAsync(AvatarType type, Guid ownerId, Stream fileStream,
        CancellationToken cancellationToken);
    
    Task<GetFileResult> GetAvatarAsync(AvatarType type, Guid ownerId, CancellationToken cancellationToken);
    
    Task<DeleteFileResult> DeleteAvatarAsync(AvatarType type, Guid ownerId, CancellationToken cancellationToken);
}