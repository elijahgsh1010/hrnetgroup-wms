using Ardalis.Specification;

namespace Hrnetgroup.Wms.Application.Contracts;

public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
{
    
}
