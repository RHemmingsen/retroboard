namespace RetroBoard.Api.Boards;

public sealed record Board(Guid Id, string Name, DateTimeOffset CreatedUtc);
