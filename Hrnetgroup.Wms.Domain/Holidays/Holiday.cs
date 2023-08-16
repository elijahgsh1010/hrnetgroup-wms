using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hrnetgroup.Wms.Application.Contracts;

namespace Hrnetgroup.Wms.Domain.Holidays;

public class Holiday : Entity<int>, IAggregateRoot
{
    [Required]
    public virtual string Name { get; set; }
    
    [Column(TypeName = "Date")]
    [Required]
    public virtual DateTime Date { get; set; }
}