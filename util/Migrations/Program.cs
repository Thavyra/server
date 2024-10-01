using Migrations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EntityOptions>(builder.Configuration.GetSection("Entities"));
builder.Services.AddDbContext<MigrationDbContext>();

var app = builder.Build();

app.Run();