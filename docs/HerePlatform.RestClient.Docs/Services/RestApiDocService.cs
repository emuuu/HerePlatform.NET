using System.Net.Http.Json;
using HerePlatform.RestClient.Docs.Models;

namespace HerePlatform.RestClient.Docs.Services;

public interface IRestApiDocService
{
    Task InitializeAsync();
    ServiceDoc? GetService(string interfaceName);
    MethodDoc? GetMethod(string interfaceName, string methodName);
    List<ParamDoc> GetMethodParams(string interfaceName, string methodName);
}

public class RestApiDocService : IRestApiDocService
{
    private readonly HttpClient _http;
    private RestApiDocsRoot? _docs;

    public RestApiDocService(HttpClient http)
    {
        _http = http;
    }

    public async Task InitializeAsync()
    {
        if (_docs is not null) return;
        _docs = await _http.GetFromJsonAsync<RestApiDocsRoot>("data/rest-api-docs.json");
    }

    public ServiceDoc? GetService(string interfaceName) =>
        _docs?.Services.FirstOrDefault(s => s.InterfaceName == interfaceName);

    public MethodDoc? GetMethod(string interfaceName, string methodName) =>
        GetService(interfaceName)?.Methods.FirstOrDefault(m => m.Name == methodName);

    public List<ParamDoc> GetMethodParams(string interfaceName, string methodName)
    {
        var method = GetMethod(interfaceName, methodName);
        if (method is null) return [];

        var result = new List<ParamDoc>();
        var parameters = method.Parameters;

        // If a single parameter is a complex type (has properties), flatten without prefix
        var singleComplex = parameters.Count == 1 && parameters[0].Properties is { Count: > 0 };

        foreach (var param in parameters)
        {
            if (param.Properties is { Count: > 0 })
            {
                var prefix = singleComplex ? "" : $"{param.Name}.";
                FlattenProperties(param.Properties, prefix, result);
            }
            else
            {
                result.Add(new ParamDoc(
                    param.Name,
                    param.Type,
                    param.Required,
                    param.Description,
                    param.Default));
            }
        }

        return result;
    }

    private static void FlattenProperties(List<ParamDocEntry> properties, string prefix, List<ParamDoc> result)
    {
        foreach (var prop in properties)
        {
            result.Add(new ParamDoc(
                $"{prefix}{prop.Name}",
                prop.Type,
                prop.Required,
                prop.Description,
                prop.Default));
        }
    }
}
