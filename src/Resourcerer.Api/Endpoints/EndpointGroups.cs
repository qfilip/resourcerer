namespace Resourcerer.Api.Endpoints;

public class EndpointGroups
{
    public static string Fake(string path) => $"/fake/{path}";
    public static string Categories(string path) => $"/categories/{path}";
    public static string Companies(string path) => $"/companies/{path}";
    public static string Instances(string path) => $"/instances/{path}";
    public static string Items(string path) => $"/items/{path}";
    public static string UnitsOfMeasure(string path) => $"/unitsOfMeasure/{path}";
    public static string Users(string path) => $"/users/{path}";
}
