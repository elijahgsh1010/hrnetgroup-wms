using Ardalis.Specification.EntityFrameworkCore;
using Hrnetgroup.Wms.Application.Contracts;

namespace Hrnetgroup.Wms.EntityframeworkCore;

public class EfRepository<T> : RepositoryBase<T>, IRepository<T> where T : class, IAggregateRoot
{
    public EfRepository(WmsDbContext dbContext) : base(dbContext)
    {
    }
}
