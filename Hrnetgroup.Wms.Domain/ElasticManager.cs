using System.Runtime.ExceptionServices;
using Nest;

namespace Hrnetgroup.Wms.Domain;

public class ElasticManager : IElasticManager
{
    private readonly ElasticClient _elasticClient;
    public ElasticManager(ElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }


    public virtual async Task BulkIndex<T>(List<T> documents)  where T : class
    {
        var response = _elasticClient.BulkAll<T>(documents, b =>
                b.BackOffTime(new TimeSpan(0, 0, 30))
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
    }
}