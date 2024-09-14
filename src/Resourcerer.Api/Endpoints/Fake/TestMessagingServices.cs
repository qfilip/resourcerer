using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Application.Messaging.Abstractions;
using Resourcerer.Dtos.Fake;
using Resourcerer.Logic.Fake;

namespace Resourcerer.Api.Endpoints.Fake;

public class TestMessagingServices
{
    public static async Task<IResult> Action(
        [FromBody] FakeCommandDto dto,
        [FromServices] Pipeline pipeline,
        [FromServices] IMessageSender<FakeCommandDto> sender,
        [FromServices] FakeCommandEventHandler.Validator validator)
    {
        return await pipeline.PipeMessage(
            dto,
            validator,
            sender,
            nameof(TestMessagingServices));
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("messaging_test", Action);
    }
}
