using System.ComponentModel.DataAnnotations;

namespace Hrnetgroup.Wms.Application.Contracts.Workers.Dtos;

public class ApplyLeaveInput
{
    [Required]
    public int WorkerId { get; set; }
    [Required]
    public DateTime DateFrom { get; set; }
    [Required]
    public DateTime DateTo { get; set; }
}