using System.Management.Automation;
using MountAnything;

namespace MountAws;

public abstract class AwsItem : Item
{
    private readonly Lazy<WebUrlBuilder> _webUrlBuilder;

    protected AwsItem(ItemPath parentPath, PSObject underlyingObject) : base(parentPath, underlyingObject)
    {
        _webUrlBuilder = new Lazy<WebUrlBuilder>(CreateWebUrlBuilder);
    }

    protected override string TypeName => GetType().FullName!;
    
    public override string ItemType => GetType().Name.EndsWith("Item")
        ? GetType().Name.Remove(GetType().Name.Length - 4)
        : GetType().Name;
    
    [ItemProperty]
    public virtual string? WebUrl { get; }

    protected WebUrlBuilder UrlBuilder => _webUrlBuilder.Value;
    private WebUrlBuilder CreateWebUrlBuilder()
    {
        if (FullPath.Parts.Length < 2)
        {
            return WebUrlBuilder.Regionless();
        }
        else
        {
            return WebUrlBuilder.ForRegion(FullPath.Parts[1]);
        }
    }
}

public abstract class AwsItem<T> : Item<T> where T : class
{
    private readonly Lazy<WebUrlBuilder> _webUrlBuilder;

    protected AwsItem(ItemPath parentPath, T underlyingObject) : base(parentPath, underlyingObject)
    {
        _webUrlBuilder = new Lazy<WebUrlBuilder>(CreateWebUrlBuilder);
    }

    protected override string TypeName => GetType().FullName!;

    public override string ItemType => GetType().Name.EndsWith("Item")
        ? GetType().Name.Remove(GetType().Name.Length - 4)
        : GetType().Name;
    
    [ItemProperty]
    public virtual string? WebUrl { get; }
    
    protected WebUrlBuilder UrlBuilder => _webUrlBuilder.Value;
    private WebUrlBuilder CreateWebUrlBuilder()
    {
        if (FullPath.Parts.Length < 2)
        {
            return WebUrlBuilder.Regionless();
        }
        else
        {
            return WebUrlBuilder.ForRegion(FullPath.Parts[1]);
        }
    }
}