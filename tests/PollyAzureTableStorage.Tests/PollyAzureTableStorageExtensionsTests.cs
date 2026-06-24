public class PollyAzureTableStorageExtensionsTests
{
    private static readonly TableClient _client =
        new("DefaultEndpointsProtocol=https;AccountName=fake;AccountKey=AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==;EndpointSuffix=core.windows.net",
            "TestTable");

    private static readonly ResiliencePipeline _pipeline = new ResiliencePipelineBuilder().Build();

    [Fact]
    public void WithPolly_Pipeline_ReturnsResilientTableClient()
    {
        var resilient = _client.WithPolly(_pipeline);
        Assert.NotNull(resilient);
        Assert.Same(_client, resilient.Inner);
    }

    [Fact]
    public void WithPolly_Configure_ReturnsResilientTableClient()
    {
        var resilient = _client.WithPolly(p => p.AddRetry(new RetryStrategyOptions
        {
            MaxRetryAttempts = 3, Delay = TimeSpan.Zero,
            ShouldHandle = TableStorageTransientErrors.IsTransient,
        }));
        Assert.NotNull(resilient);
        Assert.Same(_client, resilient.Inner);
    }
}
