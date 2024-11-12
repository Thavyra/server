using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Thavyra.Storage.S3;

namespace Thavyra.Storage.Configuration;

public static class Services
{
    public static IServiceCollection AddCloudStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<StorageOptions>(configuration);

        foreach (var section in configuration.GetChildren())
            switch (section.Key)
            {
                case "S3":
                    services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(
                        new BasicAWSCredentials(section["AccessKey"], section["SecretKey"]),
                        new AmazonS3Config { RegionEndpoint = RegionEndpoint.EUWest2 }));
                    services.AddSingleton<IAvatarStorageService, S3AvatarStorageService>();
                    break;
            }

        return services;
    }
}