using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using AutoEmpiric.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<Orchestrator>();

var app = builder.Build();

app.MapGet("/api/health", () => Results.Ok(new { Status = "Available", Timestamp = DateTime.UtcNow }));

app.Run();
[WARNING] --raw-output is enabled. Model output is not sanitized and may contain harmful ANSI sequences (e.g. for phishing or command injection). Use --accept-raw-output-risk to suppress this warning.