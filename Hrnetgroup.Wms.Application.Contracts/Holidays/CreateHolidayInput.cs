using System.ComponentModel.DataAnnotations;

namespace Hrnetgroup.Wms.Application.Contracts.Holidays;

public class CreateHolidayInput
{
    [Required]
    public string Name { get; set; }
    [Required]
    public DateTime Date { get; set; }
}