namespace Hrnetgroup.Wms.Application.Contracts.Holidays;

public interface IHolidayAppService
{
    Task CreateHoliday(CreateHolidayInput input);
    Task DeleteHoliday(int holidayId);
}