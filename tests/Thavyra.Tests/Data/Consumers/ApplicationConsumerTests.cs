using Bogus;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using Testcontainers.PostgreSql;
using Thavyra.Contracts;
using Thavyra.Contracts.Application;
using Thavyra.Data.Consumers;
using Thavyra.Data.Contexts;

namespace Thavyra.Tests.Data.Consumers;

public class ApplicationConsumerTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder().Build();
    
    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.StopAsync();
    }
    
    [Fact]
    public async Task Application_CheckClientSecret__NotFound()
    {
        await using var provider = new ServiceCollection()
            .AddDbContext<ThavyraDbContext>(builder => builder.UseNpgsql(_postgresContainer.GetConnectionString()))
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<ApplicationConsumer>();
            })
            .BuildServiceProvider(true);

        using (var scope = provider.CreateScope())
        {
            var seedContext = scope.ServiceProvider.GetRequiredService<ThavyraDbContext>();
            await seedContext.Database.EnsureCreatedAsync();
            
            await seedContext.SaveChangesAsync();
        }
        
        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        var client = harness.GetRequestClient<Application_CheckClientSecret>();
        
        var faker = new Faker();
        
        var response = await client.GetResponse<Correct, Incorrect, NotFound>(new Application_CheckClientSecret
        {
            ApplicationId = faker.Random.Guid(),
            Secret = faker.Random.Utf16String()
        });

        (await harness.Sent.Any<NotFound>()).Should().BeTrue();
        (await harness.Consumed.Any<Application_CheckClientSecret>()).Should().BeTrue();

        var consumerHarness = harness.GetConsumerHarness<ApplicationConsumer>();

        (await consumerHarness.Consumed.Any<Application_CheckClientSecret>()).Should().BeTrue();
    }

    [Fact]
    public async Task Application_CheckClientSecret__Correct()
    {
        await using var provider = new ServiceCollection()
            .AddDbContext<ThavyraDbContext>(builder => builder.UseNpgsql(_postgresContainer.GetConnectionString()))
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<ApplicationConsumer>();
            })
            .BuildServiceProvider(true);

        var application = Bogus.Application(type: OpenIddictConstants.ApplicationTypes.Web)[0];
        
        using (var scope = provider.CreateScope())
        {
            var seedContext = scope.ServiceProvider.GetRequiredService<ThavyraDbContext>();
            await seedContext.Database.EnsureCreatedAsync();

            seedContext.Users.Add(application.Owner);
            seedContext.Applications.Add(application);
            
            await seedContext.SaveChangesAsync();
        }
        
        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        var client = harness.GetRequestClient<Application_CheckClientSecret>();
        
        var response = await client.GetResponse<Correct, Incorrect, NotFound>(new Application_CheckClientSecret
        {
            ApplicationId = application.Id,
            Secret = application.ClientSecret!
        });
        
        (await harness.Sent.Any<Correct>()).Should().BeTrue();
        (await harness.Consumed.Any<Application_CheckClientSecret>()).Should().BeTrue();

        var consumerHarness = harness.GetConsumerHarness<ApplicationConsumer>();

        (await consumerHarness.Consumed.Any<Application_CheckClientSecret>()).Should().BeTrue();
    }

    [Fact]
    public async Task Application_CheckClientSecret__Incorrect()
    {
        await using var provider = new ServiceCollection()
            .AddDbContext<ThavyraDbContext>(builder => builder.UseNpgsql(_postgresContainer.GetConnectionString()))
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<ApplicationConsumer>();
            })
            .BuildServiceProvider(true);

        var application = Bogus.Application(type: OpenIddictConstants.ApplicationTypes.Web)[0];

        using (var scope = provider.CreateScope())
        {
            var seedContext = scope.ServiceProvider.GetRequiredService<ThavyraDbContext>();
            await seedContext.Database.EnsureCreatedAsync();

            seedContext.Users.Add(application.Owner);
            seedContext.Applications.Add(application);
            
            await seedContext.SaveChangesAsync();
        }
        
        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        var client = harness.GetRequestClient<Application_CheckClientSecret>();
        
        var response = await client.GetResponse<Correct, Incorrect, NotFound>(new Application_CheckClientSecret
        {
            ApplicationId = application.Id,
            Secret = new Faker().Random.Utf16String()
        });
        
        (await harness.Sent.Any<Incorrect>()).Should().BeTrue();
        (await harness.Consumed.Any<Application_CheckClientSecret>()).Should().BeTrue();

        var consumerHarness = harness.GetConsumerHarness<ApplicationConsumer>();

        (await consumerHarness.Consumed.Any<Application_CheckClientSecret>()).Should().BeTrue();
    }
    
    [Fact]
    public async Task Application_Count__EmptyDatabase()
    {
        await using var provider = new ServiceCollection()
            .AddDbContext<ThavyraDbContext>(builder => builder.UseNpgsql(_postgresContainer.GetConnectionString()))
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<ApplicationConsumer>();
            })
            .BuildServiceProvider(true);
        
        using (var scope = provider.CreateScope())
        {
            var seedContext = scope.ServiceProvider.GetRequiredService<ThavyraDbContext>();
            await seedContext.Database.EnsureCreatedAsync();
            
            await seedContext.SaveChangesAsync();
        }
        
        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        var client = harness.GetRequestClient<Application_Count>();

        var response = await client.GetResponse<Count>(new Application_Count());

        response.Message.Value.Should().Be(0);
        
        (await harness.Sent.Any<Count>()).Should().BeTrue();
        (await harness.Consumed.Any<Application_Count>()).Should().BeTrue();
        
        var consumerHarness = harness.GetConsumerHarness<ApplicationConsumer>();

        (await consumerHarness.Consumed.Any<Application_Count>()).Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(10)]
    public async Task Application_Count__Multiple(int count)
    {
        await using var provider = new ServiceCollection()
            .AddDbContext<ThavyraDbContext>(builder => builder.UseNpgsql(_postgresContainer.GetConnectionString()))
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<ApplicationConsumer>();
            })
            .BuildServiceProvider(true);

        var applications = Bogus.Application(count);
        
        using (var scope = provider.CreateScope())
        {
            var seedContext = scope.ServiceProvider.GetRequiredService<ThavyraDbContext>();
            await seedContext.Database.EnsureCreatedAsync();
            
            seedContext.Users.AddRange(applications.Select(x => x.Owner));
            seedContext.Applications.AddRange(applications);
            
            await seedContext.SaveChangesAsync();
        }
        
        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        var client = harness.GetRequestClient<Application_Count>();

        var response = await client.GetResponse<Count>(new Application_Count());

        response.Message.Value.Should().Be(count);
        
        (await harness.Sent.Any<Count>()).Should().BeTrue();
        (await harness.Consumed.Any<Application_Count>()).Should().BeTrue();
        
        var consumerHarness = harness.GetConsumerHarness<ApplicationConsumer>();

        (await consumerHarness.Consumed.Any<Application_Count>()).Should().BeTrue();
    }

    [Fact]
    public async Task Application_Create__Web()
    {
        await using var provider = new ServiceCollection()
            .AddDbContext<ThavyraDbContext>(builder => builder.UseNpgsql(_postgresContainer.GetConnectionString()))
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<ApplicationConsumer>();
            })
            .BuildServiceProvider(true);

        var owner = Bogus.User()[0];
        
        using (var seedScope = provider.CreateScope())
        {
            var seedContext = seedScope.ServiceProvider.GetRequiredService<ThavyraDbContext>();
            await seedContext.Database.EnsureCreatedAsync();

            seedContext.Users.Add(owner);
            
            await seedContext.SaveChangesAsync();
        }
        
        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        var client = harness.GetRequestClient<Application_Create>();

        var faker = new Faker();

        var request = new Application_Create
        {
            OwnerId = owner.Id,
            Type = OpenIddictConstants.ApplicationTypes.Web,
            Name = faker.Company.CompanyName(),
            Description = faker.Lorem.Paragraph(),
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit
        };
        
        var response = await client.GetResponse<Application>(request);
        
        (await harness.Sent.Any<Application>()).Should().BeTrue();
        (await harness.Consumed.Any<Application_Create>()).Should().BeTrue();
        
        var consumerHarness = harness.GetConsumerHarness<ApplicationConsumer>();

        (await consumerHarness.Consumed.Any<Application_Create>()).Should().BeTrue();
        
        response.Message.OwnerId.Should().Be(request.OwnerId);
        response.Message.Type.Should().Be(request.Type);
        response.Message.Name.Should().Be(request.Name);
        response.Message.Description.Should().Be(request.Description);
        response.Message.ConsentType.Should().Be(request.ConsentType);
        response.Message.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

        response.Message.ClientId.Should().NotBeNull();
        response.Message.ClientType.Should().Be(OpenIddictConstants.ClientTypes.Confidential);

        using var scope = provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ThavyraDbContext>();
            
        var application = await dbContext.Applications.FindAsync(response.Message.Id);

        Assert.NotNull(application);

        application.ClientSecret.Should().NotBeNull();
    }

    [Fact]
    public async Task Application_Create__Native()
    {
        await using var provider = new ServiceCollection()
            .AddDbContext<ThavyraDbContext>(builder => builder.UseNpgsql(_postgresContainer.GetConnectionString()))
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<ApplicationConsumer>();
            })
            .BuildServiceProvider(true);

        var owner = Bogus.User()[0];
        
        using (var seedScope = provider.CreateScope())
        {
            var seedContext = seedScope.ServiceProvider.GetRequiredService<ThavyraDbContext>();
            await seedContext.Database.EnsureCreatedAsync();
            
            seedContext.Users.Add(owner);
            
            await seedContext.SaveChangesAsync();
        }
        
        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        var client = harness.GetRequestClient<Application_Create>();

        var faker = new Faker();

        var request = new Application_Create
        {
            OwnerId = owner.Id,
            Type = OpenIddictConstants.ApplicationTypes.Native,
            Name = faker.Company.CompanyName(),
            Description = faker.Lorem.Paragraph(),
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit
        };
        
        var response = await client.GetResponse<Application>(request);
        
        (await harness.Sent.Any<Application>()).Should().BeTrue();
        (await harness.Consumed.Any<Application_Create>()).Should().BeTrue();
        
        var consumerHarness = harness.GetConsumerHarness<ApplicationConsumer>();

        (await consumerHarness.Consumed.Any<Application_Create>()).Should().BeTrue();
        
        response.Message.OwnerId.Should().Be(request.OwnerId);
        response.Message.Type.Should().Be(request.Type);
        response.Message.Name.Should().Be(request.Name);
        response.Message.Description.Should().Be(request.Description);
        response.Message.ConsentType.Should().Be(request.ConsentType);
        response.Message.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

        response.Message.ClientId.Should().NotBeNull();
        response.Message.ClientType.Should().Be(OpenIddictConstants.ClientTypes.Public);

        using var scope = provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ThavyraDbContext>();
            
        var application = await dbContext.Applications.FindAsync(response.Message.Id);

        Assert.NotNull(application);

        application.ClientSecret.Should().BeNull();
    }

    [Fact]
    public async Task Application_Delete()
    {
        await using var provider = new ServiceCollection()
            .AddDbContext<ThavyraDbContext>(builder => builder.UseNpgsql(_postgresContainer.GetConnectionString()))
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<ApplicationConsumer>();
            })
            .BuildServiceProvider(true);
        
        var application = Bogus.Application()[0];
        
        using (var seedScope = provider.CreateScope())
        {
            var seedContext = seedScope.ServiceProvider.GetRequiredService<ThavyraDbContext>();
            await seedContext.Database.EnsureCreatedAsync();

            seedContext.Users.Add(application.Owner);
            seedContext.Applications.Add(application);
            
            await seedContext.SaveChangesAsync();
        }
        
        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        var client = harness.GetRequestClient<Application_Delete>();
        
        var response = await client.GetResponse<Success>(new Application_Delete
        {
            Id = application.Id
        });
        
        (await harness.Sent.Any<Success>()).Should().BeTrue();
        (await harness.Consumed.Any<Application_Delete>()).Should().BeTrue();
        
        var consumerHarness = harness.GetConsumerHarness<ApplicationConsumer>();

        (await consumerHarness.Consumed.Any<Application_Delete>()).Should().BeTrue();
        
        using var scope = provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ThavyraDbContext>();
        
        dbContext.Applications.Any(x => x.Id == application.Id).Should().BeFalse();
    }

    [Fact]
    public async Task Application_GetByClientId__NotFound()
    {
        await using var provider = new ServiceCollection()
            .AddDbContext<ThavyraDbContext>(builder => builder.UseNpgsql(_postgresContainer.GetConnectionString()))
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<ApplicationConsumer>();
            })
            .BuildServiceProvider(true);

        using (var scope = provider.CreateScope())
        {
            var seedContext = scope.ServiceProvider.GetRequiredService<ThavyraDbContext>();
            await seedContext.Database.EnsureCreatedAsync();
            
            await seedContext.SaveChangesAsync();
        }
        
        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        var client = harness.GetRequestClient<Application_GetByClientId>();
        
        var faker = new Faker();
        
        var response = await client.GetResponse<Correct, Incorrect, NotFound>(new Application_GetByClientId
        {
            ClientId = faker.Random.AlphaNumeric(32)
        });

        (await harness.Sent.Any<NotFound>()).Should().BeTrue();
        (await harness.Consumed.Any<Application_GetByClientId>()).Should().BeTrue();

        var consumerHarness = harness.GetConsumerHarness<ApplicationConsumer>();

        (await consumerHarness.Consumed.Any<Application_GetByClientId>()).Should().BeTrue();
    }
    
    [Fact]
    public async Task Application_GetByClientId__Found()
    {
        await using var provider = new ServiceCollection()
            .AddDbContext<ThavyraDbContext>(builder => builder.UseNpgsql(_postgresContainer.GetConnectionString()))
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<ApplicationConsumer>();
            })
            .BuildServiceProvider(true);

        var application = Bogus.Application()[0];
        
        using (var scope = provider.CreateScope())
        {
            var seedContext = scope.ServiceProvider.GetRequiredService<ThavyraDbContext>();
            await seedContext.Database.EnsureCreatedAsync();

            seedContext.Users.Add(application.Owner);
            seedContext.Applications.Add(application);
            
            await seedContext.SaveChangesAsync();
        }
        
        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        var client = harness.GetRequestClient<Application_GetByClientId>();
        
        var response = await client.GetResponse<Application, NotFound>(new Application_GetByClientId
        {
            ClientId = application.ClientId
        });

        (await harness.Sent.Any<Application>()).Should().BeTrue();
        (await harness.Consumed.Any<Application_GetByClientId>()).Should().BeTrue();

        var consumerHarness = harness.GetConsumerHarness<ApplicationConsumer>();

        (await consumerHarness.Consumed.Any<Application_GetByClientId>()).Should().BeTrue();

        response.Is(out Response<Application>? r).Should().BeTrue();

        Assert.NotNull(r);
        
        r.Message.OwnerId.Should().Be(application.OwnerId);
        r.Message.Type.Should().Be(application.Type);
        r.Message.Name.Should().Be(application.Name);
        r.Message.Description.Should().Be(application.Description);
        r.Message.ConsentType.Should().Be(application.ConsentType);
        r.Message.ClientId.Should().Be(application.ClientId);
        r.Message.ClientType.Should().Be(application.ClientType);

        r.Message.CreatedAt.Should().BeCloseTo(application.CreatedAt, TimeSpan.FromSeconds(1));
    }
}