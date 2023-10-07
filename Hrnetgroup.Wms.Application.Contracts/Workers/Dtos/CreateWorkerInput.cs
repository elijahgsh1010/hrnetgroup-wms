using System.ComponentModel.DataAnnotations;

namespace Hrnetgroup.Wms.Application.Contracts.Workers.Dtos;

public class CreateWorkerInput : IValidatableObject
{
    [Required]
    public string Name { get; set; }
    [Required]
    public DateTime ContractStartDate { get; set; }
    [Required]
    public int TotalNumOfWorkingDays { get; set; }
    [Required]
    public decimal AmountPerHour { get; set; }
    [Required]
    public DayOfWeek[] WorkingDays { get; set; }
    [Required]
    public int NumOfHourPerDay { get; set; } // assume work one full hour and assume hour is the same for all the days for simplicity

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(Name))
        {
            yield return new ValidationResult(
                $"{nameof(CreateWorkerInput)}.{nameof(Name)} is not configured",
                new[] { nameof(Name) });
        }
    }
}

public class UpdateWorkerInput : CreateWorkerInput
{
    [Required]
    public int Id { get; set; }
}