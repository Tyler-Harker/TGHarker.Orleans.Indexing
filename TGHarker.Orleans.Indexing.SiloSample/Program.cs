using TGHarker.Orleans.Indexing;
using TGHarker.Orleans.Indexing.AzureCognitiveSearch;
using TGHarker.Orleans.Indexing.Core;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseOrleans(orleansBuilder =>
{
    const string inMemoryStorageName = "memory";
    orleansBuilder.UseLocalhostClustering();
    orleansBuilder.AddMemoryGrainStorage(inMemoryStorageName);
    orleansBuilder.AddIndexing((indexBuilder) =>
    {
        indexBuilder.ConfigureOptions = (indexOptions) =>
        {
            indexOptions.PrimaryStorageProviderName = inMemoryStorageName;
            indexOptions.AwaitIndexSave = true;
            indexOptions.GrainStateAssemblies = new[]
            {
                typeof(Program).Assembly
            };
        };
        indexBuilder.UseAzureCognitiveSearch(azureOptions =>
        {
            azureOptions.ApiKey = "eX15jzvgdCyLypmqI0KxRKOf7yOmcA6zQT9RiPUz2pAzSeDpyIxV";
            azureOptions.Uri = new Uri("https://didentitysearch.search.windows.net");
        });
    });
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
