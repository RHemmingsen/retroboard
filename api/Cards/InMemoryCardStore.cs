using System.Collections.Concurrent;

namespace RetroBoard.Api.Cards;

public sealed class InMemoryCardStore : ICardStore
{
    private readonly ConcurrentDictionary<Guid, ConcurrentQueue<Card>> _cardsByBoard = new();

    public Task<Card> CreateAsync(Guid boardId, string text, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var card = new Card(Guid.NewGuid(), boardId, text);
        _cardsByBoard.GetOrAdd(boardId, _ => new ConcurrentQueue<Card>()).Enqueue(card);
        return Task.FromResult(card);
    }

    public Task<IReadOnlyList<Card>> ListByBoardAsync(Guid boardId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<Card> cards = _cardsByBoard.TryGetValue(boardId, out var queue) ? queue.ToArray() : [];
        return Task.FromResult(cards);
    }
}
