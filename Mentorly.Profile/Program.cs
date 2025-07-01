using Carter;
using Mentorly.Profile.Extentions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureMongoDb().ConfigureMongoDbEntities();

builder.Services.AddCarter();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

await app.UseMongoDbEntitiesAsync();
app.MapCarter();
app.UseHttpsRedirection();



app.Run();

