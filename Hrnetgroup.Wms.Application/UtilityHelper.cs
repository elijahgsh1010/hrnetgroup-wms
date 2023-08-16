using Hrnetgroup.Wms.Domain.Workers;

namespace Hrnetgroup.Wms.Application;

public class UtilityHelper
{
    public static WorkingDay GetWorkingDay(DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Sunday => WorkingDay.Sunday,
            DayOfWeek.Monday => WorkingDay.Monday,
            DayOfWeek.Tuesday => WorkingDay.Tuesday,
            DayOfWeek.Wednesday => WorkingDay.Wednesday,
            DayOfWeek.Thursday => WorkingDay.Thursday,
            DayOfWeek.Friday => WorkingDay.Friday,
            DayOfWeek.Saturday => WorkingDay.Saturday,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public static DayOfWeek GetDayOfWeek(WorkingDay day)
    {
        return day switch
        {
            WorkingDay.Sunday => DayOfWeek.Sunday,
            WorkingDay.Monday => DayOfWeek.Monday,
            WorkingDay.Tuesday => DayOfWeek.Tuesday,
            WorkingDay.Wednesday => DayOfWeek.Wednesday,
            WorkingDay.Thursday => DayOfWeek.Thursday,
            WorkingDay.Friday => DayOfWeek.Friday,
            WorkingDay.Saturday => DayOfWeek.Saturday,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public static List<DateTime> GetDatesBetween(DateTime startDate, DateTime endDate)
    {
        List<DateTime> dates = new List<DateTime>();

        for (DateTime date = startDate.Date; date <= endDate; date = date.AddDays(1))
        {
            dates.Add(date);
        }

        return dates;
    }
}