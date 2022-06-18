namespace MountAws.Services.Cloudwatch;

public class MetricTimeframe
{
    public static MetricTimeframe[] All { get; } = 
    {
        new("1h", System.TimeSpan.FromHours(1), 60, "Last hour"),
        new("3h", System.TimeSpan.FromHours(3), 60, "Last 3 hours"),
        new("12h", System.TimeSpan.FromHours(12), 60, "Last 12 hours"),
        new("1d", System.TimeSpan.FromDays(1), 300, "Last day"),
        new("3d", System.TimeSpan.FromDays(3), 300, "Last 3 days"),
        new("1w", System.TimeSpan.FromDays(7), 3600, "Last week"),
        new("2w", System.TimeSpan.FromDays(14), 3600, "Last 2 weeks"),
        new("1m", System.TimeSpan.FromDays(30), 3600, "Last month"),
        new("3m", System.TimeSpan.FromDays(90), 3600, "Last 3 months"),
        new("6m", System.TimeSpan.FromDays(180), 3600, "Last 6 months"),
        new("12m", System.TimeSpan.FromDays(365), 3600, "Last year"),
        new("15m", System.TimeSpan.FromDays(425), 3600, "Last 15 months"),
    };

    public static MetricTimeframe FromName(string name)
    {
        return All.Single(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public static bool TryFromName(string name, out MetricTimeframe timeframe)
    {
        var result = All.SingleOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (result != null)
        {
            timeframe = result;
            return true;
        }

        timeframe = null!;
        return false;
    }

    public MetricTimeframe(string name, TimeSpan timespan, int periodInSeconds, string description)
    {
        Name = name;
        TimeSpan = timespan;
        Period = periodInSeconds;
        Description = description;
    }

    public string Name { get; }
    public TimeSpan TimeSpan { get; }
    public int Period { get; }
    public string Description { get; }
    public (DateTime StartDate, DateTime EndDate) GetStartEndDatesUtc()
    {
        return (DateTime.UtcNow - TimeSpan, DateTime.UtcNow);
    }
}