using AutoMapper;
using Hrnetgroup.Wms.Application.Contracts.Workers;

namespace Hrnetgroup.Wms.Application;

public class WorkerAppService : IWorkerAppService
{
    private readonly IMapper _mapper;
    
    public WorkerAppService()
    {
        var mapperConfiguration = new MapperConfiguration(config =>
        {

        });
        _mapper = mapperConfiguration.CreateMapper();
    }

    public virtual async Task CreateWorker()
    {

    }
}