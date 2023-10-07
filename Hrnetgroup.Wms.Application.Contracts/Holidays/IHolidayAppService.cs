namespace Hrnetgroup.Wms.Application.Contracts.Holidays;

public interface IHolidayAppService : ITransientService
{
    Task CreateHoliday(CreateHolidayInput input);
    Task DeleteHoliday(int holidayId);
}