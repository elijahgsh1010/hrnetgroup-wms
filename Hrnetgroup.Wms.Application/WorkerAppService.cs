using System.ComponentModel.DataAnnotations;
using Ardalis.Specification;
using AutoMapper;
using Hrnetgroup.Wms.Application.Contracts.Workers;
using Hrnetgroup.Wms.Application.Contracts.Workers.Dtos;
using Hrnetgroup.Wms.Domain;
using Hrnetgroup.Wms.Domain.Holidays;
using Hrnetgroup.Wms.Domain.Leaves;
using Hrnetgroup.Wms.Domain.Workers;
using Microsoft.AspNetCore.Identity;
using Nest;
using Leave = Hrnetgroup.Wms.Domain.Leaves.Leave;

namespace Hrnetgroup.Wms.Application;

public static class StringExtensions
{
    public static string JoinAsString(this IEnumerable<string> source, string separator)
    {
        return string.Join(separator, source);
    }
}

public class WorkerAppService : IWorkerAppService
{
    private readonly IMapper _mapper;
    private readonly Contracts.IRepository<Worker> _workerRepository;
    private readonly Contracts.IRepository<Holiday> _holidayRepository;
    private readonly Contracts.IRepository<Leave> _leaveRepository;
    private const decimal HolidayMultiplier = 2;
    private const decimal EveHolidayMultiplier = 1.5M;

    private readonly IElasticManager _elasticManager;

    public WorkerAppService(Contracts.IRepository<Worker> workRepository
        , Contracts.IRepository<Holiday> holidayRepository
        , Contracts.IRepository<Leave> leaveRepository
        , IElasticManager elasticManager)
    {
        _workerRepository = workRepository;
        _holidayRepository = holidayRepository;
        _leaveRepository = leaveRepository;
        _elasticManager = elasticManager;
        var mapperConfiguration = new MapperConfiguration(config =>
        {
            config.CreateMap<UpdateWorkerInput, Worker>()
                .ForMember(sub => sub.Leaves, opt => opt.Ignore())
                .ForMember(sub => sub.WorkingDays, opt => opt.Ignore())
                .ForMember(sub => sub.Id, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    foreach (var workingDay in src.WorkingDays)
                    {
                        dest.WorkingDays |= UtilityHelper.GetWorkingDay(workingDay);
                    }
                });
            config.CreateMap<CreateWorkerInput, Worker>()
                .ForMember(sub => sub.WorkingDays, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    foreach (var workingDay in src.WorkingDays)
                    {
                        dest.WorkingDays |= UtilityHelper.GetWorkingDay(workingDay);
                    }
                });
            config.CreateMap<Worker, WorkerDto>()
                .ForMember(sub => sub.Tags, opt => opt.Ignore())
                .ForMember(sub => sub.Leaves, opt => opt.Ignore())
                .ForMember(sub => sub.WorkingDays, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                { 
                    dest.Tags = src.Tags.Split(',').ToList();
                });

            config.CreateMap<Worker, GetAllWorkerOutput>();
            config.CreateMap<Worker, ElasticWorker>()
                .ForMember(sub => sub.Tags, opt => opt.Ignore())
                .ForMember(sub => sub.Leaves, opt => opt.Ignore())
                .ForMember(sub => sub.WorkingDays, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.Tags = src.Tags.Split(',').ToList();
                });
        });
        _mapper = mapperConfiguration.CreateMapper();
    }

    public virtual async Task CreateWorker(CreateWorkerInput input)
    { 
        var worker = _mapper.Map<CreateWorkerInput, Worker>(input);
        
        await _workerRepository.AddAsync(worker);
    }

    public virtual async Task UpdateWorker(UpdateWorkerInput input)
    {
        var worker = await _workerRepository.FirstOrDefaultAsync(new WorkerByIdSpec(input.Id));

        if (worker == null) throw new UserFriendlyException("Worker not found");
        
        _mapper.Map<UpdateWorkerInput, Worker>(input, worker);

        await _workerRepository.SaveChangesAsync();
    }
    
    public virtual async Task AddTags(int id, List<string> tags)
    {
        var worker = await _workerRepository.FirstOrDefaultAsync(new WorkerByIdSpec(id));

        if (worker == null) throw new UserFriendlyException("Worker not found");

        worker.Tags = tags.JoinAsString(",");

        await _workerRepository.SaveChangesAsync();
    }

    public virtual async Task DeleteWorker(int workerId)
    {
        var worker = await _workerRepository.FirstOrDefaultAsync(new WorkerByIdSpec(workerId));

        await _workerRepository.DeleteAsync(worker ?? new Worker());
    }

    // @TODO add filter, page, sort
    public virtual async Task<List<GetAllWorkerOutput>> GetAllWorker()
    {
        var workers = await _workerRepository.ListAsync();
        
        return _mapper.Map<List<Worker>, List<GetAllWorkerOutput>>(workers);
    }

    public virtual async Task ApplyLeave(ApplyLeaveInput input)
    {
        if (input.DateTo < input.DateFrom) throw new UserFriendlyException("Invalid date");

        var leaves = await _leaveRepository.ListAsync(new LeaveByOwnerRangeSpec(input.WorkerId, input.DateFrom, input.DateTo));

        if (leaves.Count > 0) throw new UserFriendlyException("Not allowed");
        
        var worker = await _workerRepository.FirstOrDefaultAsync(new WorkerByIdSpec(input.WorkerId));

        if (worker == null) throw new UserFriendlyException("Worker not found");
        
        worker.Leaves.Add(new Leave()
        {
            DateFrom = input.DateFrom,
            DateTo = input.DateTo.Date.AddDays(1).AddTicks(-1)
        });

        await _workerRepository.SaveChangesAsync();
    }

    public virtual async Task DeleteLeave(DeleteLeaveInput input)
    {
        var leave = await _leaveRepository.FirstOrDefaultAsync(new LeaveByOwnerIdSpec(input.WorkerId, input.LeaveId));

        await _leaveRepository.DeleteAsync(leave);
    }

    public virtual async Task<WorkerDto> GetWorkerInformation([Required] int workerId)
    {
        var worker = await _workerRepository.FirstOrDefaultAsync(new WorkerByIdSpec(workerId));

        if (worker == null) throw new UserFriendlyException("Worker not found");
        
        var workerDto = _mapper.Map<Worker, WorkerDto>(worker);

        var workingDayOfWeek = (from e in Enum.GetValues(typeof(WorkingDay)).Cast<WorkingDay>() where worker.WorkingDays.HasFlag(e) select UtilityHelper.GetDayOfWeek(e)).ToArray();

        var leaves = worker.Leaves.SelectMany(x => UtilityHelper.GetDatesBetween(x.DateFrom, x.DateTo)).Distinct().ToArray();
        
        workerDto.ExpectedEndDate = GetExpectedEndDate(worker.ContractStartDate, worker.TotalNumOfWorkingDays, workingDayOfWeek, leaves);

        var holidays = await GetHolidayWithinRange(worker.ContractStartDate, workerDto.ExpectedEndDate);
        
        workerDto.TotalSalary = CalculateTotalSalary(worker.AmountPerHour, worker.NumOfHourPerDay, worker.TotalNumOfWorkingDays, workingDayOfWeek, holidays);

        workerDto.WorkingDays = workingDayOfWeek.Select(x => x.ToString()).ToArray();

        workerDto.Leaves = leaves;
        
        return workerDto;
    }

    private DateTime GetExpectedEndDateOptimize(DateTime start, int numOfDays, DayOfWeek[] workingDays, DateTime[] leaves)
    {
        int workingDayCount = workingDays.Length;

        var finalNumOfDays = workingDays.Contains(start.DayOfWeek) ? numOfDays - 1 : numOfDays;
        int totalDays = (numOfDays / workingDayCount) * 7;
        int remainingDays = numOfDays % workingDayCount;

        DateTime expectedEndDate = start.AddDays(totalDays);

        foreach (var leave in leaves)
        {
            if (leave >= start && leave <= expectedEndDate && workingDays.Contains(leave.DayOfWeek))
            {
                finalNumOfDays++;
            }
        }

        if (finalNumOfDays != numOfDays)
        {
            totalDays = (finalNumOfDays / workingDayCount) * 7;
            remainingDays = finalNumOfDays % workingDayCount;

            expectedEndDate = start.AddDays(totalDays);
        }

        while (remainingDays > 0)
        {
            expectedEndDate = expectedEndDate.AddDays(1);
            if (workingDays.Contains(expectedEndDate.DayOfWeek))
            {
                remainingDays--;
            }
        }

        return expectedEndDate;
    }

    private DateTime GetExpectedEndDate(DateTime start, int numOfDays, DayOfWeek[] workingDays, DateTime[] leaves)
    {
        var result = start;

        while (numOfDays > 0)
        {
            if (workingDays.Contains(result.DayOfWeek) && !leaves.Contains(result.Date))
            {
                numOfDays--;
            }

            if (numOfDays != 0)
            {
                result = result.AddDays(1);
            }
        }

        return result;
    }
    
    private decimal CalculateTotalSalary(decimal amountPerHour, int hourPerDay, int numOfDays, DayOfWeek[] workingDays, DateTime[] holidays)
    {
        decimal total = (hourPerDay * amountPerHour) * numOfDays;

        const int actualHoliday = 1;
        const int holidayEve = 2;

        var stack = new Stack<int>();

        foreach (var holiday in holidays)
        {
            if (workingDays.Contains(holiday.DayOfWeek))
            {
                stack.Push(actualHoliday);
            }

            if (workingDays.Contains(holiday.AddDays(-1).DayOfWeek))
            {
                stack.Push(holidayEve);
            }
        }

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            total -= (hourPerDay * amountPerHour);
            if (current == actualHoliday)
            {
                total += (amountPerHour * HolidayMultiplier) * hourPerDay;
            }

            if (current == holidayEve)
            {
                total += (amountPerHour * EveHolidayMultiplier) * hourPerDay;
            }
        }
        
        return total;
    }

    private async Task<DateTime[]> GetHolidayWithinRange(DateTime start, DateTime end)
    {
        var holidays = await _holidayRepository.ListAsync(new HolidayByRangeSpec(start, end));

        return holidays.Select(x => x.Date).ToArray();
    }

    public virtual async Task BulkIndexWorker()
    {
        var workers = await _workerRepository.ListAsync();

        var result = _mapper.Map<List<Worker>, List<ElasticWorker>>(workers);
        
        await _elasticManager.BulkIndex(result);
    }

    public virtual async Task<List<ElasticWorker>> SearchWorker(string name)
    {
        var result = await _elasticManager.Search();

        return result ?? new List<ElasticWorker>();
    }
}