using MassTransit;
using Microsoft.EntityFrameworkCore;
using Thavyra.Data.Consumers;
using Thavyra.Data.Contexts;

namespace Thavyra.Data.Configuration;

public static class Services
{
    public static IServiceCollection AddEntityFramework(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ThavyraDbContext>(options =>
        {
            foreach (var section in configuration.GetChildren())
                switch (section.Key)
                {
                    case "Postgres":
                        options.UseNpgsql(section["ConnectionString"]);
                        break;
                }
        });
        
        return services;
    }

    public static IBusRegistrationConfigurator AddDataConsumers(this IBusRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<ApplicationConsumer>();
        configurator.AddConsumer<AuthorizationConsumer>();
        configurator.AddConsumer<LoginConsumer>();
        configurator.AddConsumer<ScopeConsumer>();
        configurator.AddConsumer<TokenConsumer>();
        configurator.AddConsumer<UserConsumer>();
        configurator.AddConsumer<RegisterConsumer>();
        configurator.AddConsumer<TransactionConsumer>();
        configurator.AddConsumer<ScoreboardConsumer>();
        
        return configurator;
    }
}