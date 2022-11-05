using System.Management.Automation;
using Amazon.CloudWatch.Model;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class DimensionalMetricItem : AwsItem
{
    public DimensionalMetricItem(ItemPath parentPath, string @namespace, string metricName, IEnumerable<Dimension> dimensions) : base(parentPath, new PSObject())
    {
        Namespace = @namespace;
        MetricName = metricName;
        Dimensions = dimensions.ToArray();
        ItemName = string.Join(".", dimensions.Select(d => d.Value));
    }
    
    [ItemProperty]
    public string Namespace { get; }
    
    [ItemProperty]
    public string MetricName { get; }
    
    [ItemProperty]
    public Dimension[] Dimensions { get; }

    protected override void CustomizePSObject(PSObject psObject)
    {
        base.CustomizePSObject(psObject);

        foreach (var dimension in Dimensions)
        {
            psObject.SetProperty(dimension.Name, dimension.Value);
        }
    }

    public override string ItemName { get; }

    public override bool IsContainer => true;
}