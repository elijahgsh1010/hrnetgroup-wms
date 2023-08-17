using System.ComponentModel.DataAnnotations;
using Hrnetgroup.Wms.Application.Contracts;
using Hrnetgroup.Wms.Domain.Leaves;

namespace Hrnetgroup.Wms.Domain.Workers;

public class Worker : Entity<int>, IAggregateRoot
{
    public Worker()
    {
        Leaves = new List<Leave>();
    }
    
    public Worker(string name)
    {
        Name = name;
    }

    [Required]
    [MaxLength(250)]
    public virtual string Name { get; set; }
    
    [Required]
    public virtual DateTime ContractStartDate { get; set; }
    
    [Required]
    public virtual int TotalNumOfWorkingDays { get; set; }
    
    [Required]
    public virtual decimal AmountPerHour { get; set; }
    
    [Required]
    public virtual WorkingDay WorkingDays { get; set; }
    
    [Required]
    public virtual int NumOfHourPerDay { get; set; } // assume work one full hour and assume hour is the same for all the days for simplicity
    
    public virtual ICollection<Leave> Leaves { get; set; }
}