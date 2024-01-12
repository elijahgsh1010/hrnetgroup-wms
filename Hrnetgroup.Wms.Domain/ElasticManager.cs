using System.Runtime.ExceptionServices;
using Hrnetgroup.Wms.Domain.Workers;
using Nest;

namespace Hrnetgroup.Wms.Domain;

public class ElasticManager : IElasticManager
{
    private readonly ElasticClient _elasticClient;
    public const string LiveIndexAlias = "workers-current";
    public const string OldIndexAlias = "workers-old";
    public ElasticManager(ElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public virtual async Task<List<ElasticWorker>?> Search()
    {
        var result = _elasticClient.Search<ElasticWorker>(s => s);

        return result.Documents as List<ElasticWorker>;
    }

    public virtual async Task BulkIndex<T>(List<T> documents)  where T : class
    {
        var newIndex = CreateIndexName();
        await CreateIndexAsync<T>(newIndex);
        
        var response = _elasticClient.BulkAll<T>(documents, 
            b => b
                .Index(newIndex)
                .BackOffTime(new TimeSpan(0, 0, 30))
                .BackOffRetries(3)
                .Size(100)
        );
        
        var waitHandle = new CountdownEvent(1);
        
        ExceptionDispatchInfo captureInfo = null;

        response.Subscribe(new BulkAllObserver(
            onNext: b => Console.Write("."),
            onError: e =>
            {
                captureInfo = ExceptionDispatchInfo.Capture(e);
                waitHandle.Signal();
            },
            onCompleted: () => waitHandle.Signal()
        ));

        waitHandle.Wait();
        captureInfo?.Throw();
        Console.WriteLine("Done.");
        await SwapAlias(newIndex);
    }
    
    private async Task SwapAlias(string currentIndexName)
    {
        var indexExists = _elasticClient.Indices.Exists(LiveIndexAlias).Exists;

        await _elasticClient.Indices.BulkAliasAsync(aliases =>
        {
            if (indexExists)
                aliases.Add(a => a
                    .Alias(OldIndexAlias)
                    .Index(_elasticClient.GetIndicesPointingToAlias(LiveIndexAlias).First())
                );

            return aliases
                .Remove(a => a.Alias(LiveIndexAlias).Index("*"))
                .Add(a => a.Alias(LiveIndexAlias).Index(currentIndexName));
        });

        var oldIndices = (await _elasticClient.GetIndicesPointingToAliasAsync(OldIndexAlias))
                .OrderByDescending(name => name)
                .Skip(2);

        foreach (var oldIndex in oldIndices)
        {
            await _elasticClient.Indices.DeleteAsync(oldIndex);
        }
    }
    
    /// <summary>
    /// CreateEsIndex Not Mapping
    /// Auto Set Alias alias is Input IndexName
    /// </summary>
    /// <param name="indexName"></param>
    /// <param name="shard"></param>
    /// <param name="numberOfReplicas"></param>
    /// <returns></returns>
    public virtual async Task CreateIndexAsync<T>(string indexName, int shard = 1, int numberOfReplicas = 1)
        where T : class
    {
        await CreateIndexAsync<T>(indexName, indexSettingsDescriptor => indexSettingsDescriptor.NumberOfShards(shard).NumberOfReplicas(numberOfReplicas));
    }
    
    private async Task CreateIndexAsync<T>(string indexName, Func<IndexSettingsDescriptor, IndexSettingsDescriptor> selector) where T : class
    {
        await _elasticClient.Indices.CreateAsync(indexName,
            ss => ss
                .Index(indexName)
                .Settings(indexSettingsDescriptor => selector(indexSettingsDescriptor).Setting("max_result_window", int.MaxValue))
                .Map(m => m.AutoMap<T>()));
    }
    
    public static string CreateIndexName() => $"{LiveIndexAlias}-{DateTime.UtcNow:dd-MM-yyyy-HH-mm-ss}";
}