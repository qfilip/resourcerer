// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Resourcerer.Api;
using Resourcerer.Dtos;
using Resourcerer.HttpClientGenerator;

var app = Webapi.Create(args);

var tsMapper = new TypeScriptMapper();
tsMapper.Export(typeof(TestDto));
//var dtos = typeof(IAssemblyMarker).Assembly
//    .GetTypes()
//    .Where(x =>
//        x.IsClass &&
//        !x.IsAbstract &&
//        x.IsSubclassOf(typeof(BaseDto)))
//    .OrderBy(x => x.Name)
//    .ToList();

//dtos.ForEach(tsMapper.Export);


var endpointSources = app as IEndpointRouteBuilder;
var endpoints = new List<Endpoint>();

foreach(var source in endpointSources.DataSources)
{
    endpoints.AddRange(source.Endpoints);
}

//endpoints[0].
var x = 0;

public class Dependency { }
public class TestDto
{
    public Guid Id { get; set; }
    public string? Oooh { get; set; }
    public Guid? MaybeId { get; set; }
    public DateTime DoB { get; set; }
    public List<int>? PrimitiveList { get; set; }
    public Dependency? Dependency { get; set; }
    public List<Dependency>? Dependencies { get; set; }
}