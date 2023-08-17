using AutoMapper;
using Hrnetgroup.Wms.Application.Contracts;
using Hrnetgroup.Wms.Application.Contracts.Holidays;
using Hrnetgroup.Wms.Domain;
using Hrnetgroup.Wms.Domain.Holidays;

namespace Hrnetgroup.Wms.Application;

public class HolidayAppService : IHolidayAppService
{
    private readonly IRepository<Holiday> _holidayRepository;
    private readonly IMapper _mapper;
    
    public HolidayAppService(IRepository<Holiday> holidayRepository)
    {
        _holidayRepository = holidayRepository;
        var mapperConfiguration = new MapperConfiguration(config =>
        {
            config.CreateMap<CreateHolidayInput, Holiday>();
        });
        _mapper = mapperConfiguration.CreateMapper();
    }

    public virtual async Task CreateHoliday(CreateHolidayInput input)
    {
        var exist = await _holidayRepository.FirstOrDefaultAsync(new HolidayByRangeSpec(input.Date, DateTime.Now));

        if (exist != null) throw new UserFriendlyException("Holiday exists");
            
        var holiday = _mapper.Map<CreateHolidayInput, Holiday>(input);

        await _holidayRepository.AddAsync(holiday);
    }
    
    public virtual async Task DeleteHoliday(int holidayId)
    {
        var holiday = await _holidayRepository.GetByIdAsync(holidayId);

        await _holidayRepository.DeleteAsync(holiday);
    }
}