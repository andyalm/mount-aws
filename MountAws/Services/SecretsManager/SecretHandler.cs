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
        var fullPath = _secretPath.Value;

        // 1. Try as an actual secret
        var secret = _secretsManager.DescribeSecretOrDefault(fullPath);
        if (secret != null)
        {
            return new SecretItem(ParentPath, secret);
        }

        // 2. Check if the last segment is a JSON key of a parent secret
        var itemPath = new ItemPath(fullPath);
        if (!itemPath.Parent.IsRoot)
        {
            var parentSecret = _secretsManager.DescribeSecretOrDefault(itemPath.Parent.FullName);
            if (parentSecret != null)
            {
                var secretString = GetSecretString(itemPath.Parent.FullName);
                if (secretString != null && TryParseJsonObject(secretString, out var props) &&
                    props.TryGetValue(itemPath.Name, out var value))
                {
                    return new SecretValueItem(ParentPath, itemPath.Name, value);
                }
            }
        }

        // 3. Must be a folder
        return new SecretFolderItem(ParentPath, itemPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return GetItem() switch
        {
            SecretItem => GetSecretChildren(),
            SecretFolderItem => GetFolderChildren(),
            _ => Enumerable.Empty<IItem>()
        };
    }

    private IEnumerable<IItem> GetSecretChildren()
    {
        var secretString = GetSecretString(_secretPath.Value);
        if (secretString == null) yield break;

        if (TryParseJsonObject(secretString, out var properties))
        {
            foreach (var property in properties)
            {
                yield return new SecretValueItem(Path, property.Key, property.Value);
            }
        }
    }

    private IEnumerable<IItem> GetFolderChildren()
    {
        return _navigator.ListChildItems(Path, new ItemPath(_secretPath.Value));
    }

    public IStreamContentReader GetContentReader()
    {
        var item = GetItem();
        return item switch
        {
            SecretItem => GetSecretContentReader(),
            SecretValueItem valueItem => new StreamContentReader(
                new MemoryStream(Encoding.UTF8.GetBytes(valueItem.Value))),
            _ => throw new InvalidOperationException("Cannot read content from a folder")
        };
    }

    private IStreamContentReader GetSecretContentReader()
    {
        var secretString = GetSecretString(_secretPath.Value);
        if (secretString == null)
        {
            throw new InvalidOperationException("Secret does not contain a string value");
        }
        return new StreamContentReader(new MemoryStream(Encoding.UTF8.GetBytes(secretString)));
    }

    public IStreamContentWriter GetContentWriter()
    {
        var item = GetItem();
        return item switch
        {
            SecretItem => GetSecretContentWriter(),
            SecretValueItem => GetSecretValueContentWriter(),
            _ => throw new InvalidOperationException("Cannot write content to a folder")
        };
    }

    private IStreamContentWriter GetSecretContentWriter()
    {
        return new StreamContentWriter(stream =>
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var secretString = reader.ReadToEnd();
            _secretsManager.PutSecretValue(_secretPath.Value, secretString);
        });
    }

    private IStreamContentWriter GetSecretValueContentWriter()
    {
        var itemPath = new ItemPath(_secretPath.Value);
        var secretName = itemPath.Parent.FullName;
        var keyName = itemPath.Name;

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

            jsonObject[keyName] = JsonValue.Create(newValue);
            _secretsManager.PutSecretValue(secretName, jsonObject.ToJsonString());
        });
    }

    public override IEnumerable<IItemProperty> GetItemProperties(HashSet<string> propertyNames, Func<ItemPath, string> pathResolver)
    {
        if (GetItem() is not SecretItem) return base.GetItemProperties(propertyNames, pathResolver);

        var secretString = GetSecretString(_secretPath.Value);
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
        if (GetItem() is not SecretItem)
        {
            throw new InvalidOperationException("Can only set properties on a secret");
        }

        var secretString = GetSecretString(_secretPath.Value)
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

        _secretsManager.PutSecretValue(_secretPath.Value, jsonObject.ToJsonString());
    }

    private string? GetSecretString(string secretName)
    {
        try
        {
            var response = _secretsManager.GetSecretValue(secretName);
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
