using LibrAI.Api.Contracts;
using LibrAI.Api.NetServices;
using LibrAI.Api.Repositories;
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
app.MapPost("/titles", async (CreateTitleRequest request, ITitleRepository titleRepository) =>
{
    if (request == null)
    {
        return Results.BadRequest("request is null");
    }
    if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.ISBN) || string.IsNullOrWhiteSpace(request.Author))
    {
        return Results.BadRequest("Parameter is null or empty");
    }
    string id = Guid.NewGuid().ToString();
    var title = new Title(id, request.Name, request.ISBN, request.Author, request.Description, request.Publisher);
    if ((await titleRepository.TryAddAsync(title)))
    {
        return Results.Created($"/titles/{title.Id}", title);
    }
    return Results.Problem(statusCode: 500, detail: "Unable to create title.");
});

app.Run();
