using System.Reflection;
using System.Text;

namespace Resourcerer.HttpClientGenerator;

public class TypeScriptMapper
{
    private readonly string _solutionPath;
    private List<string> _files = new();
    private StringBuilder _outer = new StringBuilder();
    private StringBuilder _inner = new StringBuilder();
    private readonly Dictionary<string, string> _exportTypes = new()
    {
        { typeof(int).Name, "number" },
        { typeof(double).Name, "number" },
        { typeof(decimal).Name, "number" },
        { typeof(bool).Name, "boolean" },
        { typeof(Guid).Name, "string" },
        { typeof(string).Name, "string" },
        { typeof(DateTime).Name, "Date" }
    };
    
    public TypeScriptMapper()
    {
        _solutionPath = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.IndexOf("src"));
    }
    public void Export(Type t)
    {
        var propInfo = t.GetProperties();
        var file = Path.Combine(_solutionPath, $"I{t.Name}.ts");
        
        foreach (var prop in propInfo)
        {
            var tsType = GetTsType(prop);
            _inner.Append("\t");
            _inner.Append(ToCamelCase(prop.Name));
            // is nullable
            _inner.Append(":");
            _inner.Append($" {tsType};");
            _inner.Append(Environment.NewLine);
        }
        // _outer add imports here
        _outer.Append($"export default interface I{t.Name}");
        
        if (t.BaseType!.Name != typeof(Object).Name)
        {
            _outer.Append($" extends I{t.BaseType!.Name}");
        }
        _outer.Append(" {");
        _outer.Append(Environment.NewLine);
        _outer.Append(_inner.ToString());
        _outer.Append(Environment.NewLine);
        _outer.Append("}");

        File.WriteAllText(file, _outer.ToString());
        
        _inner.Clear();
        _outer.Clear();
    }

    private string GetTsType(PropertyInfo propInfo)
    {
        if (propInfo.PropertyType.IsGenericType && propInfo.PropertyType.GetGenericTypeDefinition()== typeof(List<>))
        {
            Type itemType = propInfo.PropertyType.GetGenericArguments()[0];
        }
        if (_exportTypes.TryGetValue(propInfo.PropertyType.Name, out string? val))
        {
            return val;
        }
        else
        {
            return "blargh";
        }
    }

    private string ToCamelCase(string s)
    {
        return Char.ToLowerInvariant(s[0]) + s.Substring(1);
    }
}
