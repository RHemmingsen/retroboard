using RetroBoard.Api.Boards;
using RetroBoard.Api.Cards;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IBoardStore, InMemoryBoardStore>();
builder.Services.AddSingleton<ICardStore, InMemoryCardStore>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapBoardEndpoints();
app.MapCardEndpoints();

app.Run();
