using Reinforced.Typings.Fluent;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos;
using System.Text;
using TsBuilder = Reinforced.Typings.Fluent.ConfigurationBuilder;

namespace Resourcerer.Api.TsExport;

public static class Exporter
{
    public const string SOLUTION_NAMESPACE = "Resourcerer";

    private static TBuilder BaseConfiguration<TBuilder>(this TBuilder conf)
        where TBuilder : ClassOrInterfaceExportBuilder
    {
        conf
            .WithPublicProperties(i => i.CamelCase())
            .WithProperties(i => i.PropertyType == typeof(DateTime) || i.PropertyType == (typeof(DateTime?)), i => i.Type("Date"))
            .WithProperties(i => i.PropertyType == typeof(Guid) || i.PropertyType == (typeof(Guid?)), i => i.Type("string"))
            .WithProperties(i => i.PropertyType == typeof(Dictionary<Guid, double>), i => i.Type("{ [key:string]: number }"))
            .WithProperties(i => i.PropertyType == typeof(Dictionary<Guid, int>), i => i.Type("{ [key:string]: number }"));
        
        return conf;
    }

    private static Type[] GetDtos<T>()
    {   
        return typeof(IDto)
            .Assembly
            .GetTypes()
            .Where(x =>
                x.GetInterface(typeof(IDto).Name) != null &&
                !x.IsInterface)
            .ToArray();
    }

    private static readonly Action<InterfaceExportBuilder> _interfacesConfiguration = conf => conf
        .BaseConfiguration()
        .ExportTo("interfaces.ts");

    private static void ExportCustom(TsBuilder builder)
    {
        var customExporters = new List<(Action<StringBuilder> exporter, string file)>()
        {
            (CustomExports.ExportPermissionsMapConst, "constants.ts")
        };

        var sb = new StringBuilder();
        customExporters.ForEach(ce =>
        {
            ce.exporter(sb);
            var path = Path.Combine(builder.Context.TargetDirectory, ce.file);
            File.WriteAllText(path, sb.ToString());
            sb.Clear();
        });
    }


    public static void Configure(TsBuilder builder)
    {
        builder.Global(i => i.UseModules());
        builder.ConfigureTypes();
    }

    private static void ConfigureTypes(this TsBuilder builder)
    {
        var dtos = GetDtos<IDto>();

        // export self defined
        ExportCustom(builder);

        // Tools > Options > Projects And Solutions > Build And Run
        // Set MSBuild project build output verbosity -> Detailed
        Console.WriteLine("EXPORT-DIR");
        Console.WriteLine(builder.Context.TargetDirectory);
        Console.WriteLine(builder.Context.TargetFile);
        
        foreach(var type in dtos)
            Console.WriteLine($"Exporting {type.Name}");
        
        // dto export
        Console.WriteLine($"builder {builder.ToString()}");
        builder.ExportAsInterfaces(dtos, _interfacesConfiguration);

        // enum export
        builder.ExportAsEnums(new Type[]
            {
                typeof(eEntityStatus),
                typeof(ePermission),
                typeof(ePermissionSection)
            },
            conf => conf.ExportTo("enums.ts")
        );
    }

}
