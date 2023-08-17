namespace Hrnetgroup.Wms.Application.Contracts.Workers.Dtos;

public class WorkerDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string[] WorkingDays { get; set; }
    public DateTime ExpectedEndDate { get; set; }
    public decimal TotalSalary { get; set; }
    public DateTime[] Leaves { get; set; }
    public virtual DateTime ContractStartDate { get; set; }
}