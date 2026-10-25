namespace RetroBoard.Api.Boards;

public interface IBoardStore
{
    Task<Board> CreateAsync(string name, CancellationToken cancellationToken);
}
