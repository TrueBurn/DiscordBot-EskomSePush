using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TrueBurn.DiscordBot.EskomSePush.Scaffolding;

namespace TrueBurn.DiscordBot.EskomSePush.Tests.ScaffoldingTests;

[TestClass]
public class ServiceCollectionSetupTests
{

    [TestMethod]
    public async Task ConfigureServices_ShouldReturnServiceProvider()
    {
        // Arrange
        await using ServiceProvider serviceProvider = ServiceCollectionSetup.ConfigureServices();
        // Act
        // Assert
        serviceProvider.Should().NotBeNull();
        serviceProvider.Should().BeOfType(typeof(ServiceProvider));
    }


}
