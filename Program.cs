var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<GreetingHandler>();

var app = builder.Build();

app.MapGet("/", (GreetingHandler handler, HttpContext context) => handler.Handle(context));

var urls = Environment.GetEnvironmentVariable("ASPNET_CORE_APPLICATION_URLS");
await app.RunAsync(urls);