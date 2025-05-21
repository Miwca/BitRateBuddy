using BitRateBuddy.Service;
using BitRateBuddy.Service.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.RegisterDependencies(builder.Configuration);

var host = builder.Build();
host.Run();
