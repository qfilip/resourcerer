using Resourcerer.Messaging.Emails.Abstractions;

namespace Resourcerer.Messaging.Emails;

public class EmailService : IEmailSender
{
    public Task SendAsync(Email _) => Task.CompletedTask;
    public bool Validate(string? address) => address != null && address.Contains("@");
}
