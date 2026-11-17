using BallastLane.Application.Abstractions;
using BallastLane.Application.Users;
using BallastLane.Infrastructure.Persistence;
using BallastLane.Infrastructure.Security;
using BallastLane.Infrastructure.Time;
using BallastLane.SharedWeb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSharedAuthentication(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Ballast Lane User API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT bearer token issued by POST /api/users/login.",
    });
});

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
    {
        var origin = builder.Configuration["FrontendOrigin"];
        if (!string.IsNullOrWhiteSpace(origin))
        {
            policy.WithOrigins(origin).AllowAnyHeader().AllowAnyMethod();
        }
    }));

builder.Services.AddSingleton<ITimeProvider, SystemTimeProvider>();
builder.Services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();
builder.Services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();
builder.Services.AddSingleton<ITokenIssuer, JwtTokenIssuer>();
builder.Services.AddScoped<NpgsqlUserRepository>();
builder.Services.AddScoped<IUserReader>(provider => provider.GetRequiredService<NpgsqlUserRepository>());
builder.Services.AddScoped<IUserWriter>(provider => provider.GetRequiredService<NpgsqlUserRepository>());
builder.Services.AddScoped<UserValidator>();
builder.Services.AddScoped<RegisterUserUseCase>();
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<GetCurrentUserUseCase>();

var app = builder.Build();

app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    // Unhandled failures answer with a ProblemDetails carrying no stack trace, SQL, or driver text.
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    context.Response.ContentType = "application/problem+json";
    await context.Response.WriteAsJsonAsync(new ProblemDetails
    {
        Status = StatusCodes.Status500InternalServerError,
        Title = "An unexpected error occurred",
        Type = "about:blank",
        Instance = context.TraceIdentifier,
    });
}));

if (SwaggerSetup.IsEnabled(app.Environment.EnvironmentName, app.Configuration))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

/// <summary>Exposed so HTTP integration tests can host the application in-process.</summary>
public sealed class UserApiEntryPoint;

