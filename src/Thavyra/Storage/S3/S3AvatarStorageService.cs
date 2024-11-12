using System.Net;
using System.Net.Mime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Thavyra.Storage.Configuration;

namespace Thavyra.Storage.S3;

public class S3AvatarStorageService : IAvatarStorageService
{
    private readonly IAmazonS3 _s3;
    private readonly HttpClient _httpClient;
    private readonly StorageOptions _options;

    public S3AvatarStorageService(IAmazonS3 s3, HttpClient httpClient, IOptions<StorageOptions> options)
    {
        _s3 = s3;
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<UploadFileResult> UploadAvatarAsync(AvatarType type, Guid ownerId, string url, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return new UploadFileFailedResult();
        }
        
        var content = await response.Content.ReadAsStreamAsync(cancellationToken);

        return await UploadAvatarAsync(type, ownerId, content, cancellationToken);
    }

    public async Task<UploadFileResult> UploadAvatarAsync(AvatarType type, Guid ownerId, Stream fileStream, CancellationToken cancellationToken)
    {
        var prefix = type switch
        {
            AvatarType.User => _options.Avatars.Users.Prefix,
            AvatarType.Application => _options.Avatars.Applications.Prefix,
            _ => throw new ArgumentException("Invalid avatar type.")
        };

        using var stream = new MemoryStream();

        try
        {
            using var image = await Image.LoadAsync(fileStream, cancellationToken);
            
            image.Mutate(x => x.Resize(500, 500));
            await image.SaveAsPngAsync(stream, cancellationToken);
        }
        catch (NotSupportedException)
        {
            return new InvalidFileFormatResult();
        }
        catch (InvalidImageContentException)
        {
            return new InvalidFileFormatResult();
        }
        catch (UnknownImageFormatException)
        {
            return new InvalidFileFormatResult();
        }
        catch (ImageProcessingException)
        {
            return new UploadFileFailedResult();
        }
        
        var request = new PutObjectRequest
        {
            BucketName = _options.Avatars.Bucket,
            Key = $"{prefix}/{ownerId}",
            ContentType = "image/png",
            InputStream = stream
        };

        var response = await _s3.PutObjectAsync(request, cancellationToken);

        return response.HttpStatusCode switch
        {
            HttpStatusCode.OK => new UploadFileSucceededResult(),
            _ => new UploadFileFailedResult()
        };
    }

    public async Task<GetFileResult> GetAvatarAsync(AvatarType type, Guid ownerId, CancellationToken cancellationToken)
    {
        var prefix = type switch
        {
            AvatarType.User => _options.Avatars.Users.Prefix,
            AvatarType.Application => _options.Avatars.Applications.Prefix,
            _ => throw new ArgumentException("Invalid avatar type.")
        };
        
        var request = new GetObjectRequest
        {
            BucketName = _options.Avatars.Bucket,
            Key = $"{prefix}/{ownerId}"
        };

        try
        {
            var response = await _s3.GetObjectAsync(request, cancellationToken);
            
            return new GetFileSucceededResult(response.ResponseStream);
        }
        catch (AmazonS3Exception ex) when (ex.Message is "The specified key does not exist.")
        {
            return new FileNotFoundResult();
        }
    }

    public async Task<DeleteFileResult> DeleteAvatarAsync(AvatarType type, Guid ownerId, CancellationToken cancellationToken)
    {
        var prefix = type switch
        {
            AvatarType.User => _options.Avatars.Users.Prefix,
            AvatarType.Application => _options.Avatars.Applications.Prefix,
            _ => throw new ArgumentException("Invalid avatar type.")
        };

        var request = new DeleteObjectRequest
        {
            BucketName = _options.Avatars.Bucket,
            Key = $"{prefix}/{ownerId}"
        };
        
        var response = await _s3.DeleteObjectAsync(request, cancellationToken);

        return response.HttpStatusCode switch
        {
            HttpStatusCode.NoContent => new DeleteFileSucceededResult(),
            HttpStatusCode.NotFound => new DeleteFileNotFoundResult(),
            _ => new DeleteFileFailedResult()
        };
    }
}