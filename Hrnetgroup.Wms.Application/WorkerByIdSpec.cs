using Ardalis.Specification;
using Hrnetgroup.Wms.Domain.Workers;

namespace Hrnetgroup.Wms.Application;

public class WorkerByIdSpec : Specification<Worker>
{
    public WorkerByIdSpec(int id) =>
        Query
            .Where(x => x.Id == id)
            .Include(x => x.Leaves);
}

