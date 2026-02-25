using Amazon.Lambda.Model;
using MountAnything;

namespace MountAws.Services.Lambda;

public class EventSourceMappingItem : AwsItem<EventSourceMappingConfiguration>
{
    public EventSourceMappingItem(ItemPath parentPath, EventSourceMappingConfiguration mapping)
        : base(parentPath, mapping) { }

    public override string ItemName => UnderlyingObject.UUID;
    public override string ItemType => LambdaItemTypes.EventSourceMapping;
    public override bool IsContainer => false;
}
