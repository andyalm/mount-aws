using System.Management.Automation;
using Amazon.ECR.Model;
using MountAnything;

namespace MountAws.Services.ECR;

public class ImageTagItem : Item
{
    private readonly Repository _repository;
    private readonly ImageIdentifier _imageIdentifier;

    public ImageTagItem(string parentPath, Repository repository, ImageIdentifier imageIdentifier) : base(parentPath)
    {
        _repository = repository;
        _imageIdentifier = imageIdentifier;
    }

    public override string ItemName => _imageIdentifier.ImageTag;
    public override object UnderlyingObject => _imageIdentifier;
    public override string ItemType => "ImageTag";
    public string RepositoryUri => $"{_repository.RepositoryUri}:{_imageIdentifier.ImageTag}";
    public override bool IsContainer => false;

    public override void CustomizePSObject(PSObject psObject)
    {
        base.CustomizePSObject(psObject);
        psObject.Properties.Add(new PSNoteProperty(nameof(RepositoryUri), RepositoryUri));
    }
}