using DisasterApi.Data;
using DisasterApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .SelectMany(e => e.Value!.Errors.Select(err => new
                {
                    field = e.Key,
                    message = err.ErrorMessage
                }))
                .ToList();

            return new BadRequestObjectResult(new
            {
                success = false,
                message = "Validation failed",
                errors
            });
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>{});
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<DisasterService>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "DisasterApi_";
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    try
    {
        if (!db.Database.CanConnect())
            Console.WriteLine("❌ Cannot connect to the database.");
        else
            Console.WriteLine("✅ Database connection successful.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database connection error: {ex.Message}");
    }

    var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
    var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
    Console.WriteLine($"Redis connection string: {redisConnectionString}");
    try
    {
        var testKey = "redis-connection-test";
        cache.SetString(testKey, "ok", new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10) });
        var value = cache.GetString(testKey);
        if (value == "ok")
            Console.WriteLine("✅ Redis connection successful.");
        else
            Console.WriteLine("❌ Cannot connect to Redis.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Redis connection error: {ex.Message}");
    }
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();

// API Status endpoint
app.MapGet("/", () => Results.Ok(new { message = "Hello disaster allocation API, For documentation visit /swagger" }))
    .WithName("RootEndpoint");

app.Run();