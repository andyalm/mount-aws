using MountAnything;

namespace MountAws.Services.S3;

public class BucketItem : Item
{
    private readonly string _bucketName;

    public BucketItem(string parentPath, string bucketName) : base(parentPath)
    {
        _bucketName = bucketName;
        UnderlyingObject = new
        {
            BucketName = _bucketName
        };
    }

    public override string ItemName => _bucketName;
    public override object UnderlyingObject { get; }

    public override string TypeName => "MountAws.Services.S3.BucketItem";
    public override string ItemType => "Bucket";
    public override bool IsContainer => true;
}