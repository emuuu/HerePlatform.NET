using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace HerePlatform.Docs.Generator;

/// <summary>
/// Renders an XML documentation element (summary, param) as plain text. <see cref="XElement.Value"/>
/// drops self-closing tags such as <c>&lt;see cref="..."/&gt;</c>, which leaves gaps like
/// "The provides" in the generated docs.
/// </summary>
internal static class XmlDocText
{
    public static string? Render(XElement? element)
    {
        if (element is null) return null;

        var sb = new StringBuilder();
        Append(element, sb);

        // Normalize whitespace
        return string.Join(" ", sb.ToString().Split(default(char[]), StringSplitOptions.RemoveEmptyEntries));
    }

    private static void Append(XElement element, StringBuilder sb)
    {
        foreach (var node in element.Nodes())
        {
            if (node is XText text)
            {
                sb.Append(text.Value);
            }
            else if (node is XElement child)
            {
                if (child.Name == "para") sb.Append(' ');

                if (child.Nodes().Any())
                    Append(child, sb);
                else
                    sb.Append(RenderEmpty(child));

                if (child.Name == "para") sb.Append(' ');
            }
        }
    }

    // <see cref/langword/href/>, <paramref name/>, <typeparamref name/>
    private static string RenderEmpty(XElement element)
    {
        if (element.Attribute("cref")?.Value is { } cref) return ShortCref(cref);
        if (element.Attribute("langword")?.Value is { } langword) return langword;
        if (element.Attribute("name")?.Value is { } name) return name;
        if (element.Attribute("href")?.Value is { } href) return href;
        return "";
    }

    // "T:Ns.HereAutosuggest" -> "HereAutosuggest", "P:Ns.HereAutosuggest.InputTemplate" ->
    // "HereAutosuggest.InputTemplate", "M:Ns.Type.#ctor(System.String)" -> "Type"
    private static string ShortCref(string cref)
    {
        var kind = cref.Length > 2 && cref[1] == ':' ? cref[0] : 'T';
        var path = cref.Length > 2 && cref[1] == ':' ? cref[2..] : cref;

        var paren = path.IndexOf('(');
        if (paren >= 0) path = path[..paren];
        path = Regex.Replace(path, @"`+\d+", "");

        var isConstructor = path.EndsWith(".#ctor", StringComparison.Ordinal);
        if (isConstructor) path = path[..^".#ctor".Length];

        var parts = path.Split('.');
        if (kind is 'T' or 'N' || isConstructor || parts.Length < 2)
            return parts[^1];
        return $"{parts[^2]}.{parts[^1]}";
    }
}
