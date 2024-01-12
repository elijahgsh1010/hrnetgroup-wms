using Hrnetgroup.Wms.Application.Contracts.Workers;
using Hrnetgroup.Wms.Application.Contracts.Workers.Dtos;
using Hrnetgroup.Wms.Domain.Workers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrnetgroup.Wms.Controllers.Workers;

[ApiController]
[Route("api/[controller]/[action]")]
public class WorkerController : ControllerBase
{
    private readonly IWorkerAppService _workerAppService;
    
    public WorkerController(IWorkerAppService workerAppService)
    {
        _workerAppService = workerAppService;
    }

    [HttpPost(Name = "CreateWorker")]
    public Task CreateWorker(CreateWorkerInput input)
    {
        return _workerAppService.CreateWorker(input);
    }

    [HttpPost(Name = "UpdateWorker")]
    public Task UpdateWorker(UpdateWorkerInput input)
    {
        return _workerAppService.UpdateWorker(input);
    }

    [HttpDelete(Name = "DeleteWorker")]
    public Task DeleteWorker(int workerId)
    {
        return _workerAppService.DeleteWorker(workerId);
    }

    [HttpGet(Name = "GetAllWorker")]
    public Task<List<GetAllWorkerOutput>> GetAllWorker()
    {
        return _workerAppService.GetAllWorker();
    }

    [HttpGet(Name = "GetWorkerInformation")]
    public Task<WorkerDto> GetWorkerInformation(int workerId)
    {
        return _workerAppService.GetWorkerInformation(workerId);
    }

    [HttpPost(Name = "ApplyLeave")]
    public Task ApplyLeave(ApplyLeaveInput input)
    {
        return _workerAppService.ApplyLeave(input);
    }

    [HttpDelete(Name = "DeleteLeave")]
    public Task DeleteLeave(DeleteLeaveInput input)
    {
        return _workerAppService.DeleteLeave(input);
    }
    
    [HttpGet]
    [Route("bulk-index")]
    public Task BulkIndexWorker()
    {
        return _workerAppService.BulkIndexWorker();
    }
    
    [HttpPost]
    [Route("add-tags")]
    public Task AddTags(int id, List<string> tags)
    {
        return _workerAppService.AddTags(id, tags);
    }
    
    [HttpGet]
    [Route("search-worker")]
    public Task<List<ElasticWorker>> SearchWorkers(string name)
    {
        return _workerAppService.SearchWorker(name);
    }
}