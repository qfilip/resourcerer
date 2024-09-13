using Resourcerer.Application.Messaging.Emails.Abstractions;
using Resourcerer.Application.Messaging.Emails.Models;

namespace Resourcerer.Application.Messaging.Emails;

public class EmailService : IEmailSender
{
    public Task SendAsync(Email _) => Task.CompletedTask;
    public bool Validate(string? address) => address != null && address.Contains("@");
}
