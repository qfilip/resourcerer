using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Messaging.Emails.Abstractions;

public interface IEmailSender : IMessageSender<Email>
{
    bool Validate(string? address);
}
