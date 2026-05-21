using System;
using AutoEmpiric.Core;
using AutoEmpiric.Core.Interfaces;
using AutoEmpiric.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IAgent, BasicAgent>();
builder.Services.AddScoped<ISandboxEngine, InProcessSandboxEngine>();
builder.Services.AddScoped<IValidator, EmpiricalValidator>();
builder.Services.AddScoped<Orchestrator>();

var app = builder.Build();

app.MapGet("/api/health", () => Results.Ok(new { Status = "Available", Timestamp = DateTime.UtcNow }));
app.MapControllers();

app.Run();
