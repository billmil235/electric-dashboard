using ElectricDashboardApi;
using ElectricDashboardApi.Infrastructure;
using ElectricDashboardApi.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(opts => opts.SerializerOptions.IncludeFields = true);

builder.Services.AddDbContext<ElectricDashboardContext>(options =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("postgres"))
    );

builder.RegisterServices();

builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(30),
        LocalCacheExpiration = TimeSpan.FromMinutes(5)
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "allowedOrigins",
        policy => { policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader(); });
});

// Add authentication / authorization
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("user", policy => policy.RequireRole("user"));
    o.AddPolicy("admin", policy => policy.RequireRole("admin"));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.Audience = builder.Configuration["Keycloak:Audience"];
        o.MetadataAddress = builder.Configuration["Keycloak:MetadataAddress"]!;
        o.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = builder.Configuration["Keycloak:ValidIssuer"],
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuer = true
        };
    });

builder.Services.AddAuthorizationBuilder();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(builder.Configuration["Keycloak:AuthorizationUrl"]!),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "openid" },
                    { "profile", "profile" }
                }
            }
        },
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'"
    });

    var securityRequirement = new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference
                {
                    Id = "Keycloak",
                    Type = ReferenceType.SecurityScheme
                },
                In = ParameterLocation.Header,
                Name = "Bearer",
                Scheme = "Bearer"
            },
            []
        }
    };

    options.AddSecurityRequirement(securityRequirement);
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("allowedOrigins");
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGroup("/users").RegisterUserEndpoints();
app.MapGroup("/profile").RegisterProfileEndpoints();
app.MapGroup("/data").RegisterDataEndpoints();

app.Run();
