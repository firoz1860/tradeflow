using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using TradeFlow.Api.Configuration;
using TradeFlow.Api.Hubs;
using TradeFlow.Api.Middleware;
using TradeFlow.Application.Abstractions;
using TradeFlow.Application.Services;
using TradeFlow.Infrastructure;
using TradeFlow.Infrastructure.Authentication;
using TradeFlow.Infrastructure.Persistence;
using TradeFlow.MatchingEngine.Services;
using TradeFlow.Risk.Services;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, services, configuration) => configuration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services).WriteTo.Console());

var connectionString = builder.Configuration.GetConnectionString("Postgres") ?? throw new InvalidOperationException("ConnectionStrings:Postgres is required.");
var redisConnection = builder.Configuration.GetConnectionString("Redis") ?? "redis:6379";
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();
var dashboardOrigins = DashboardCorsOrigins.Resolve(builder.Configuration["Cors:AllowedOrigins"]);
if (jwtSettings.SigningKey.Length < 32) throw new InvalidOperationException("Jwt:SigningKey must be at least 32 characters.");

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.AddDbContext<TradeFlowDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddStackExchangeRedisCache(options => options.Configuration = redisConnection);
builder.Services.AddTradeFlowInfrastructure();
builder.Services.AddSingleton<RiskEngineService>();
builder.Services.AddSingleton<OrderBookRegistry>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TradingService>();
builder.Services.AddScoped<ITradingNotifier, SignalRTradingNotifier>();
builder.Services.AddSignalR();
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddPolicy("dashboard", policy => policy.WithOrigins(dashboardOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Query["access_token"];
            if (!string.IsNullOrWhiteSpace(token) && context.HttpContext.Request.Path.StartsWithSegments("/hubs/trading")) context.Token = token;
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();
using (var scope = app.Services.CreateScope()) await DatabaseSeeder.SeedAsync(scope.ServiceProvider.GetRequiredService<TradeFlowDbContext>());

app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("dashboard");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<TradingHub>("/hubs/trading");
app.Run();

public partial class Program;
