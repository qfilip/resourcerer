﻿using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos.Fake;
using Resourcerer.Logic.Fake;
using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Api.Endpoints.Fake;

public class TestMessagingServices : IAppEndpoint
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

    public AppEndpoint GetEndpointInfo() => 
        new AppEndpoint(1, 0, EndpointMapper.Fake("messaging_test"), eHttpMethod.Post, Action, null);
}

// USED FOR TESTING ENDPOINT MAPPING
//public class TestMessagingServices1 : IAppEndpoint
//{
//    public static async Task<IResult> Action(
//        [FromBody] FakeCommandDto dto,
//        [FromServices] Pipeline pipeline,
//        [FromServices] IMessageSender<FakeCommandDto> sender,
//        [FromServices] FakeCommandEventHandler.Validator validator)
//    {
//        return await pipeline.PipeMessage(
//            dto,
//            validator,
//            sender,
//            nameof(TestMessagingServices));
//    }

//    public AppEndpoint GetEndpointInfo() =>
//        new AppEndpoint(1, 1, EndpointMapper.Fake("messaging_test"), HttpMethod.Post, Action, null);
//}

//public class TestMessagingServices2 : IAppEndpoint
//{
//    public static async Task<IResult> Action(
//        [FromBody] FakeCommandDto dto,
//        [FromServices] Pipeline pipeline,
//        [FromServices] IMessageSender<FakeCommandDto> sender,
//        [FromServices] FakeCommandEventHandler.Validator validator)
//    {
//        return await pipeline.PipeMessage(
//            dto,
//            validator,
//            sender,
//            nameof(TestMessagingServices));
//    }

//    public AppEndpoint GetEndpointInfo() =>
//        new AppEndpoint(1, 2, EndpointMapper.Fake("messaging_test"), HttpMethod.Post, Action, null);
//}

//public class TestMessagingServices3 : IAppEndpoint
//{
//    public static async Task<IResult> Action(
//        [FromBody] FakeCommandDto dto,
//        [FromServices] Pipeline pipeline,
//        [FromServices] IMessageSender<FakeCommandDto> sender,
//        [FromServices] FakeCommandEventHandler.Validator validator)
//    {
//        return await pipeline.PipeMessage(
//            dto,
//            validator,
//            sender,
//            nameof(TestMessagingServices));
//    }

//    public AppEndpoint GetEndpointInfo() =>
//        new AppEndpoint(2, 0, EndpointMapper.Fake("messaging_test"), HttpMethod.Post, Action, null);
//}