using Resourcerer.Application.Messaging.Emails;
using Resourcerer.Application.Messaging.Emails.Abstractions;

namespace Resourcerer.Api.Services;

public static partial class ServiceRegistry
{
    public static void AddEmailServices(IServiceCollection services)
    {
        services.AddSingleton<IEmailSender, EmailService>();
    }
}
