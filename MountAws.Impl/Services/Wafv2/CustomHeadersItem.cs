using Amazon.WAFV2.Model;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Wafv2;

public class CustomHeadersItem : GenericContainerItem
{
    public IEnumerable<CustomHTTPHeader> CustomHeaders { get; }

    public CustomHeadersItem(ItemPath parentPath, string name, string description, IEnumerable<CustomHTTPHeader> customHeaders) : base(parentPath, name, description)
    {
        CustomHeaders = customHeaders;
    }
}