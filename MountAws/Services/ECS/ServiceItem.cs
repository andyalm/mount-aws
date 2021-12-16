using Amazon.ECS.Model;
using MountAnything;

namespace MountAws.Services.ECS;

public class ServiceItem : Item
{
    private readonly Service _service;

    public ServiceItem(string parentPath, Service service) : base(parentPath)
    {
        _service = service;
    }

    public override string ItemName => _service.ServiceName;
    public override object UnderlyingObject => _service;
    public override string ItemType => "Service";
    public override bool IsContainer => true;
}