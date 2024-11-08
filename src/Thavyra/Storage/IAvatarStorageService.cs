namespace Thavyra.Storage;

public interface IAvatarStorageService
{
    Task<UploadFileResult> UploadAvatarAsync(AvatarType type, Guid ownerId, IFormFile file, CancellationToken cancellationToken);
    
    Task<GetFileResult> GetAvatarAsync(AvatarType type, Guid ownerId, CancellationToken cancellationToken);
    
    Task<DeleteFileResult> DeleteAvatarAsync(AvatarType type, Guid ownerId, CancellationToken cancellationToken);
}