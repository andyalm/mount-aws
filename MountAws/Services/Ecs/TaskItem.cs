using System.Collections.Immutable;
using Amazon.EC2.Model;
using Amazon.ECS.Model;
using MountAnything;
using Task = Amazon.ECS.Model.Task;

namespace MountAws.Services.Ecs;

public class TaskItem : AwsItem<Task>
{
    private readonly Instance? _ec2Instance;
    private readonly Lazy<Endpoint[]> _endpoints;
    private readonly bool _useServiceView;

    public TaskItem(ItemPath parentPath, Task task, Instance? ec2Instance, LinkGenerator linkGenerator, bool useServiceView) : base(parentPath, task)
    {
        _ec2Instance = ec2Instance;
        _endpoints = new Lazy<Endpoint[]>(GetEndpoints);
        _useServiceView = useServiceView;
        ItemName = task.TaskArn.Split("/").Last();
        LinkPaths = ImmutableDictionary.Create<string,ItemPath>()
            .Add("TaskDefinition", linkGenerator.TaskDefinition(task.TaskDefinitionArn));
    }

    // We want to use different views based on whether we are in the context of a service or not, so we vary the type name to select the correct view
    protected override string TypeName =>
        _useServiceView ? GetType().FullName!.Replace(".TaskItem", ".ServiceTaskItem") : GetType().FullName!;

    public override string ItemName { get; }
    public override string ItemType => EcsItemTypes.Task;

    public override string? WebUrl =>
        UrlBuilder.CombineWith($"ecs/home#/clusters/{UnderlyingObject.ClusterName()}/tasks/{ItemName}");
    public override bool IsContainer => false;

    [ItemProperty] 
    public Endpoint[] Endpoints => _endpoints.Value;

    private Endpoint[] GetEndpoints()
    {
        var containerEndpoints = UnderlyingObject.Containers
            .SelectMany(c => c.NetworkInterfaces)
            .Where(i => !string.IsNullOrEmpty(i.PrivateIpv4Address))
            .Select(i => new Endpoint(i.PrivateIpv4Address))
            .ToArray();

        if (containerEndpoints.Any())
        {
            return containerEndpoints;
        }
            
        var ec2Ip = _ec2Instance?.PrivateIpAddress;
        if (ec2Ip == null)
        {
            return Array.Empty<Endpoint>();
        }

        return UnderlyingObject.Containers.SelectMany(c => c.NetworkBindings)
            .Select(b => new Endpoint(ec2Ip, b.HostPort))
            .ToArray();
    }

    [ItemProperty] 
    public Endpoint? Endpoint => Endpoints.FirstOrDefault();
}