namespace ModelRepoBrowser;

public class CrawlerScheduler
{
    private readonly Timer timer;

    public TimeOnly PreferedTime { get; private set; }

    public CrawlerScheduler(TimeOnly preferedTime)
    {
        PreferedTime = preferedTime;
        timer = new Timer(Action);
        ScheduleAction();
    }

    private void Action(object? obj)
    {
        Console.WriteLine($"Crawling... <{DateTime.Now.TimeOfDay}>");
        ScheduleAction();
    }

    private void ScheduleAction()
    {
        TimeSpan timeUntilPreferedTime = GetTimeSpanUntilPreferedTime(DateTime.Now);
        Console.WriteLine($"Schedule crawler to run timespan:<{timeUntilPreferedTime}>");
        timer.Change(timeUntilPreferedTime, Timeout.InfiniteTimeSpan);
    }

    public TimeSpan GetTimeSpanUntilPreferedTime(DateTime currentDateTime)
    {
        var preferedDateTime = currentDateTime.Date + PreferedTime.ToTimeSpan();
        if (preferedDateTime <= currentDateTime)
        {
            preferedDateTime = preferedDateTime.AddDays(1);
        }

        return preferedDateTime - currentDateTime;
    }
}
