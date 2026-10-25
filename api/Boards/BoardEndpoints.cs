using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace RetroBoard.Api.Boards;

public static class BoardEndpoints
{
    public static WebApplication MapBoardEndpoints(this WebApplication app)
    {
        app.MapPost(
            "/api/boards",
            async Task<Results<Created<Board>, BadRequest<ProblemDetails>>> (
                CreateBoardRequest request,
                IBoardStore store,
                CancellationToken cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var name = request.Name?.Trim();
                if (string.IsNullOrEmpty(name) || name.Length > 80)
                {
                    var problem = new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Invalid board name",
                        Detail = "Name is required and must be between 1 and 80 characters after trimming."
                    };
                    return TypedResults.BadRequest(problem);
                }

                var board = await store.CreateAsync(name, cancellationToken);
                return TypedResults.Created($"/api/boards/{board.Id}", board);
            });

        app.MapGet(
            "/api/boards/{id:guid}",
            async Task<Results<Ok<Board>, NotFound>> (
                Guid id,
                IBoardStore store,
                CancellationToken cancellationToken) =>
            {
                var board = await store.GetAsync(id, cancellationToken);
                return board is null ? TypedResults.NotFound() : TypedResults.Ok(board);
            });

        return app;
    }
}
