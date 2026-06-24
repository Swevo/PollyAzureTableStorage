# PollyAzureTableStorage

[![NuGet](https://img.shields.io/nuget/v/PollyAzureTableStorage.svg)](https://www.nuget.org/packages/PollyAzureTableStorage/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/PollyAzureTableStorage.svg)](https://www.nuget.org/packages/PollyAzureTableStorage/)
[![CI](https://github.com/Swevo/PollyAzureTableStorage/actions/workflows/build.yml/badge.svg)](https://github.com/Swevo/PollyAzureTableStorage/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**Polly v8 resilience for Azure.Data.Tables** — add retry, timeout, and circuit-breaker to any Table Storage operation in two lines.

```csharp
var tableClient = new TableClient(connectionString, "orders");

var resilient = tableClient.WithPolly(pipeline => pipeline
    .AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(1),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        ShouldHandle = TableStorageTransientErrors.IsTransient,
    })
    .AddTimeout(TimeSpan.FromSeconds(10)));

await resilient.UpsertEntityAsync(entity);
var response = await resilient.GetEntityAsync<OrderEntity>("partitionKey", "rowKey");
```

## Why PollyAzureTableStorage?

Azure Table Storage throttles aggressively at scale and returns 503 during maintenance windows. Without retry logic, a single throttle event causes data loss or visible errors. This library adds Polly v8 resilience with a predicate tuned for Table Storage:

| Problem | Solution |
|---------|----------|
| HTTP 429 throttling (storage account IOPS limit) | Caught by `TableStorageTransientErrors.IsTransient` |
| HTTP 503 service unavailable during maintenance | Caught by `TableStorageTransientErrors.IsTransient` |
| HTTP 504 gateway timeout | Caught by `TableStorageTransientErrors.IsTransient` |
| `HttpRequestException` network failure | Caught by `TableStorageTransientErrors.IsTransient` |
| `TaskCanceledException` timeout in transit | Caught by `TableStorageTransientErrors.IsTransient` |

## Installation

```
dotnet add package PollyAzureTableStorage
dotnet add package Polly.Core
```

## Quick-start

### 1. Manual wiring

```csharp
var client = new TableClient(connectionString, "orders");

var resilient = client.WithPolly(p => p
    .AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(1),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        ShouldHandle = TableStorageTransientErrors.IsTransient,
    }));

await resilient.UpsertEntityAsync(order);
var result = await resilient.GetEntityAsync<OrderEntity>("Orders", orderId);
```

### 2. Dependency injection

```csharp
builder.Services.AddPollyAzureTableStorage(connectionString, "orders", pipeline => pipeline
    .AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(1),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        ShouldHandle = TableStorageTransientErrors.IsTransient,
    })
    .AddTimeout(TimeSpan.FromSeconds(10)));

public class OrderRepository(ResilientTableClient client)
{
    public Task UpsertAsync(OrderEntity order, CancellationToken ct)
        => client.UpsertEntityAsync(order, cancellationToken: ct);

    public Task<Response<OrderEntity>> GetAsync(string pk, string rk, CancellationToken ct)
        => client.GetEntityAsync<OrderEntity>(pk, rk, cancellationToken: ct);
}
```

## API reference

| Member | Description |
|--------|-------------|
| `ResilientTableClient.Inner` | The underlying `TableClient` |
| `AddEntityAsync<T>(entity, ct)` | Adds an entity through the pipeline |
| `UpsertEntityAsync<T>(entity, mode, ct)` | Upserts an entity through the pipeline |
| `UpdateEntityAsync<T>(entity, ifMatch, mode, ct)` | Updates an entity through the pipeline |
| `DeleteEntityAsync(pk, rk, ifMatch, ct)` | Deletes an entity through the pipeline |
| `GetEntityAsync<T>(pk, rk, select?, ct)` | Gets a single entity through the pipeline |
| `ExecuteAsync<T>(operation, ct)` | Runs any `TableClient` operation through the pipeline |
| `TableStorageTransientErrors.IsTransient` | `PredicateBuilder` for 429/503/504, `HttpRequestException`, `TaskCanceledException` |
| `TableStorageTransientErrors.StatusCodes` | `IReadOnlySet<int>` — `{429, 503, 504}` |
| `client.WithPolly(pipeline)` | Wraps `TableClient` with a pre-built pipeline |
| `client.WithPolly(configure)` | Builds pipeline inline and wraps the client |
| `services.AddPollyAzureTableStorage(configure)` | DI registration (requires `TableClient` in DI) |
| `services.AddPollyAzureTableStorage(connStr, table, configure)` | DI with connection string shortcut |

## Target frameworks

.NET 6 ✅ · .NET 8 ✅ · .NET 9 ✅

## Related packages

| Package | Description |
|---------|-------------|
| [PollyAzureBlob](https://github.com/Swevo/PollyAzureBlob) | Polly v8 for Azure Blob Storage |
| [PollyAzureServiceBus](https://github.com/Swevo/PollyAzureServiceBus) | Polly v8 for Azure Service Bus |
| [PollyAzureKeyVault](https://github.com/Swevo/PollyAzureKeyVault) | Polly v8 for Azure Key Vault |
| [PollyAzureEventHub](https://github.com/Swevo/PollyAzureEventHub) | Polly v8 for Azure Event Hubs |
| [PollyCosmosDb](https://github.com/Swevo/PollyCosmosDb) | Polly v8 for Azure Cosmos DB |
| [PollyElasticsearch](https://github.com/Swevo/PollyElasticsearch) | Polly v8 for Elasticsearch |
| [PollyRedis](https://github.com/Swevo/PollyRedis) | Polly v8 for StackExchange.Redis |
| [PollyEFCore](https://github.com/Swevo/PollyEFCore) | Polly v8 for Entity Framework Core |
| [PollyDapper](https://github.com/Swevo/PollyDapper) | Polly v8 for Dapper |
| [PollyMongo](https://github.com/Swevo/PollyMongo) | Polly v8 for MongoDB |
| [PollyNpgsql](https://github.com/Swevo/PollyNpgsql) | Polly v8 for Npgsql (PostgreSQL) |
| [PollySqlClient](https://github.com/Swevo/PollySqlClient) | Polly v8 for Microsoft.Data.SqlClient |
| [PollyGrpc](https://github.com/Swevo/PollyGrpc) | Polly v8 for gRPC |
| [PollyRabbitMQ](https://github.com/Swevo/PollyRabbitMQ) | Polly v8 for RabbitMQ |
| [PollyKafka](https://github.com/Swevo/PollyKafka) | Polly v8 for Confluent.Kafka |
| [PollyOpenAI](https://github.com/Swevo/PollyOpenAI) | Polly v8 for OpenAI .NET SDK |
| [PollyMediatR](https://github.com/Swevo/PollyMediatR) | Polly v8 for MediatR |
| [PollyHealthChecks](https://github.com/Swevo/PollyHealthChecks) | Polly v8 for ASP.NET Core Health Checks |
| [PollySendGrid](https://github.com/Swevo/PollySendGrid) | Polly v8 for SendGrid |
| [PollyMassTransit](https://github.com/Swevo/PollyMassTransit) | Polly v8 for MassTransit |
| [PollyMailKit](https://github.com/Swevo/PollyMailKit) | MailKit SMTP email client |
| [PollyAzureQueueStorage](https://github.com/Swevo/PollyAzureQueueStorage) | Azure Queue Storage QueueClient |
| [PollyHangfire](https://github.com/Swevo/PollyHangfire) | Hangfire IBackgroundJobClient |
| [PollyBackoff](https://github.com/Swevo/PollyBackoff) | Polly v8 backoff helpers |

## 💼 Need .NET consulting?

The author of this package is available for consulting on **Polly v8 resilience**, **Azure cloud architecture**, and **clean .NET design**.

**[→ solidqualitysolutions.com](https://www.solidqualitysolutions.com/)**
## License

MIT © [Justin Bannister](https://github.com/Swevo)