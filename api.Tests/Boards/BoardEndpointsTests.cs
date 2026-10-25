using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using RetroBoard.Api.Boards;
using Xunit;

namespace RetroBoard.Api.Tests.Boards;

public sealed class BoardEndpointsTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Create_ReturnsCreatedBoardWithTrimmedName()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/boards", new CreateBoardRequest("  Sprint 1  "), CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var board = await response.Content.ReadFromJsonAsync<Board>(CancellationToken);
        Assert.NotNull(board);
        Assert.Equal("Sprint 1", board.Name);
        Assert.Equal($"/api/boards/{board.Id}", response.Headers.Location?.OriginalString);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Create_WithMissingName_ReturnsBadRequest(string? name)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/boards", new CreateBoardRequest(name), CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(CancellationToken);
        Assert.Equal("Invalid board name", problem?.Title);
    }

    [Fact]
    public async Task Create_WithNameLongerThan80Characters_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/boards", new CreateBoardRequest(new string('a', 81)), CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsCreatedBoard()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/api/boards", new CreateBoardRequest("Retro"), CancellationToken);
        var created = await createResponse.Content.ReadFromJsonAsync<Board>(CancellationToken);

        var board = await _client.GetFromJsonAsync<Board>(createResponse.Headers.Location, CancellationToken);

        Assert.Equal(created, board);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("not-a-guid")]
    public async Task Get_WithUnknownId_ReturnsNotFound(string id)
    {
        var response = await _client.GetAsync($"/api/boards/{id}", CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
