using Amazon.Lambda.Model;
using MountAnything;

namespace MountAws.Services.Lambda;

public class AliasItem : AwsItem<AliasConfiguration>
{
    private readonly string _functionName;

    public AliasItem(ItemPath parentPath, AliasConfiguration alias, string functionName)
        : base(parentPath, alias)
    {
        _functionName = functionName;
    }

    public override string ItemName => UnderlyingObject.Name;
    public override string ItemType => LambdaItemTypes.Alias;
    public override bool IsContainer => false;
    public override string? WebUrl =>
        UrlBuilder.CombineWith($"lambda/home#/functions/{_functionName}/aliases/{ItemName}");
}
