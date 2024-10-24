using MassTransit;
using Microsoft.EntityFrameworkCore;
using Thavyra.Data.Consumers;
using Thavyra.Data.Contexts;
using Thavyra.Data.Security.Hashing;

namespace Thavyra.Data.Configuration;

public static class Services
{
    public static IServiceCollection AddEntityFramework(this IServiceCollection services, IConfiguration configuration)
    {
        foreach (var section in configuration.GetChildren())
            switch (section.Key)
            {
                case "System":
                    services.Configure<SystemOptions>(section);
                    break;
                
                case "Postgres":
                    services.AddDbContext<ThavyraDbContext>(options =>
                    {
                        options.UseNpgsql(section["ConnectionString"]);
                    });

                    services.AddMemoryCache();
                    
                    break;
                
                case "Security":
                    foreach (var securitySection in section.GetChildren())
                        switch (securitySection.Key)
                        {
                            case "BCrypt":
                                services.Configure<BCryptOptions>(securitySection);
                                services.AddSingleton<IHashService, BCryptHashService>();
                                    
                                break;
                        }
                    
                    break;
            }
        
        
        
        return services;
    }

    public static IBusRegistrationConfigurator AddDataConsumers(this IBusRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<ApplicationConsumer>();
        configurator.AddConsumer<ApplicationCreatedConsumer>();
        configurator.AddConsumer<PermissionConsumer>();
        configurator.AddConsumer<AuthorizationConsumer>();
        configurator.AddConsumer<LoginConsumer>();
        configurator.AddConsumer<ScopeConsumer>();
        configurator.AddConsumer<TokenConsumer>();
        configurator.AddConsumer<UserConsumer>();
        configurator.AddConsumer<RoleConsumer>();
        configurator.AddConsumer<RegisterConsumer>();
        configurator.AddConsumer<TransactionConsumer>();
        configurator.AddConsumer<ScoreboardConsumer>();
        
        return configurator;
    }
}