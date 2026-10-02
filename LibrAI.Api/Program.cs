using LibrAI.Api.NetServices;
using LibrAI.Domain.Catalog;

// read configuration, prepare the application container
var builder = WebApplication.CreateBuilder(args);

// register services in the application container
builder.Services.AddSingleton<ITitleRepository, InMemoryTitleRepository>();

// assemble the web service
var app = builder.Build();

// configure the HTTP request pipeline
app.MapGet("/", () => "Hello World!");
app.MapGet("/healthz", () => HealthzService.GetHealthz());
app.MapGet("/titles", async (ITitleRepository titleRepository) =>
{
    var titles = await titleRepository.ListAsync();
    return Results.Ok(titles);
});
app.MapGet("/titles/{id}", async (string id, ITitleRepository titleRepository) =>
{
    var title = await titleRepository.GetByIdAsync(id);
    if (title is null)
    {
        return Results.NotFound();
    }
    return Results.Ok(title);
});


app.Run();
