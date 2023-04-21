using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;
using TrueBurn.DiscordBot.EskomSePush.Models.ESP;
using TrueBurn.DiscordBot.EskomSePush.Options;
using TrueBurn.DiscordBot.EskomSePush.Services;

namespace TrueBurn.DiscordBot.EskomSePush.Tests.ServicesTests;

[TestClass]
public class EskomSePushServiceTests
{

    private readonly EskomSePushService _sut;
    private readonly MockHttpMessageHandler _mockHttpMessageHandler = new();

    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();
    private readonly IMemoryCache _memoryCache;
    private readonly Mock<IOptions<EskomSePushOptions>> _eskomSePushOptionsMock = new();

    private readonly EskomSePushOptions _eskomSePushOptions = new();

    public EskomSePushServiceTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _eskomSePushOptionsMock.Setup(x => x.Value).Returns(_eskomSePushOptions);
        _sut = new EskomSePushService(_httpClientFactoryMock.Object, _eskomSePushOptionsMock.Object, _memoryCache);
    }

    [TestMethod]
    public async Task GetLoadSheddingStatus_ShouldReturnLoadSheddingStatus()
    {
        // Arrange
        Faker faker = new();

        Faker<LoadSheddingStatus> loadSheddingStatusFaker = new();

        LoadSheddingStatus loadSheddingStatus = loadSheddingStatusFaker.Generate();

        _mockHttpMessageHandler
            .When($"{_eskomSePushOptions.BaseUrl}/{_eskomSePushOptions.Endpoints.Status}")
            .Respond("application/json", JsonSerializer.Serialize(loadSheddingStatus));

        _httpClientFactoryMock
            .Setup(x => x
                .CreateClient(nameof(EskomSePushService))
            )
            .Returns(new HttpClient(_mockHttpMessageHandler)
            {
                BaseAddress = new Uri(_eskomSePushOptions.BaseUrl)
            });

        // Act
        LoadSheddingStatus result = await _sut.GetLoadSheddingStatus();

        // Assert
        result.Should().BeOfType(typeof(LoadSheddingStatus));
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(loadSheddingStatus);
    }

    [TestMethod]
    public async Task GetLoadSheddingStatus_ShouldReturnLoadSheddingStatusFromCache()
    {
        // Arrange
        Faker faker = new();

        Faker<LoadSheddingStatus> loadSheddingStatusFaker = new();
        LoadSheddingStatus loadSheddingStatus = loadSheddingStatusFaker.Generate();

        _mockHttpMessageHandler
            .When($"{_eskomSePushOptions.BaseUrl}/{_eskomSePushOptions.Endpoints.Status}")
            .Respond("application/json", JsonSerializer.Serialize(loadSheddingStatus));
        _httpClientFactoryMock
            .Setup(x => x
                           .CreateClient(nameof(EskomSePushService))
                       )
            .Returns(new HttpClient(_mockHttpMessageHandler)
            {
                BaseAddress = new Uri(_eskomSePushOptions.BaseUrl)
            });

        // Act
        LoadSheddingStatus result = await _sut.GetLoadSheddingStatus();
        LoadSheddingStatus resultCache = await _sut.GetLoadSheddingStatus();

        // Assert
        result.Should().BeOfType(typeof(LoadSheddingStatus));
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(loadSheddingStatus);

        // ensure that the cache is working
        resultCache.Should().BeOfType(typeof(LoadSheddingStatus));
        resultCache.Should().NotBeNull();
        resultCache.Should().BeEquivalentTo(loadSheddingStatus);

        // ensure that the http client is only called once
        _mockHttpMessageHandler.VerifyNoOutstandingExpectation();

        // ensure that the cache exists
        _memoryCache.TryGetValue("EskomSePushService.GetLoadSheddingStatus", out LoadSheddingStatus? _).Should().BeTrue();

    }

    [TestMethod]
    public async Task GetLoadSheddingStatus_ShouldThrowExceptionWhenHttpStatusCodeNotSuccessful()
    {
        // Arrange
        Faker faker = new();

        Faker<LoadSheddingStatus> loadSheddingStatusFaker = new();
        LoadSheddingStatus loadSheddingStatus = loadSheddingStatusFaker.Generate();

        _mockHttpMessageHandler
            .When($"{_eskomSePushOptions.BaseUrl}/{_eskomSePushOptions.Endpoints.Status}")
            .Respond(HttpStatusCode.BadRequest);
        _httpClientFactoryMock
            .Setup(x => x
                .CreateClient(nameof(EskomSePushService))
            )
            .Returns(new HttpClient(_mockHttpMessageHandler)
            {
                BaseAddress = new Uri(_eskomSePushOptions.BaseUrl)
            });

        // Act
        Func<Task> act = async () => await _sut.GetLoadSheddingStatus();

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [TestMethod]
    public async Task GetLoadSheddingStatus_ShouldThrowExceptionWhenCannotSerializeResponse()
    {
        // Arrange
        Faker faker = new();

        Faker<LoadSheddingStatus> loadSheddingStatusFaker = new();
        LoadSheddingStatus loadSheddingStatus = loadSheddingStatusFaker.Generate();

        _mockHttpMessageHandler
            .When($"{_eskomSePushOptions.BaseUrl}/{_eskomSePushOptions.Endpoints.Status}")
            .Respond("application/json", "invalid json");
        _httpClientFactoryMock
            .Setup(x => x
                .CreateClient(nameof(EskomSePushService))
            )
            .Returns(new HttpClient(_mockHttpMessageHandler)
            {
                BaseAddress = new Uri(_eskomSePushOptions.BaseUrl)
            });

        // Act
        Func<Task> act = async () => await _sut.GetLoadSheddingStatus();

        // Assert
        await act.Should().ThrowAsync<JsonException>();
    }
}


