# TGHarker.Orleans.Indexing

TGHarker.Orleans.Indexing is a library for Orleans 7.0 that provides an easy way to index your grain states for searching. The library currently supports Azure Cognitive Search as an IndexProvider, while Elasticsearch support is being worked on and planned for future releases. Additional IndexProviders can be added in the future.

## Indexable Attributes

1. `IndexableStateAttribute`: This attribute goes on your grain's state. It tells the library that we should generate an index using the configured provider.
2. `IndexablePropertyAttribute`: This attribute goes on your grain states properties. It tells the library that this property should be included in the index. It has some customizable options: `IsId`, `IsFacetable`, `IsSearchable`, `IsHidden`, `IsSortable`. Every State object needs to have 1 and only 1 property declared as `IsId=true`.

## IndexProviders

1. Azure Cognitive Search: Azure Cognitive Search is a search-as-a-service cloud solution that provides a rich set of features for building sophisticated search applications. [Learn more about Azure Cognitive Search pricing and setup.](https://azure.microsoft.com/en-us/pricing/details/cognitive-services/search-services/)

## Getting Started

1. Install the NuGet package `TGHarker.Orleans.Indexing.AzureCognitiveSearch` on the project where your grain implementations are.
2. Install the NuGet package `TGHarker.Orleans.Indexing.AzureCognitiveSearch.CodeGenerator` on the project that contains your grain state objects.

### Adding Azure Cognitive Search Indexing to Your Orleans Silo

To add Azure Cognitive Search Indexing to your Orleans Silo, you'll need to use the `AddIndexing` extension method provided by the `ISiloBuilderExtensions` class.

First, create an `AzureSearchOptions` object with your Azure Cognitive Search configuration:

```csharp
using TGHarker.Orleans.Indexing.AzureCognitiveSearch;

var azureSearchOptions = new AzureSearchOptions
{
    Uri = new Uri("your-azure-search-uri"),
    ApiKey = "your-azure-search-api-key"
};
```

Next, in your Orleans Silo host builder configuration, call the AddIndexing extension method:

```csharp
using TGHarker.Orleans.Indexing;
using TGHarker.Orleans.Indexing.AzureCognitiveSearch;

var siloBuilder = new SiloHostBuilder()
    // ...
    .AddIndexing(builder => builder
        .UseAzureCognitiveSearch(options => {
            options.Uri = azureSearchOptions.Uri;
            options.ApiKey = azureSearchOptions.ApiKey;
        }))
    // ...
    .Build();
```

By calling this method, you'll add Azure Cognitive Search as an IndexProvider for your Orleans Silo and register the necessary services.

Now that your Orleans Silo is configured to use Azure Cognitive Search for indexing your grain states, you'll need to define which grain states should be indexed and which properties of those grain states should be included in the index.

First, add the IndexableStateAttribute to the grain state class that you want to index:
```csharp
using TGHarker.Orleans.Indexing.Core;

[IndexableState]
public class MyGrainState
{
    // ...
}
```

Then, add the IndexablePropertyAttribute to the properties you want to include in the index:
```csharp
using TGHarker.Orleans.Indexing.Core;

[IndexableState]
public class MyGrainState
{
    [IndexableProperty(IsId = true)]
    public Guid Id { get; set; }

    [IndexableProperty(IsSearchable = true)]
    public string Name { get; set; }

    [IndexableProperty(IsFacetable = true)]
    public int Age { get; set; }

    // Other properties you don't want to index...
}

```

You should have exactly one property marked with IsId = true in each grain state class that has the IndexableStateAttribute. This property will be used as the unique identifier for the index.

With these attributes added, the TGHarker.Orleans.Indexing.AzureCognitiveSearch.CodeGenerator package will automatically generate the required index classes for your grain states during the build process.

Now, your grain states are configured for indexing, and the Orleans Silo will automatically handle indexing your grain state data using Azure Cognitive Search whenever the grain state is written. To query the indexed data, you can use Azure Cognitive Search SDKs or REST APIs. You can learn more about querying Azure Cognitive Search in the official documentation.
