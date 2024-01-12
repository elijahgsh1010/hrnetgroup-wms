using Hrnetgroup.Wms.Domain.Workers;

namespace Hrnetgroup.Wms.Domain;

public interface IElasticManager
{
    Task BulkIndex<T>(List<T> documents) where T : class;

    Task<List<ElasticWorker>?> Search();
}