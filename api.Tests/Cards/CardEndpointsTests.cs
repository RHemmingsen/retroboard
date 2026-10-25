using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using RetroBoard.Api.Boards;
using RetroBoard.Api.Cards;
using Xunit;

namespace RetroBoard.Api.Tests.Cards;

public sealed class CardEndpointsTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Create_ReturnsCreatedCardWithTrimmedText()
    {
        var board = await CreateBoardAsync();

        var response = await _client.PostAsJsonAsync(
            $"/api/boards/{board.Id}/cards", new CreateCardRequest("  Went well  "), CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var card = await response.Content.ReadFromJsonAsync<Card>(CancellationToken);
        Assert.NotNull(card);
        Assert.NotEqual(Guid.Empty, card.Id);
        Assert.Equal(board.Id, card.BoardId);
        Assert.Equal("Went well", card.Text);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Create_WithMissingText_ReturnsBadRequest(string? text)
    {
        var board = await CreateBoardAsync();

        var response = await _client.PostAsJsonAsync(
            $"/api/boards/{board.Id}/cards", new CreateCardRequest(text), CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(CancellationToken);
        Assert.Equal("Invalid card text", problem?.Title);
    }

    [Fact]
    public async Task Create_WithTextLongerThan500Characters_ReturnsBadRequest()
    {
        var board = await CreateBoardAsync();

        var response = await _client.PostAsJsonAsync(
            $"/api/boards/{board.Id}/cards", new CreateCardRequest(new string('a', 501)), CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_OnUnknownBoard_ReturnsNotFound()
    {
        var response = await _client.PostAsJsonAsync(
            $"/api/boards/{Guid.NewGuid()}/cards", new CreateCardRequest("Hello"), CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task List_ReturnsOnlyThatBoardsCardsInCreationOrder()
    {
        var board = await CreateBoardAsync();
        var otherBoard = await CreateBoardAsync();
        var first = await CreateCardAsync(board.Id, "First");
        var second = await CreateCardAsync(board.Id, "Second");
        await CreateCardAsync(otherBoard.Id, "Elsewhere");

        var cards = await _client.GetFromJsonAsync<Card[]>($"/api/boards/{board.Id}/cards", CancellationToken);

        Assert.NotNull(cards);
        Assert.Equal([first, second], cards);
    }

    [Fact]
    public async Task List_ForBoardWithoutCards_ReturnsEmptyList()
    {
        var board = await CreateBoardAsync();

        var cards = await _client.GetFromJsonAsync<Card[]>($"/api/boards/{board.Id}/cards", CancellationToken);

        Assert.NotNull(cards);
        Assert.Empty(cards);
    }

    [Fact]
    public async Task List_ForUnknownBoard_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/boards/{Guid.NewGuid()}/cards", CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<Board> CreateBoardAsync()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/boards", new CreateBoardRequest("Retro"), CancellationToken);
        return (await response.Content.ReadFromJsonAsync<Board>(CancellationToken))!;
    }

    private async Task<Card> CreateCardAsync(Guid boardId, string text)
    {
        var response = await _client.PostAsJsonAsync(
            $"/api/boards/{boardId}/cards", new CreateCardRequest(text), CancellationToken);
        return (await response.Content.ReadFromJsonAsync<Card>(CancellationToken))!;
    }
}
