using System.Collections.Concurrent;

namespace RetroBoard.Api.Boards;

public sealed class InMemoryBoardStore : IBoardStore
{
    private readonly ConcurrentDictionary<Guid, Board> _boards = new();

    public Task<Board> CreateAsync(string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var board = new Board(Guid.NewGuid(), name, DateTimeOffset.UtcNow);
        _boards.TryAdd(board.Id, board);
        return Task.FromResult(board);
    }

    public Task<Board?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _boards.TryGetValue(id, out var board);
        return Task.FromResult(board);
    }
}
