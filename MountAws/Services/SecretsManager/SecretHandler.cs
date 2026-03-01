using System.Management.Automation;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Amazon.SecretsManager;
using MountAnything;
using MountAnything.Content;

namespace MountAws.Services.SecretsManager;

public class SecretHandler(
    ItemPath path,
    IPathHandlerContext context,
    IAmazonSecretsManager secretsManager,
    SecretNavigator navigator,
    SecretPath secretPath)
    : PathHandler(path, context), IContentReaderHandler, IContentWriterHandler, ISetItemPropertiesHandler
{
    protected override IItem? GetItemImpl()
    {
        var childSecrets = secretsManager
            .ListSecrets(secretPath.Path.FullName)
            .ToArray();

        if (childSecrets.Length == 1 && childSecrets[0].Name == secretPath.Path.FullName)
        {
            return new SecretItem(ParentPath, childSecrets[0]);
        }
        if (childSecrets.Any())
        {
            return new SecretFolderItem(ParentPath, secretPath.Path);
        }

        return null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return GetItem() switch
        {
            SecretFolderItem => navigator.ListChildItems(Path, secretPath.Path),
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
            secretsManager.PutSecretValue(secretPath.Path.FullName, secretString);
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

        secretsManager.PutSecretValue(secretPath.Path.FullName, jsonObject.ToJsonString());
    }

    private string? GetSecretString()
    {
        try
        {
            var response = secretsManager.GetSecretValue(secretPath.Path.FullName);
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
