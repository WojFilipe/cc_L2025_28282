using Cdv.PeopleApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<PeopleDb>(options=>{
    
    var cs = builder.Configuration.GetConnectionString("Db");
    options.UseSqlServer(cs);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.MapGet("/People", (PeopleDb db) =>
{
    var people = db.People.ToList();
    
    return people;
})
.WithName("GetPeople");

app.MapPost("/People", (PeopleDb db, PersonEntity person) =>
{
    db.People.Add(person);
    db.SaveChanges();
})
.WithName("AddPerson");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
