using Hrnetgroup.Wms.Application.Contracts.Workers.Dtos;

namespace Hrnetgroup.Wms.Application.Contracts.Workers;

public interface IWorkerAppService : ITransientService
{
    Task CreateWorker(CreateWorkerInput input);
    Task UpdateWorker(UpdateWorkerInput input);
    Task DeleteWorker(int workerId);
    Task<List<GetAllWorkerOutput>> GetAllWorker();
    Task<WorkerDto> GetWorkerInformation(int workerId);
    Task ApplyLeave(ApplyLeaveInput input);
    Task DeleteLeave(DeleteLeaveInput input);
    Task BulkIndexWorker();
}