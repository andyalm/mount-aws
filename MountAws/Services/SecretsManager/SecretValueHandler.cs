using System.Text;
using Amazon.SecretsManager;
using MountAnything;
using MountAnything.Content;

namespace MountAws.Services.SecretsManager;

public class SecretValueHandler : PathHandler, IContentReaderHandler
{
    private readonly SecretHandler _parentHandler;

    public SecretValueHandler(ItemPath path, IPathHandlerContext context, IAmazonSecretsManager secretsManager) : base(path, context)
    {
        _parentHandler = new SecretHandler(path.Parent, context, secretsManager);
    }

    protected override IItem? GetItemImpl()
    {
        return _parentHandler.GetChildItems()
            .SingleOrDefault(i => i.ItemName.Equals(ItemName, StringComparison.OrdinalIgnoreCase));
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }

    public IStreamContentReader GetContentReader()
    {
        var item = GetItem() as SecretValueItem;
        if (item == null)
        {
            throw new InvalidOperationException("Secret value does not exist");
        }
        return new StreamContentReader(new MemoryStream(Encoding.UTF8.GetBytes(item.Value)));
    }
}
