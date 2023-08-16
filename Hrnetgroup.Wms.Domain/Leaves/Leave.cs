using System.ComponentModel.DataAnnotations;
using Hrnetgroup.Wms.Application.Contracts;
using Hrnetgroup.Wms.Domain.Workers;

namespace Hrnetgroup.Wms.Domain.Leaves;

public class Leave : Entity<int>, IAggregateRoot
{
    [Required]
    public virtual DateTime DateFrom { get; set; }
    
    [Required]
    public virtual DateTime DateTo{ get; set; }
    
    [Required]
    public virtual int WorkerId { get; set; }
    
    [Required]
    public virtual Worker Worker { get; set; }
}