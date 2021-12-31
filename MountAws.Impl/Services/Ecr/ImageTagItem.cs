using System.Management.Automation;
using Amazon.ECR.Model;
using MountAnything;
using MountAws.Api;

namespace MountAws.Services.Ecr;

public class ImageTagItem : AwsItem<ImageIdentifier>
{
    private readonly Repository _repository;

    public ImageTagItem(ItemPath parentPath, ImageIdentifier imageIdentifier, Repository repository) : base(parentPath, imageIdentifier)
    {
        _repository = repository;
    }

    public override string ItemName => UnderlyingObject.ImageTag;
    public override string ItemType => EcrItemTypes.ImageTag;
    public string RepositoryUri => $"{_repository.RepositoryUri}:{ItemName}";
    public override bool IsContainer => false;

    public override void CustomizePSObject(PSObject psObject)
    {
        base.CustomizePSObject(psObject);
        psObject.Properties.Add(new PSNoteProperty(nameof(RepositoryUri), RepositoryUri));
    }
}