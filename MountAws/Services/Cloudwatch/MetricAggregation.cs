using Amazon.CloudWatch.Model;

namespace MountAws.Services.Cloudwatch;

public class MetricAggregation
{
    public static MetricAggregation[] All { get; } =
    {
        new("Minimum", d => d.Minimum),
        new("Maximum", d => d.Maximum),
        new("Average", d => d.Average),
        new("Sum", d => d.Sum),
        new("p50", true),
        new("p75", true),
        new("p80", true),
        new("p90", true),
        new("p95", true),
        new("p99", true),
        new("p99.9", true),
    };

    public MetricAggregation(string name, bool isExtended)
    {
        Name = name;
        IsExtended = isExtended;
    }

    public MetricAggregation(string name, Func<Datapoint, double> valueAccessor)
    {
        Name = name;
        _valueAccessor = valueAccessor;
    }

    private Func<Datapoint, double>? _valueAccessor;
    public string Name { get; }
    public bool IsExtended { get; }

    public double GetValue(Datapoint datapoint)
    {
        if (_valueAccessor != null)
        {
            return _valueAccessor.Invoke(datapoint);
        }
        else
        {
            return datapoint.ExtendedStatistics[Name];
        }
    }

    public static bool TryParse(string name, out MetricAggregation aggregation)
    {
        var result = All.SingleOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (result != null)
        {
            aggregation = result;
            return true;
        }

        aggregation = null!;
        return false;
    }
}