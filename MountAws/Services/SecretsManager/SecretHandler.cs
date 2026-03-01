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
    private readonly SecretNavigator _navigator;
    private readonly SecretPath _secretPath;

    public SecretHandler(ItemPath path, IPathHandlerContext context, IAmazonSecretsManager secretsManager,
        SecretNavigator navigator, SecretPath secretPath) : base(path, context)
    {
        _secretsManager = secretsManager;
        _navigator = navigator;
        _secretPath = secretPath;
    }

    protected override IItem? GetItemImpl()
    {
        // 1. Try as an actual secret
        var secret = _secretsManager.DescribeSecretOrDefault(_secretPath.Path.FullName);
        if (secret != null)
        {
            return new SecretItem(ParentPath, secret);
        }

        // 2. Check if it's a folder (prefix of other secrets)
        var childSecrets = _secretsManager.ListSecrets(_secretPath.Path.FullName);
        if (childSecrets.Any())
        {
            return new SecretFolderItem(ParentPath, _secretPath.Path);
        }

        return null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return GetItem() switch
        {
            SecretFolderItem => _navigator.ListChildItems(Path, _secretPath.Path),
            _ => Enumerable.Empty<IItem>()
        };
    }

    public IStreamContentReader GetContentReader()
    {
        var secretString = GetSecretString()
            ?? throw new InvalidOperationException("Secret does not contain a string value");
        return new StreamContentReader(new MemoryStream(Encoding.UTF8.GetBytes(secretString)));
    }

    public IStreamContentWriter GetContentWriter()
    {
        return new StreamContentWriter(stream =>
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var secretString = reader.ReadToEnd();
            _secretsManager.PutSecretValue(_secretPath.Path.FullName, secretString);
        });
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

        _secretsManager.PutSecretValue(_secretPath.Path.FullName, jsonObject.ToJsonString());
    }

    private string? GetSecretString()
    {
        try
        {
            var response = _secretsManager.GetSecretValue(_secretPath.Path.FullName);
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
