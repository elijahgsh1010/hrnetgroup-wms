namespace Hrnetgroup.Wms.Domain;

public interface IElasticManager
{
    Task BulkIndex<T>(List<T> documents) where T : class;
}