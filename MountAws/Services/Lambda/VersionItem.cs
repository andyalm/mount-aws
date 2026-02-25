using Amazon.Lambda.Model;
using MountAnything;

namespace MountAws.Services.Lambda;

public class VersionItem : AwsItem<FunctionConfiguration>
{
    public VersionItem(ItemPath parentPath, FunctionConfiguration versionConfig)
        : base(parentPath, versionConfig) { }

    public override string ItemName => UnderlyingObject.Version;
    public override string ItemType => LambdaItemTypes.Version;
    public override bool IsContainer => false;
    public override string? WebUrl =>
        UrlBuilder.CombineWith(
            $"lambda/home#/functions/{UnderlyingObject.FunctionName}/versions/{ItemName}");
}
