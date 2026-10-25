namespace RetroBoard.Api.Cards;

public sealed record Card(Guid Id, Guid BoardId, string Text);
