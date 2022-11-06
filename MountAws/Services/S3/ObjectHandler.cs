using System.Management.Automation.Provider;
using Amazon.S3;
using MountAnything;
using MountAnything.Content;

namespace MountAws.Services.S3;

public class ObjectHandler : PathHandler, IContentReaderHandler,
    IContentWriterHandler,
    INewItemHandler,
    IRemoveItemHandler
{
    private readonly ObjectPath _objectPath;
    private readonly CurrentBucket _currentBucket;
    private readonly IAmazonS3 _s3;
    
    public ObjectHandler(ItemPath path, IPathHandlerContext context, ObjectPath objectPath, CurrentBucket currentBucket, IAmazonS3 s3) : base(path, context)
    {
        _objectPath = objectPath;
        _currentBucket = currentBucket;
        _s3 = s3;
    }

    protected override IItem? GetItemImpl()
    {
        try
        {
            WriteDebug($"s3.GetObject({_currentBucket.Name}, {_objectPath.Value})");
            var response = _s3.GetObject(_currentBucket.Name, _objectPath.Value);

            return new ObjectItem(ParentPath, response);
        }
        catch (ObjectNotFoundException)
        {
            var children = _s3.ListChildItems(_currentBucket.Name, ParentPath, $"{_objectPath.Value}/", maxResults: 1);
            if (children.Any())
            {
                return new ObjectItem(ParentPath, _objectPath.Value, _currentBucket.Name);
            }
        }

        return null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _s3.ListChildItems(_currentBucket.Name, Path, $"{_objectPath.Value}/");
    }

    public Stream GetContent()
    {
        return _s3.GetObjectStream(_currentBucket.Name, _objectPath.Value);
    }

    public Stream GetWriterStream()
    {
        return new MemoryStream();
    }

    public void WriterFinished(Stream stream)
    {
        _s3.PutObject(_currentBucket.Name, _objectPath.Value, stream);
    }

    public void NewItem(string? itemTypeName, object? newItemValue)
    {
        _s3.PutObject(_currentBucket.Name, _objectPath.Value, newItemValue?.ToString());
    }

    public void RemoveItem()
    {
        _s3.DeleteObject(_currentBucket.Name, _objectPath.Value);
    }
}