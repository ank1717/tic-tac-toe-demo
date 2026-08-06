using Microsoft.EntityFrameworkCore;
using TicTacToe.Api.Data;
using TicTacToe.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Core Web API Controllers Setup
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2. Register SQLite Database Context State Provider
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseSqlite("Data Source=tictactoe_master.db"));

// 3. Register Business Domain Engine Service
builder.Services.AddScoped<IGameRulesEngine, GameRulesEngine>();

// 4. CRUCIAL FIX: Register the SignalR Core Real-time Engine
builder.Services.AddSignalR(); // 👈 THIS LINE IS MISSING AND RESOLVES THE ERROR

// 5. Explicit CORS Cross-Origin Integration Rules Strategy
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Crucial configuration mapping requirement for SignalR handshakes
    });
});

var app = builder.Build();

// 6. Ensure Database Generation Schema Routine
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    context.Database.EnsureCreated();
}

// 7. Establish Request Handling Middleware Routing Pipelines
app.UseRouting();

// Apply CORS policy after routing and before endpoints
app.UseCors("LocalDev");
app.UseAuthorization();

// 8. Map controllers and require the same CORS policy for the SignalR hub
app.MapControllers();
app.MapHub<TicTacToe.Api.Hubs.GameHub>("/hub/game").RequireCors("LocalDev");

app.Run();
