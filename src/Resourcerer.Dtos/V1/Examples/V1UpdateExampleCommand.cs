namespace Resourcerer.Dtos.V1;

public class V1UpdateExampleCommand : V1ExampleCommand
{
    public Guid ExampleId { get; set; }
    public string? NewText { get; set; }
}
