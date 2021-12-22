using System.Management.Automation;
using MountAws.Api;

namespace MountAws.Services.ECR;

public class ImageTagItem : AwsItem
{
    private readonly PSObject _repository;

    public ImageTagItem(string parentPath, PSObject repository, PSObject imageIdentifier) : base(parentPath, imageIdentifier)
    {
        _repository = repository;
    }

    public override string ItemName => Property<string>("ImageTag")!;
    public override string ItemType => EcrItemTypes.ImageTag;
    public string RepositoryUri => $"{_repository.Property<string>("RepositoryUri")}:{ItemName}";
    public override bool IsContainer => false;

    public override void CustomizePSObject(PSObject psObject)
    {
        base.CustomizePSObject(psObject);
        psObject.Properties.Add(new PSNoteProperty(nameof(RepositoryUri), RepositoryUri));
    }
}