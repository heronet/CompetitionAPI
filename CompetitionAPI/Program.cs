using CompetitionAPI.Data;
using CompetitionAPI.Extensions;
using CompetitionAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Added Services
builder.Services.AddScoped<TokenService>();
builder.Services.AddCors();
builder.Services.AddMetaServices(builder.Configuration);

var app = builder.Build();

// Migrate Database
using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
    {
        if (context!.Database.GetPendingMigrations().Any())
            context.Database.Migrate();
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors(policy =>
{
    policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Welcome to the Competition API!");
});

app.Run();
