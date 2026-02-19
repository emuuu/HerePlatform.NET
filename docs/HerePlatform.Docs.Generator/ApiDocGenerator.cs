using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components;

namespace HerePlatform.Docs.Generator;

public class ApiDocGenerator
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task GenerateAsync(string outputPath)
    {
        var assembly = typeof(HerePlatformComponents.DependencyInjectionExtensions).Assembly;
        var xmlPath = FindXmlDocPath(assembly);
        var xmlDocs = xmlPath is not null ? LoadXmlDocs(xmlPath) : new Dictionary<string, string>();

        var types = assembly.GetExportedTypes()
            .Where(t => !t.IsAbstract || t.IsInterface || t.IsSealed)
            .Where(t => !t.Name.StartsWith('<'))
            .Where(t => !IsInternalHelper(t))
            .OrderBy(t => t.Name)
            .ToList();

        var apiDocs = new List<ApiTypeDoc>();

        foreach (var type in types)
        {
            var members = new List<ApiMemberDoc>();

            if (IsBlazorComponent(type))
            {
                members.AddRange(GetComponentParameters(type, xmlDocs));
            }
            else if (type.IsInterface && type.Name.StartsWith('I') && type.Name.EndsWith("Service"))
            {
                members.AddRange(GetServiceMethods(type, xmlDocs));
            }
            else if (type.IsClass && type.IsSealed && type.IsAbstract) // static class
            {
                members.AddRange(GetStaticMethods(type, xmlDocs));
            }
            else
            {
                members.AddRange(GetPublicProperties(type, xmlDocs));
                members.AddRange(GetPublicMethods(type, xmlDocs));
            }

            if (members.Count > 0)
            {
                apiDocs.Add(new ApiTypeDoc
                {
                    TypeName = type.Name,
                    FullName = type.FullName ?? type.Name,
                    Members = members
                });
            }
        }

        var json = JsonSerializer.Serialize(apiDocs, JsonOptions);
        await File.WriteAllTextAsync(outputPath, json);
        Console.WriteLine($"  Generated {apiDocs.Count} type docs -> {Path.GetFileName(outputPath)}");
    }

    private static bool IsBlazorComponent(Type type) =>
        !type.IsAbstract && type.IsAssignableTo(typeof(ComponentBase));

    private static bool IsInternalHelper(Type type) =>
        type.Namespace?.Contains("Serialization") == true ||
        type.Name.Contains("Converter") ||
        type.Name.Contains("Dto") ||
        type.Name == "JsCallableAction" ||
        type.Name == "JsCallableFunc" ||
        type.Name == "ExceptionExtensions";

    private static List<ApiMemberDoc> GetComponentParameters(Type type, Dictionary<string, string> xmlDocs)
    {
        var members = new List<ApiMemberDoc>();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<ParameterAttribute>() is not null ||
                        p.GetCustomAttribute<CascadingParameterAttribute>() is not null);

        foreach (var prop in properties)
        {
            var isEvent = prop.PropertyType.IsGenericType &&
                          prop.PropertyType.GetGenericTypeDefinition() == typeof(EventCallback<>) ||
                          prop.PropertyType == typeof(EventCallback);

            var description = FindXmlDoc(xmlDocs, type, prop.Name, "P");
            members.Add(new ApiMemberDoc
            {
                Name = prop.Name,
                Kind = isEvent ? "Event" : "Property",
                Type = FormatTypeName(prop.PropertyType),
                Default = isEvent ? null : GetDefaultValue(type, prop),
                Description = description
            });
        }

        return members.OrderBy(m => m.Kind == "Event" ? 1 : 0).ThenBy(m => m.Name).ToList();
    }

    private static List<ApiMemberDoc> GetServiceMethods(Type type, Dictionary<string, string> xmlDocs)
    {
        var members = new List<ApiMemberDoc>();
        foreach (var method in type.GetMethods().Where(m => !m.IsSpecialName))
        {
            var description = FindXmlDoc(xmlDocs, type, method.Name, "M");
            var paramStr = string.Join(", ", method.GetParameters().Select(p => $"{FormatTypeName(p.ParameterType)} {p.Name}"));
            members.Add(new ApiMemberDoc
            {
                Name = $"{method.Name}({paramStr})",
                Kind = "Method",
                Type = FormatTypeName(method.ReturnType),
                Description = description
            });
        }
        return members;
    }

    private static List<ApiMemberDoc> GetStaticMethods(Type type, Dictionary<string, string> xmlDocs)
    {
        var members = new List<ApiMemberDoc>();
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => !m.IsSpecialName))
        {
            var description = FindXmlDoc(xmlDocs, type, method.Name, "M");
            var paramStr = string.Join(", ", method.GetParameters().Select(p => $"{FormatTypeName(p.ParameterType)} {p.Name}"));
            members.Add(new ApiMemberDoc
            {
                Name = $"{method.Name}({paramStr})",
                Kind = "Method",
                Type = FormatTypeName(method.ReturnType),
                Description = description
            });
        }
        return members;
    }

    private static List<ApiMemberDoc> GetPublicProperties(Type type, Dictionary<string, string> xmlDocs)
    {
        var members = new List<ApiMemberDoc>();
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            if (prop.GetMethod?.IsPublic != true) continue;
            var description = FindXmlDoc(xmlDocs, type, prop.Name, "P");
            members.Add(new ApiMemberDoc
            {
                Name = prop.Name,
                Kind = "Property",
                Type = FormatTypeName(prop.PropertyType),
                Description = description
            });
        }
        return members;
    }

    private static List<ApiMemberDoc> GetPublicMethods(Type type, Dictionary<string, string> xmlDocs)
    {
        var members = new List<ApiMemberDoc>();
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                     .Where(m => !m.IsSpecialName))
        {
            var description = FindXmlDoc(xmlDocs, type, method.Name, "M");
            var paramStr = string.Join(", ", method.GetParameters().Select(p => $"{FormatTypeName(p.ParameterType)} {p.Name}"));
            members.Add(new ApiMemberDoc
            {
                Name = $"{method.Name}({paramStr})",
                Kind = "Method",
                Type = FormatTypeName(method.ReturnType),
                Description = description
            });
        }
        return members;
    }

    /// <summary>
    /// Walks the type hierarchy (and interfaces) to find XML doc for a member.
    /// Inherited members have their docs on the declaring type, not the concrete type.
    /// </summary>
    private static string FindXmlDoc(Dictionary<string, string> xmlDocs, Type type, string memberName, string prefix)
    {
        // Try the concrete type first
        var key = $"{prefix}:{type.FullName}.{memberName}";
        if (xmlDocs.TryGetValue(key, out var doc))
            return doc;

        // Walk base types
        var baseType = type.BaseType;
        while (baseType is not null && baseType != typeof(object))
        {
            key = $"{prefix}:{baseType.FullName}.{memberName}";
            if (xmlDocs.TryGetValue(key, out doc))
                return doc;
            baseType = baseType.BaseType;
        }

        // Try interfaces
        foreach (var iface in type.GetInterfaces())
        {
            key = $"{prefix}:{iface.FullName}.{memberName}";
            if (xmlDocs.TryGetValue(key, out doc))
                return doc;
        }

        return "";
    }

    private static string FormatTypeName(Type type)
    {
        if (type == typeof(void)) return "void";
        if (type == typeof(string)) return "string";
        if (type == typeof(bool)) return "bool";
        if (type == typeof(int)) return "int";
        if (type == typeof(long)) return "long";
        if (type == typeof(double)) return "double";
        if (type == typeof(float)) return "float";
        if (type == typeof(decimal)) return "decimal";
        if (type == typeof(object)) return "object";

        var nullableUnderlying = Nullable.GetUnderlyingType(type);
        if (nullableUnderlying is not null)
            return $"{FormatTypeName(nullableUnderlying)}?";

        if (type.IsGenericType)
        {
            var name = type.Name[..type.Name.IndexOf('`')];
            var args = string.Join(", ", type.GetGenericArguments().Select(FormatTypeName));

            if (name == "Task" && type.GetGenericArguments().Length == 1)
                return $"Task<{args}>";
            if (name == "EventCallback" && type.GetGenericArguments().Length == 1)
                return $"EventCallback<{args}>";
            if (name == "List")
                return $"List<{args}>";
            if (name == "Dictionary")
                return $"Dictionary<{args}>";

            return $"{name}<{args}>";
        }

        if (type == typeof(Task)) return "Task";
        if (type == typeof(EventCallback)) return "EventCallback";

        return type.Name;
    }

    private static string? GetDefaultValue(Type componentType, PropertyInfo property)
    {
        try
        {
            if (!componentType.IsAbstract && componentType.GetConstructor(Type.EmptyTypes) is not null)
            {
                var instance = Activator.CreateInstance(componentType);
                if (instance is null) return null;
                var value = property.GetValue(instance);

                return value switch
                {
                    null => "null",
                    string s when s == "" => "\"\"",
                    string s => $"\"{s}\"",
                    bool b => b ? "true" : "false",
                    double d => d.ToString("G"),
                    float f => f.ToString("G"),
                    int i => i.ToString(),
                    Enum e => e.ToString(),
                    _ => null
                };
            }
        }
        catch { }
        return null;
    }

    private static string? FindXmlDocPath(Assembly assembly)
    {
        var dllPath = assembly.Location;
        if (string.IsNullOrEmpty(dllPath)) return null;
        var xmlPath = Path.ChangeExtension(dllPath, ".xml");
        return File.Exists(xmlPath) ? xmlPath : null;
    }

    private static Dictionary<string, string> LoadXmlDocs(string xmlPath)
    {
        var docs = new Dictionary<string, string>();
        try
        {
            var doc = XDocument.Load(xmlPath);
            foreach (var member in doc.Descendants("member"))
            {
                var name = member.Attribute("name")?.Value;
                var summary = member.Element("summary")?.Value.Trim();
                if (name is not null && !string.IsNullOrEmpty(summary))
                {
                    // Normalize whitespace
                    summary = string.Join(" ", summary.Split(default(char[]), StringSplitOptions.RemoveEmptyEntries));
                    docs[name] = summary;
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Warning: Could not parse XML docs: {ex.Message}");
        }
        return docs;
    }
}

public class ApiTypeDoc
{
    public string TypeName { get; set; } = "";
    public string FullName { get; set; } = "";
    public List<ApiMemberDoc> Members { get; set; } = [];
}

public class ApiMemberDoc
{
    public string Name { get; set; } = "";
    public string Kind { get; set; } = "";
    public string Type { get; set; } = "";
    public string? Default { get; set; }
    public string Description { get; set; } = "";
}
