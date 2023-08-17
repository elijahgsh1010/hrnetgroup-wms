using Ardalis.Specification;
using Hrnetgroup.Wms.Domain.Holidays;

namespace Hrnetgroup.Wms.Application;

public class HolidayByRangeSpec : Specification<Holiday>
{
    public HolidayByRangeSpec(DateTime start, DateTime end) =>
        Query
            .Where(x => x.Date >= start && x.Date <= end.Date);
}