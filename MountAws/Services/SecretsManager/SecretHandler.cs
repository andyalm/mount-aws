using System.Text;
using System.Text.Json;
using Amazon.SecretsManager;
using MountAnything;
using MountAnything.Content;

namespace MountAws.Services.SecretsManager;

public class SecretHandler : PathHandler, IContentReaderHandler
{
    private readonly IAmazonSecretsManager _secretsManager;
    private readonly SecretsHandler _parentHandler;

    public SecretHandler(ItemPath path, IPathHandlerContext context, IAmazonSecretsManager secretsManager) : base(path, context)
    {
        _secretsManager = secretsManager;
        _parentHandler = new SecretsHandler(path.Parent, context, secretsManager);
    }

    protected override IItem? GetItemImpl()
    {
        return _parentHandler.GetChildItems()
            .SingleOrDefault(i => i.ItemName.Equals(ItemName, StringComparison.OrdinalIgnoreCase));
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var secretString = GetSecretString();
        if (secretString == null) yield break;

        if (TryParseJsonObject(secretString, out var properties))
        {
            foreach (var property in properties)
            {
                yield return new SecretValueItem(Path, property.Key, property.Value);
            }
        }
    }

    public IStreamContentReader GetContentReader()
    {
        var secretString = GetSecretString();
        if (secretString == null)
        {
            throw new InvalidOperationException("Secret does not contain a string value");
        }
        return new StreamContentReader(new MemoryStream(Encoding.UTF8.GetBytes(secretString)));
    }

    private string? GetSecretString()
    {
        try
        {
            var response = _secretsManager.GetSecretValue(ItemName);
            return response.SecretString;
        }
        catch
        {
            return null;
        }
    }

    private static bool TryParseJsonObject(string value, out Dictionary<string, string> properties)
    {
        properties = new Dictionary<string, string>();
        try
        {
            using var doc = JsonDocument.Parse(value);
            if (doc.RootElement.ValueKind != JsonValueKind.Object) return false;

            foreach (var property in doc.RootElement.EnumerateObject())
            {
                properties[property.Name] = property.Value.ValueKind == JsonValueKind.String
                    ? property.Value.GetString()!
                    : property.Value.GetRawText();
            }
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
