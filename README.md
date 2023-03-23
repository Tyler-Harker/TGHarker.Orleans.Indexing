# TGHarker.Orleans.Indexing

TGHarker.Orleans.Indexing is an indexing library for Orleans grains that provides an easy way to generate and manage indexes for your grain state. This library currently supports Orleans 7.0.

## Features

- `IndexableStateAttribute`: This attribute goes on your grain's state. It tells the library that we should generate an index using the configured provider.
- `IndexablePropertyAttribute`: This attribute goes on your grain state's properties. It tells the library that this property should be included in the index. It has some customizable options: `IsId`, `IsFacetable`, `IsSearchable`, `IsHidden`, `IsSortable`. Every State object needs to have 1 and only 1 property declared as `IsId=true`.

### IndexProviders

Currently, the following index providers are supported:

1. **Azure Cognitive Search**: Azure Cognitive Search is a fully managed search-as-a-service that provides a rich search experience over private, heterogeneous content in web, mobile, and enterprise applications. You can find the pricing details [here](https://azure.microsoft.com/en-us/pricing/details/cognitive-services/search-services/) and the setup guide [here](https://docs.microsoft.com/en-us/azure/search/search-get-started-portal).

ElasticSearch is being worked on, but is not currently available. Other index providers can be added in the future.

## Getting Started

1. Install the `TGHarker.Orleans.Indexing.AzureCognitiveSearch` NuGet package to the project where your grain implementations are.
2. Install the `TGHarker.Orleans.Indexing.AzureCognitiveSearch.CodeGenerator` NuGet package to the project that contains your grain state objects.

### Adding the Indexing to Your Orleans Silo

To add indexing to your Orleans Silo, you can use the extension method `AddAzureCognitiveSearchIndexing`:

```csharp
using System;
using TGHarker.Orleans.Indexing.AzureCognitiveSearch;

var builder = new SiloBuilder()
    .Configure<ClusterOptions>(options => { /* ... */ })
    .AddAzureCognitiveSearchIndexing(new AzureSearchOptions
    {
        Uri = new Uri("https://your-search-service-name.search.windows.net"),
        ApiKey = "your-api-key"
    });
```
## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Usage

### Step 1: Decorate Grain State with Attributes

In your grain state class, add the `IndexableStateAttribute` to the class, and `IndexablePropertyAttribute` to the properties you want to index.

```csharp
using TGHarker.Orleans.Indexing.Core;

[IndexableState]
public class MyGrainState
{
    [IndexableProperty(IsId = true)]
    public Guid Id { get; set; }

    [IndexableProperty]
    public string Name { get; set; }

    [IndexableProperty(IsFacetable = true)]
    public int Age { get; set; }

    public double UnindexedProperty { get; set; }
}
```
### Step 2: Configure Silo to Use Azure Cognitive Search

In your Silo configuration, use the `AddIndexing` method to enable indexing, and configure it to use Azure Cognitive Search:

```csharp
using TGHarker.Orleans.Indexing;
using TGHarker.Orleans.Indexing.AzureCognitiveSearch;

var builder = new SiloBuilder()
    .Configure<ClusterOptions>(options => { /* ... */ })
    .AddIndexing((builder) =>
    {
        builder.ConfigureOptions = options =>
        {
            options.PrimaryStorageProviderName = "YourPrimaryStorageProviderName";
            options.GrainStateAssemblies = new[] { typeof(MyGrainState).Assembly };
        };
        builder.Services.UseAzureCognitiveSearch(options =>
        {
            options.Uri = new Uri("https://your-search-service-name.search.windows.net");
            options.ApiKey = "your-search-service-api-key";
        });
    });
```
Replace `your-search-service-name` and `your-search-service-api-key` with your Azure Cognitive Search service name and API key, respectively. Also, replace `YourPrimaryStorageProviderName` with the name of your primary storage provider (e.g., "AzureTableGrainStorage").

### Step 3: Querying the Index

You can now query the index using Azure Cognitive Search SDK or REST API. For example, to search for grains with a specific name or age, you can use the following code snippet:

```csharp
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;

var searchClient = new SearchClient(new Uri("https://your-search-service-name.search.windows.net"), "mygrainstate-index", new AzureKeyCredential("your-search-service-api-key"));

SearchResults<SearchDocument> response = searchClient.Search<SearchDocument>("Name:John OR Age:25");

foreach (SearchResult<SearchDocument> result in response.GetResults())
{
    Console.WriteLine($"Found: {result.Document}");
}
```
Replace `your-search-service-name` and `-service-api-key` with your Azure Cognitive Search service name and API key, respectively.

This code snippet demonstrates how to use the Azure Cognitive Search SDK to search for grains that have the name "John" or the age 25.

## Contributing

If you'd like to contribute to this project, please feel free to submit a pull request or open an issue to discuss your ideas.

## Acknowledgments

- Thanks to the Orleans team for their fantastic work on the Orleans framework.
- Thanks to Azure Cognitive Search for providing a powerful search service.

