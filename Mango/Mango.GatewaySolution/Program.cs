using Mango.GatewaySolution.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

if(builder.Environment.EnvironmentName.ToString().ToLower() == "production")
    builder.Configuration.AddJsonFile("ocelot.Production.json", optional: true, reloadOnChange: true);
else
    builder.Configuration.AddJsonFile("ocelot.json", optional: true, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);
builder.AddAppAuthentication();

var app = builder.Build();
await app.UseOcelot();
app.Run();
