using LibrAI.Api.NetServices;

// read configuration, prepare the application container
var builder = WebApplication.CreateBuilder(args);

// assemble the web service
var app = builder.Build();

// configure the HTTP request pipeline
app.MapGet("/", () => "Hello World!");
app.MapGet("/healthz", () => HealthzService.GetHealthz());

app.Run();
