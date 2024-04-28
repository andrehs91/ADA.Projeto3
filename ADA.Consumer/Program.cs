using ADA.Consumer;
using ADA.Consumer.Configurations;
using ADA.Consumer.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IAppSettings, AppSettings>();
builder.Services.AddScoped<IConsumerService, ConsumerService>();

var host = builder.Build();
host.Run();
