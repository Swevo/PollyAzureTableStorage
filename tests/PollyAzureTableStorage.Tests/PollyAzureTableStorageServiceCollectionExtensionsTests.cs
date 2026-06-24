public class PollyAzureTableStorageServiceCollectionExtensionsTests
{
    private static readonly TableClient _client =
        new("DefaultEndpointsProtocol=https;AccountName=fake;AccountKey=AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==;EndpointSuffix=core.windows.net",
            "TestTable");

    [Fact]
    public void AddPollyAzureTableStorage_RegistersResiliencePipeline()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_client);
        services.AddPollyAzureTableStorage(p => { });
        Assert.NotNull(services.BuildServiceProvider().GetRequiredService<ResiliencePipeline>());
    }

    [Fact]
    public void AddPollyAzureTableStorage_RegistersResilientTableClient()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_client);
        services.AddPollyAzureTableStorage(p => { });
        var resilient = services.BuildServiceProvider().GetRequiredService<ResilientTableClient>();
        Assert.NotNull(resilient);
        Assert.Same(_client, resilient.Inner);
    }

    [Fact]
    public void AddPollyAzureTableStorage_WithConnectionString_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddPollyAzureTableStorage(
            "DefaultEndpointsProtocol=https;AccountName=fake;AccountKey=AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==;EndpointSuffix=core.windows.net",
            "TestTable", p => { });
        Assert.NotNull(services.BuildServiceProvider().GetRequiredService<ResilientTableClient>());
    }

    [Fact]
    public void AddPollyAzureTableStorage_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_client);
        Assert.Same(services, services.AddPollyAzureTableStorage(p => { }));
    }
}
