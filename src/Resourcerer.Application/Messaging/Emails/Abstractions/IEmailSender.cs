using Resourcerer.Application.Messaging.Abstractions;
using Resourcerer.Application.Messaging.Emails.Models;

namespace Resourcerer.Application.Messaging.Emails.Abstractions;

public interface IEmailSender : IMessageSender<Email>
{
    bool Validate(string? address);
}
