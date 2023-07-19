using Reinforced.Typings.Fluent;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using System.Reflection;
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
            .WithProperties(i => i.PropertyType == typeof(Guid) || i.PropertyType == (typeof(Guid?)), i => i.Type("string"));

        return conf;
    }

    private static Type[] GetTypesForExport<T>(string namespaceFilter)
    {
        var assembly = Assembly
            .GetAssembly(typeof(T));

        if(assembly == null)
        {
            throw new Exception("Assembly not found");
        }
        
        return typeof(IBaseDto)
            .Assembly
            .GetTypes()
            .Where(x =>
                x.GetInterface(typeof(IBaseDto).Name) != null &&
                !x.IsAbstract &&
                !x.IsInterface)
            .ToArray();

        return assembly.ExportedTypes
            .Where(i => i.Namespace!.StartsWith(namespaceFilter))
            .OrderBy(i => i.Name)
            .OrderBy(i => i.Name != nameof(T))
            .ToArray();
    }

    private static readonly Action<InterfaceExportBuilder> _interfacesConfiguration = conf => conf
        .BaseConfiguration()
        .ExportTo("interfaces.ts");


    public static void Configure(TsBuilder builder)
    {
        builder.Global(i => i.UseModules());
        builder.ConfigureTypes();
    }

    private static void ConfigureTypes(this TsBuilder builder)
    {
        var dtos = GetTypesForExport<IBaseDto>($"{SOLUTION_NAMESPACE}.Dtos");

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
                //typeof(eEntityStatus),
                //typeof(ePermission),
                //typeof(eSection)
            },
            conf => conf.ExportTo("enums.ts")
        );
    }

}
