namespace RetroBoard.Api.Cards;

public interface ICardStore
{
    Task<Card> CreateAsync(Guid boardId, string text, CancellationToken cancellationToken);

    Task<IReadOnlyList<Card>> ListByBoardAsync(Guid boardId, CancellationToken cancellationToken);
}
