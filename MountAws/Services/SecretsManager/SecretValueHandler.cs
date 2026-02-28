using System.Text;
using System.Text.Json.Nodes;
using Amazon.SecretsManager;
using MountAnything;
using MountAnything.Content;

namespace MountAws.Services.SecretsManager;

public class SecretValueHandler : PathHandler, IContentReaderHandler, IContentWriterHandler
{
    private readonly IAmazonSecretsManager _secretsManager;
    private readonly SecretHandler _parentHandler;

    public SecretValueHandler(ItemPath path, IPathHandlerContext context, IAmazonSecretsManager secretsManager) : base(path, context)
    {
        _secretsManager = secretsManager;
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

    public IStreamContentWriter GetContentWriter()
    {
        var secretName = Path.Parent.Name;
        return new StreamContentWriter(stream =>
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var newValue = reader.ReadToEnd();

            var response = _secretsManager.GetSecretValue(secretName);
            var jsonNode = JsonNode.Parse(response.SecretString)
                ?? throw new InvalidOperationException("Secret value is not valid JSON");
            if (jsonNode is not JsonObject jsonObject)
            {
                throw new InvalidOperationException("Secret value is not a JSON object");
            }

            jsonObject[ItemName] = JsonValue.Create(newValue);
            _secretsManager.PutSecretValue(secretName, jsonObject.ToJsonString());
        });
    }
}
