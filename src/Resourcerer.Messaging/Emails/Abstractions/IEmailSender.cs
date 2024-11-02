using Resourcerer.Messaging.Abstractions;
using Resourcerer.Messaging.Emails.Models;

namespace Resourcerer.Messaging.Emails.Abstractions;

public interface IEmailSender : IMessageSender<Email>
{
    bool Validate(string? address);
}
