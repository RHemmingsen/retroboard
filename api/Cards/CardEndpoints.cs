using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RetroBoard.Api.Boards;

namespace RetroBoard.Api.Cards;

public static class CardEndpoints
{
    public static WebApplication MapCardEndpoints(this WebApplication app)
    {
        app.MapPost(
            "/api/boards/{boardId:guid}/cards",
            async Task<Results<Created<Card>, BadRequest<ProblemDetails>, NotFound>> (
                Guid boardId,
                CreateCardRequest request,
                IBoardStore boardStore,
                ICardStore cardStore,
                CancellationToken cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var text = request.Text?.Trim();
                if (string.IsNullOrEmpty(text) || text.Length > 500)
                {
                    var problem = new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Invalid card text",
                        Detail = "Text is required and must be between 1 and 500 characters after trimming."
                    };
                    return TypedResults.BadRequest(problem);
                }

                if (await boardStore.GetAsync(boardId, cancellationToken) is null)
                {
                    return TypedResults.NotFound();
                }

                var card = await cardStore.CreateAsync(boardId, text, cancellationToken);
                return TypedResults.Created($"/api/boards/{boardId}/cards", card);
            });

        app.MapGet(
            "/api/boards/{boardId:guid}/cards",
            async Task<Results<Ok<IReadOnlyList<Card>>, NotFound>> (
                Guid boardId,
                IBoardStore boardStore,
                ICardStore cardStore,
                CancellationToken cancellationToken) =>
            {
                if (await boardStore.GetAsync(boardId, cancellationToken) is null)
                {
                    return TypedResults.NotFound();
                }

                var cards = await cardStore.ListByBoardAsync(boardId, cancellationToken);
                return TypedResults.Ok(cards);
            });

        return app;
    }
}
