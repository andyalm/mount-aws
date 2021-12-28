using System.Collections;
using System.Management.Automation;

namespace MountAnything;

public static class PSObjectExtensions
{
    public static PSObject ToPSObject(this object obj)
    {
        return new PSObject(obj);
    }

    public static IEnumerable<PSObject> ToPSObjects<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.Select(o => o!.ToPSObject());
    }

    public static T? Property<T>(this PSObject psObject, string propertyName)
    {
        var rawValue = psObject.Properties[propertyName]?.Value;
        if (rawValue == default)
        {
            return default;
        }

        if (rawValue is T typedValue)
        {
            return typedValue;
        }

        if (typeof(T) == typeof(PSObject))
        {
            return (T)(object)rawValue.ToPSObject();
        }

        if (typeof(IEnumerable<PSObject>).IsAssignableFrom(typeof(T)) && rawValue is IEnumerable enumerable)
        {
            return (T)enumerable.Cast<object>().ToPSObjects();
        }

        return (T)Convert.ChangeType(rawValue, typeof(T));
    }
}