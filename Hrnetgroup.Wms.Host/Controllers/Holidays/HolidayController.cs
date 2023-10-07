using Hrnetgroup.Wms.Application.Contracts.Holidays;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrnetgroup.Wms.Controllers.Holidays;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public class HolidayController : ControllerBase
{
    private readonly IHolidayAppService _holidayAppService;
    
    public HolidayController(IHolidayAppService holidayAppService)
    {
        _holidayAppService = holidayAppService;
    }
    
    [HttpPost(Name = "CreateHoliday")]
    public Task CreateHoliday(CreateHolidayInput input)
    {
        return _holidayAppService.CreateHoliday(input);
    }

    [HttpPost(Name = "DeleteHoliday")]
    public Task DeleteHoliday(int holidayId)
    {
        return _holidayAppService.DeleteHoliday(holidayId);
    }
}