namespace RetroBoard.Api.Boards;

public interface IBoardStore
{
    Task<Board> CreateAsync(string name, CancellationToken cancellationToken);

    Task<Board?> GetAsync(Guid id, CancellationToken cancellationToken);
}
