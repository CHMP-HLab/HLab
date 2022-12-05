using System.Linq;

namespace HLab.Base;

public static class XamlTool
{
    public static string Type<T>()
    {
        var fullNamespace = Namespace<T>(out var ns);
        return @$"{Type<T>(ns)} {fullNamespace}";
    }
    public static string Type<T>(out string ns)
    {
        var fullNamespace = Namespace<T>(out ns);
        return @$"{Type<T>(ns)} {fullNamespace}";
    }
    public static string Type<T>(string ns) => @$"{ns}:{typeof(T).Name}";

    public static string Namespace<T>(out string ns)
    {
        var fullNamespace = typeof(T).Namespace;
        ns = fullNamespace.Split('.').Last().ToLower();
        var assemblyName = typeof(T).Assembly.FullName.Split(',')[0];
        return $@"xmlns:{ns}=""clr-namespace:{fullNamespace};assembly={assemblyName}""";
    }

    public const string ContentPlaceHolder = "<!--Content-->";
}