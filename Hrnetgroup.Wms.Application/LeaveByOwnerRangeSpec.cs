using Ardalis.Specification;
using Hrnetgroup.Wms.Domain.Leaves;

namespace Hrnetgroup.Wms.Application;

public class LeaveByOwnerRangeSpec : Specification<Leave>
{
    
    public LeaveByOwnerRangeSpec(int workerId, DateTime start, DateTime end) =>
        Query
            .AsNoTracking()
            .Where(x => x.WorkerId == workerId)
            .Where(x => (x.DateFrom.Date <= end.Date && start.Date <= x.DateTo.Date));
}

public class LeaveByOwnerIdSpec : Specification<Leave>
{
    public LeaveByOwnerIdSpec(int workerId, int leaveId) =>
        Query
            .AsNoTracking()
            .Where(x => x.WorkerId == workerId)
            .Where(x => x.Id == leaveId);
}
