using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;
using TransactionManagementSystem.Application.Command;
using TransactionManagementSystem.Application.Services;
using TransactionManagementSystem.Application.Services.Interfaces;
using TransactionManagementSystem.Infrastructure.Caching;
using TransactionManagementSystem.Infrastructure.Data;
using TransactionManagementSystem.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
var services = builder.Services;
var configuration = builder.Configuration;

// Database
services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

// Redis Cache
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration.GetConnectionString("Redis");
});

services.AddScoped<ICacheService, RedisCacheServices>();

// Authentication & Authorization
var jwtSettings = configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ManagerOrAdmin", policy => policy.RequireRole("Manager", "Admin"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
});

// Security Services
services.AddScoped<IPasswordHasher, PasswordHasher>();
services.AddScoped<IEncryptionService>(provider =>
    new AesEncryptionService(configuration["Security:EncryptionKey"]));
services.AddScoped<IJwtTokenService>(provider =>
    new JwtTokenService(
        secretKey,
        jwtSettings["Issuer"],
        jwtSettings["Audience"]));

// External Services
services.AddHttpClient<IPaymentsService, PaystackService>();

// Application Services
services.AddScoped<IAccountNumberGenerator, AccountNumberGenerator>();
services.AddScoped<ITransactionService, TransactionService>();
//services.AddScoped<INotificationService, NotificationService>();

// MediatR
services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.RegisterServicesFromAssemblyContaining<CreateAccountCommand>();
});

// Controllers
services.AddControllers();

// API Documentation
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Banking System API",
        Version = "v1",
        Description = "A comprehensive banking transaction system with CQRS pattern",
        Contact = new OpenApiContact
        {
            Name = "Banking System Team",
            Email = "kennethtibele@gmail.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments
    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //c.IncludeXmlComments(xmlPath);
});

// CORS
services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", builder =>
    {
        builder.WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new string[] { })
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// Health Checks
//services.AddHealthChecks()
//    .AddDbContextCheck<AppDbContext>()
//    .AddRedis(configuration.GetConnectionString("Redis"));

// Rate Limiting
//services.AddMemoryCache();
//services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Banking System API V1");
        
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");

// Custom Middleware
//app.UseMiddleware<RequestLoggingMiddleware>();
//app.UseMiddleware<ErrorHandlingMiddleware>();

//app.MapHealthChecks("/health");

// Database Migration and Seeding
//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//    if (app.Environment.IsDevelopment())
//    {
//        context.Database.EnsureCreated();
//        await SeedDatabaseAsync(context, scope.ServiceProvider);
//    }
//    else
//    {
//        context.Database.Migrate();
//    }
//}
// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
