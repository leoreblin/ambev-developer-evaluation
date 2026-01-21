using System.Reflection;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

public static class PropertyHelper<T>
{
    public static bool IsValidProperty(string propertyName, bool throwExceptionIfNotFound = false)
    {
        var prop = typeof(T).GetProperty(propertyName,
            BindingFlags.IgnoreCase |
            BindingFlags.Public |
            BindingFlags.Instance);

        if (prop == null && throwExceptionIfNotFound)
            throw new ArgumentException($"Property '{propertyName}' not found in type '{typeof(T).Name}'");

        return prop != null;
    }

    public static IEnumerable<string> GetValidProperties()
    {
        return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .OrderBy(name => name);
    }
}
