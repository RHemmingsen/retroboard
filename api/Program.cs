using RetroBoard.Api.Boards;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IBoardStore, InMemoryBoardStore>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapBoardEndpoints();

app.Run();
