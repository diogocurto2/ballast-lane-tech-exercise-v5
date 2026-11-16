using BallastLane.Application.Abstractions;
using BallastLane.Application.Tasks;
using BallastLane.Infrastructure.Persistence;
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
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Ballast Lane Task API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT bearer token issued by the User API.",
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
builder.Services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();
builder.Services.AddScoped<NpgsqlTaskRepository>();
builder.Services.AddScoped<ITaskReader>(provider => provider.GetRequiredService<NpgsqlTaskRepository>());
builder.Services.AddScoped<ListTasksUseCase>();
builder.Services.AddScoped<GetTaskUseCase>();
builder.Services.AddScoped<CreateTaskUseCase>();
builder.Services.AddScoped<UpdateTaskUseCase>();
builder.Services.AddScoped<DeleteTaskUseCase>();
builder.Services.AddScoped<TaskValidator>();
builder.Services.AddScoped<ITaskWriter>(provider => provider.GetRequiredService<NpgsqlTaskRepository>());

var app = builder.Build();

app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    context.Response.ContentType = "application/problem+json";
    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new ProblemDetails
    {
        Status = StatusCodes.Status500InternalServerError,
        Title = "An unexpected error occurred",
        Type = "about:blank",
        Instance = context.TraceIdentifier,
    }));
}));

if (app.Environment.IsDevelopment())
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
public sealed class TaskApiEntryPoint;





