using E_testament.Data;
using E_testament.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔌 Baza danych
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔐 JWT – autoryzacja + obsługa tokenu przez SignalR
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };

        // ➕ Umożliwia autoryzację przez URL (dla WebSocketów SignalR)
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
                    context.Token = accessToken;

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// 📦 Kontrolery, Swagger, SignalR
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// 🧠 Własne serwisy (jeśli używasz)
builder.Services.AddScoped<DatabaseUserService>();

var app = builder.Build();

// 🔁 Obsługa wyjątków w środowisku produkcyjnym
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error.html"); // lub inna strona błędu jeśli chcesz
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🧱 Pliki statyczne
app.UseDefaultFiles();   // Szuka index.html
app.UseStaticFiles();    // Obsługuje index.html, JS, CSS


// 📡 Endpointy
app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
