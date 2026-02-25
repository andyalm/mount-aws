using Amazon.Lambda.Model;
using MountAnything;

namespace MountAws.Services.Lambda;

public class FunctionItem : AwsItem<FunctionConfiguration>
{
    public FunctionItem(ItemPath parentPath, FunctionConfiguration function)
        : base(parentPath, function) { }

    public override string ItemName => UnderlyingObject.FunctionName;
    public override string ItemType => LambdaItemTypes.Function;
    public override bool IsContainer => true;
    public override string? WebUrl => UrlBuilder.CombineWith($"lambda/home#/functions/{ItemName}");
}
