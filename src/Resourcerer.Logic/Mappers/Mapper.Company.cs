using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;

namespace Resourcerer.Logic;

public static partial class Mapper
{
    public static Company Map(CompanyDto src) =>
        Map(() =>
            new Company
            {
                Name = src.Name,
                
                Employees = src.Employees.Select(x => Map(x)).ToArray(),
                Categories = src.Categories.Select(x => Map(x)).ToArray(),
                Instances = src.Instances.Select(x => Map(x)).ToArray(),
                UnitsOfMeasure = src.UnitsOfMeasure.Select(x => Map(x)).ToArray(),
            }, src);

    public static CompanyDto Map(Company src) =>
        Map(() =>
            new CompanyDto
            {
                Name = src.Name,

                Employees = src.Employees.Select(x => Map(x)).ToArray(),
                Categories = src.Categories.Select(x => Map(x)).ToArray(),
                Instances = src.Instances.Select(x => Map(x)).ToArray(),
                UnitsOfMeasure = src.UnitsOfMeasure.Select(x => Map(x)).ToArray(),
            }, src);
}
