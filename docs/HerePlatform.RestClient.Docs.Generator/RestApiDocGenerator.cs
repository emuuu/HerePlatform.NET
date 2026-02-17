using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using HerePlatform.Core.Services;

namespace HerePlatform.RestClient.Docs.Generator;

public class RestApiDocGenerator
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private Dictionary<string, string> _xmlDocs = new();
    private Dictionary<string, Dictionary<string, string>> _xmlParamDocs = new();
    private readonly HashSet<Type> _collectedEnums = [];
    private readonly HashSet<Type> _collectedResultTypes = [];
    private readonly NullabilityInfoContext _nullabilityCtx = new();

    public async Task GenerateAsync(string outputPath)
    {
        var assembly = typeof(IRoutingService).Assembly;
        var xmlPath = FindXmlDocPath(assembly);
        (_xmlDocs, _xmlParamDocs) = xmlPath is not null
            ? LoadXmlDocs(xmlPath)
            : (new Dictionary<string, string>(), new Dictionary<string, Dictionary<string, string>>());

        var serviceInterfaces = assembly.GetExportedTypes()
            .Where(t => t.IsInterface
                        && t.Namespace == "HerePlatform.Core.Services"
                        && t.Name.StartsWith('I')
                        && t.Name.EndsWith("Service"))
            .OrderBy(t => t.Name)
            .ToList();

        var services = new List<ServiceDoc>();

        foreach (var iface in serviceInterfaces)
        {
            var svc = new ServiceDoc
            {
                InterfaceName = iface.Name,
                Description = GetXmlSummary(iface),
                Methods = []
            };

            foreach (var method in iface.GetMethods().Where(m => !m.IsSpecialName))
            {
                var methodDoc = new MethodDoc
                {
                    Name = method.Name,
                    Description = GetXmlMemberSummary(iface, method.Name, "M"),
                    ReturnType = FormatReturnType(method.ReturnType),
                    Parameters = ExtractParameters(method)
                };
                svc.Methods.Add(methodDoc);
            }

            services.Add(svc);
        }

        var enums = _collectedEnums
            .OrderBy(e => e.Name)
            .Select(BuildEnumDoc)
            .ToList();

        var resultTypes = _collectedResultTypes
            .OrderBy(t => t.Name)
            .Select(BuildResultTypeDoc)
            .ToList();

        var output = new RestApiDocsRoot
        {
            Services = services,
            Enums = enums,
            ResultTypes = resultTypes
        };

        var json = JsonSerializer.Serialize(output, JsonOptions);
        await File.WriteAllTextAsync(outputPath, json);
        Console.WriteLine($"  Generated {services.Count} services, {enums.Count} enums, {resultTypes.Count} result types -> {Path.GetFileName(outputPath)}");
    }

    private List<ParamDocEntry> ExtractParameters(MethodInfo method)
    {
        var result = new List<ParamDocEntry>();
        var methodParams = method.GetParameters();

        foreach (var param in methodParams)
        {
            var paramType = param.ParameterType;
            var isNullable = IsNullableParameter(param);

            // Prefer <param> tag description, fall back to type summary
            var paramDescription = GetXmlParamSummary(method.DeclaringType!, method.Name, param.Name!);

            if (IsCoreModelType(paramType))
            {
                if (string.IsNullOrEmpty(paramDescription))
                    paramDescription = GetXmlSummary(paramType);

                var entry = new ParamDocEntry
                {
                    Name = param.Name!,
                    Type = FormatTypeName(paramType) + (isNullable ? "?" : ""),
                    Required = !param.HasDefaultValue && !isNullable,
                    Description = paramDescription,
                    Properties = ExtractProperties(paramType)
                };
                result.Add(entry);
            }
            else
            {
                var entry = new ParamDocEntry
                {
                    Name = param.Name!,
                    Type = FormatTypeName(paramType) + (isNullable && !paramType.Name.Contains("Nullable") ? "?" : ""),
                    Required = !param.HasDefaultValue && !isNullable,
                    Description = paramDescription,
                    Default = param.HasDefaultValue ? FormatDefaultValue(param.DefaultValue) : null
                };

                CollectEnumsFromType(paramType);
                result.Add(entry);
            }
        }

        // Collect return type for result types
        var returnType = UnwrapTaskType(method.ReturnType);
        if (returnType is not null && IsCoreModelType(returnType))
        {
            CollectResultType(returnType);
        }

        return result;
    }

    private List<ParamDocEntry> ExtractProperties(Type type, int depth = 0)
    {
        if (depth > 3) return [];

        var result = new List<ParamDocEntry>();

        try
        {
            var instance = type.IsValueType ? Activator.CreateInstance(type)
                : type.GetConstructor(Type.EmptyTypes) is not null ? Activator.CreateInstance(type)
                : null;

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetMethod?.IsPublic == true);

            foreach (var prop in properties)
            {
                var propType = prop.PropertyType;
                var isNullable = IsNullableProperty(prop);
                var defaultValue = GetPropertyDefault(instance, prop);

                CollectEnumsFromType(propType);

                var entry = new ParamDocEntry
                {
                    Name = prop.Name,
                    Type = FormatPropertyTypeName(prop),
                    Required = !isNullable && defaultValue is null or "null",
                    Description = GetXmlMemberSummary(type, prop.Name, "P"),
                    Default = defaultValue
                };

                // Recurse into nested Core model types
                var unwrapped = UnwrapNullable(propType);
                if (IsCoreModelType(unwrapped) && !unwrapped.IsEnum)
                {
                    entry.Properties = ExtractProperties(unwrapped, depth + 1);
                }

                result.Add(entry);
            }
        }
        catch
        {
            // If we can't instantiate, still extract properties without defaults
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetMethod?.IsPublic == true);

            foreach (var prop in properties)
            {
                CollectEnumsFromType(prop.PropertyType);

                result.Add(new ParamDocEntry
                {
                    Name = prop.Name,
                    Type = FormatPropertyTypeName(prop),
                    Required = !IsNullableProperty(prop),
                    Description = GetXmlMemberSummary(type, prop.Name, "P")
                });
            }
        }

        return result;
    }

    private void CollectResultType(Type type)
    {
        if (!_collectedResultTypes.Add(type)) return;

        // Recursively collect nested types from result properties
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetMethod?.IsPublic == true);

        foreach (var prop in properties)
        {
            CollectEnumsFromType(prop.PropertyType);

            var unwrapped = UnwrapCollectionType(UnwrapNullable(prop.PropertyType));
            if (IsCoreModelType(unwrapped) && !unwrapped.IsEnum)
            {
                CollectResultType(unwrapped);
            }
        }
    }

    private ResultTypeDoc BuildResultTypeDoc(Type type)
    {
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetMethod?.IsPublic == true)
            .Select(p =>
            {
                var unwrapped = UnwrapCollectionType(UnwrapNullable(p.PropertyType));
                string? nestedType = IsCoreModelType(unwrapped) && !unwrapped.IsEnum
                    ? unwrapped.Name
                    : null;

                return new ResultPropertyDoc
                {
                    Name = p.Name,
                    Type = FormatPropertyTypeName(p),
                    Description = GetXmlMemberSummary(type, p.Name, "P"),
                    NestedType = nestedType
                };
            })
            .ToList();

        return new ResultTypeDoc
        {
            Name = type.Name,
            Description = GetXmlSummary(type),
            Properties = props
        };
    }

    private EnumDoc BuildEnumDoc(Type enumType)
    {
        var values = new List<EnumValueDoc>();
        foreach (var name in Enum.GetNames(enumType))
        {
            var field = enumType.GetField(name)!;
            var enumMember = field.GetCustomAttribute<EnumMemberAttribute>();
            values.Add(new EnumValueDoc
            {
                Name = name,
                SerializedValue = enumMember?.Value ?? name
            });
        }

        return new EnumDoc
        {
            Name = enumType.Name,
            Description = GetXmlSummary(enumType),
            IsFlags = enumType.GetCustomAttribute<FlagsAttribute>() is not null,
            Values = values
        };
    }

    // ── XML doc helpers ──

    private string GetXmlSummary(Type type)
    {
        var key = $"T:{type.FullName}";
        return _xmlDocs.GetValueOrDefault(key, "");
    }

    private string GetXmlMemberSummary(Type type, string memberName, string prefix)
    {
        var key = $"{prefix}:{type.FullName}.{memberName}";
        if (_xmlDocs.TryGetValue(key, out var doc))
            return doc;

        // For methods with parameters, try matching by method name only (simplified)
        if (prefix == "M")
        {
            foreach (var kvp in _xmlDocs)
            {
                if (kvp.Key.StartsWith($"M:{type.FullName}.{memberName}"))
                    return kvp.Value;
            }
        }

        return "";
    }

    private string GetXmlParamSummary(Type type, string methodName, string paramName)
    {
        // Look for <param name="x"> inside the method's XML doc
        // Try exact match first
        var key = $"M:{type.FullName}.{methodName}";
        if (_xmlParamDocs.TryGetValue(key, out var paramDocs) && paramDocs.TryGetValue(paramName, out var desc))
            return desc;

        // Try matching by method name prefix (for methods with parameter signatures in the key)
        foreach (var kvp in _xmlParamDocs)
        {
            if (kvp.Key.StartsWith($"M:{type.FullName}.{methodName}") && kvp.Value.TryGetValue(paramName, out desc))
                return desc;
        }

        return "";
    }

    // ── Type formatting ──

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
        if (type == typeof(DateTime)) return "DateTime";

        var nullableUnderlying = Nullable.GetUnderlyingType(type);
        if (nullableUnderlying is not null)
            return $"{FormatTypeName(nullableUnderlying)}?";

        if (type.IsGenericType)
        {
            var name = type.Name[..type.Name.IndexOf('`')];
            var args = string.Join(", ", type.GetGenericArguments().Select(FormatTypeName));
            return $"{name}<{args}>";
        }

        if (type == typeof(Task)) return "Task";

        return type.Name;
    }

    private string FormatPropertyTypeName(PropertyInfo prop)
    {
        var typeName = FormatTypeName(prop.PropertyType);
        if (!typeName.EndsWith('?') && IsNullableProperty(prop))
            typeName += "?";
        return typeName;
    }

    private static string FormatReturnType(Type type)
    {
        var inner = UnwrapTaskType(type);
        return inner is not null ? inner.Name : FormatTypeName(type);
    }

    private static string? FormatDefaultValue(object? value)
    {
        return value switch
        {
            null => "null",
            string s => $"\"{s}\"",
            bool b => b ? "true" : "false",
            int i => i.ToString(),
            double d => d.ToString("G"),
            Enum e => e.ToString(),
            _ => value.ToString()
        };
    }

    private string? GetPropertyDefault(object? instance, PropertyInfo prop)
    {
        if (instance is null) return null;
        try
        {
            var value = prop.GetValue(instance);
            var propType = UnwrapNullable(prop.PropertyType);

            // For value types, check if it's the default
            if (value is null)
                return IsNullableProperty(prop) ? "null" : null;

            return value switch
            {
                string s => $"\"{s}\"",
                bool b => b ? "true" : "false",
                int i => i.ToString(),
                double d => d.ToString("G"),
                Enum e when propType.GetCustomAttribute<FlagsAttribute>() is not null
                    => Convert.ToInt64(e) == 0 ? "None" : e.ToString(),
                Enum e => e.ToString(),
                _ => null
            };
        }
        catch
        {
            return null;
        }
    }

    // ── Nullability helpers ──

    private bool IsNullableProperty(PropertyInfo prop)
    {
        if (Nullable.GetUnderlyingType(prop.PropertyType) is not null)
            return true;

        try
        {
            var info = _nullabilityCtx.Create(prop);
            return info.ReadState == NullabilityState.Nullable;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsNullableParameter(ParameterInfo param)
    {
        if (Nullable.GetUnderlyingType(param.ParameterType) is not null)
            return true;

        if (param.HasDefaultValue && param.DefaultValue is null)
            return true;

        return false;
    }

    // ── Type analysis helpers ──

    private static bool IsCoreModelType(Type type)
    {
        var unwrapped = UnwrapNullable(type);
        return unwrapped.Namespace?.StartsWith("HerePlatform.Core") == true
               && unwrapped.Assembly == typeof(IRoutingService).Assembly;
    }

    private static Type? UnwrapTaskType(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
            return type.GetGenericArguments()[0];
        return null;
    }

    private static Type UnwrapNullable(Type type)
    {
        return Nullable.GetUnderlyingType(type) ?? type;
    }

    private static Type UnwrapCollectionType(Type type)
    {
        if (type.IsGenericType)
        {
            var genDef = type.GetGenericTypeDefinition();
            if (genDef == typeof(List<>) || genDef == typeof(IList<>) ||
                genDef == typeof(IEnumerable<>) || genDef == typeof(ICollection<>) ||
                genDef == typeof(IReadOnlyList<>))
            {
                return type.GetGenericArguments()[0];
            }
        }
        return type;
    }

    private void CollectEnumsFromType(Type type)
    {
        var unwrapped = UnwrapNullable(type);
        if (unwrapped.IsEnum && IsCoreModelType(unwrapped))
        {
            _collectedEnums.Add(unwrapped);
        }

        // Check generic arguments (e.g., List<TransportMode>)
        if (type.IsGenericType)
        {
            foreach (var arg in type.GetGenericArguments())
                CollectEnumsFromType(arg);
        }
    }

    // ── XML doc loading ──

    private static string? FindXmlDocPath(Assembly assembly)
    {
        var dllPath = assembly.Location;
        if (string.IsNullOrEmpty(dllPath)) return null;
        var xmlPath = Path.ChangeExtension(dllPath, ".xml");
        return File.Exists(xmlPath) ? xmlPath : null;
    }

    private static (Dictionary<string, string> summaries, Dictionary<string, Dictionary<string, string>> paramDocs) LoadXmlDocs(string xmlPath)
    {
        var summaries = new Dictionary<string, string>();
        var paramDocs = new Dictionary<string, Dictionary<string, string>>();
        try
        {
            var doc = XDocument.Load(xmlPath);
            foreach (var member in doc.Descendants("member"))
            {
                var name = member.Attribute("name")?.Value;
                if (name is null) continue;

                var summary = member.Element("summary")?.Value.Trim();
                if (!string.IsNullOrEmpty(summary))
                {
                    summary = string.Join(" ", summary.Split(default(char[]), StringSplitOptions.RemoveEmptyEntries));
                    summaries[name] = summary;
                }

                foreach (var paramEl in member.Elements("param"))
                {
                    var paramName = paramEl.Attribute("name")?.Value;
                    var paramDesc = paramEl.Value.Trim();
                    if (paramName is not null && !string.IsNullOrEmpty(paramDesc))
                    {
                        paramDesc = string.Join(" ", paramDesc.Split(default(char[]), StringSplitOptions.RemoveEmptyEntries));
                        if (!paramDocs.ContainsKey(name))
                            paramDocs[name] = new Dictionary<string, string>();
                        paramDocs[name][paramName] = paramDesc;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Warning: Could not parse XML docs: {ex.Message}");
        }
        return (summaries, paramDocs);
    }
}

// ── Output models ──

public class RestApiDocsRoot
{
    public List<ServiceDoc> Services { get; set; } = [];
    public List<EnumDoc> Enums { get; set; } = [];
    public List<ResultTypeDoc> ResultTypes { get; set; } = [];
}

public class ServiceDoc
{
    public string InterfaceName { get; set; } = "";
    public string Description { get; set; } = "";
    public List<MethodDoc> Methods { get; set; } = [];
}

public class MethodDoc
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string ReturnType { get; set; } = "";
    public List<ParamDocEntry> Parameters { get; set; } = [];
}

public class ParamDocEntry
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public bool Required { get; set; }
    public string Description { get; set; } = "";
    public string? Default { get; set; }
    public List<ParamDocEntry>? Properties { get; set; }
}

public class EnumDoc
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public bool IsFlags { get; set; }
    public List<EnumValueDoc> Values { get; set; } = [];
}

public class EnumValueDoc
{
    public string Name { get; set; } = "";
    public string SerializedValue { get; set; } = "";
}

public class ResultTypeDoc
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public List<ResultPropertyDoc> Properties { get; set; } = [];
}

public class ResultPropertyDoc
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public string Description { get; set; } = "";
    public string? NestedType { get; set; }
}
