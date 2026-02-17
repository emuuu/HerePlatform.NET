namespace HerePlatform.RestClient.Docs.Models;

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
