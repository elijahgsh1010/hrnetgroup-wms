using Hrnetgroup.Wms.Domain.Leaves;

namespace Hrnetgroup.Wms.Domain.Workers;

public class ElasticWorker 
{
    public int Id { get; set; }
    
    public virtual string Name { get; set; }
    
    public virtual DateTime ContractStartDate { get; set; }
    
    public virtual int TotalNumOfWorkingDays { get; set; }
    
    public virtual decimal AmountPerHour { get; set; }
    
    public virtual WorkingDay WorkingDays { get; set; }
    
    public virtual int NumOfHourPerDay { get; set; } // assume work one full hour and assume hour is the same for all the days for simplicity
    
    public virtual ICollection<Leave> Leaves { get; set; }
    
    public List<string> Tags { get; set; }
}