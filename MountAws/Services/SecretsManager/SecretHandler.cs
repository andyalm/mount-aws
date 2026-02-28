using System.Management.Automation;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Amazon.SecretsManager;
using MountAnything;
using MountAnything.Content;

namespace MountAws.Services.SecretsManager;

public class SecretHandler : PathHandler, IContentReaderHandler, IContentWriterHandler, ISetItemPropertiesHandler
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

    public override IEnumerable<IItemProperty> GetItemProperties(HashSet<string> propertyNames, Func<ItemPath, string> pathResolver)
    {
        var secretString = GetSecretString();
        if (secretString == null || !TryParseJsonObject(secretString, out var properties))
        {
            return base.GetItemProperties(propertyNames, pathResolver);
        }

        var psObject = new PSObject();
        foreach (var property in properties)
        {
            psObject.Properties.Add(new PSNoteProperty(property.Key, property.Value));
        }
        return psObject.AsItemProperties().WherePropertiesMatch(propertyNames);
    }

    public IStreamContentWriter GetContentWriter()
    {
        return new StreamContentWriter(stream =>
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var secretString = reader.ReadToEnd();
            _secretsManager.PutSecretValue(ItemName, secretString);
        });
    }

    public void SetItemProperties(ICollection<IItemProperty> propertyValues)
    {
        var secretString = GetSecretString()
            ?? throw new InvalidOperationException("Secret does not contain a string value");

        var jsonNode = JsonNode.Parse(secretString)
            ?? throw new InvalidOperationException("Secret value is not valid JSON");

        if (jsonNode is not JsonObject jsonObject)
        {
            throw new InvalidOperationException("Secret value is not a JSON object");
        }

        foreach (var property in propertyValues)
        {
            jsonObject[property.Name] = JsonValue.Create(property.Value?.ToString());
        }

        _secretsManager.PutSecretValue(ItemName, jsonObject.ToJsonString());
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
