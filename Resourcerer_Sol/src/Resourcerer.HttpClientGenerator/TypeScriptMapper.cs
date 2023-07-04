using System.Reflection;
using System.Text;

namespace Resourcerer.HttpClientGenerator;

public class TypeScriptMapper
{
    private readonly string _solutionPath;
    private List<string> _files = new();
    private StringBuilder _outer = new StringBuilder();
    private StringBuilder _inner = new StringBuilder();
    private readonly Dictionary<string, ExportType> _exportTypes = new()
    {
        { typeof(int).Name, new ExportType() { DataType = "number" } },
        { typeof(double).Name, new ExportType() { DataType = "number" } },
        { typeof(decimal).Name, new ExportType() { DataType = "number" } },
        { typeof(bool).Name, new ExportType() { DataType = "boolean" } },
        { typeof(Guid).Name, new ExportType() { DataType = "string" } },
        { typeof(Guid?).Name, new ExportType() { DataType = "string", IsNullable = true } },
        { typeof(string).Name, new ExportType() { DataType = "string" } },
        { typeof(DateTime).Name, new ExportType() { DataType = "Date" } }
    };
    
    public TypeScriptMapper()
    {
        _solutionPath = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.IndexOf("src"));
    }
    public void Export(Type t)
    {
        var imports = new HashSet<string>();
        
        var propInfo = t.GetProperties();
        var file = Path.Combine(_solutionPath, $"I{t.Name}.ts");
        
        foreach (var prop in propInfo)
        {
            var tsType = GetTsType(prop);
            _inner.Append("\t");
            _inner.Append(ToCamelCase(prop.Name));
            
            if(tsType.IsNullable)
            {
                _inner.Append("?");
            }
            
            _inner.Append(":");
            _inner.Append($" {tsType.DataType};");
            _inner.Append(Environment.NewLine);
            
            if(tsType.ForImport != null)
            {
                imports.Add(tsType.ForImport);
            }
        }

        foreach(var i in imports)
        {
            _outer.Append($"import I{i} from blah");
        }

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
        imports.Clear();
    }

    private ExportType GetTsType(PropertyInfo propInfo)
    {
        if (propInfo.PropertyType.IsGenericType && propInfo.PropertyType.GetGenericTypeDefinition()== typeof(List<>))
        {
            Type itemType = propInfo.PropertyType.GetGenericArguments()[0];
            return new ExportType() { DataType = "blargh" };
        }
        else if (_exportTypes.TryGetValue(propInfo.PropertyType.Name, out ExportType? val))
        {
            return val;
        }
        else
        {
            return new ExportType() { DataType = "blargh" };
        }
    }

    private string ToCamelCase(string s)
    {
        return Char.ToLowerInvariant(s[0]) + s.Substring(1);
    }
}

public class ExportType
{
    public bool IsNullable { get; set; }
    public string? DataType { get; set; }
    public string? ForImport { get; set; }
}