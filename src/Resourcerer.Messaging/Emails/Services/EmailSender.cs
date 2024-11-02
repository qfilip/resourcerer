using Resourcerer.Messaging.Emails.Abstractions;
using Resourcerer.Messaging.Emails.Models;

namespace Resourcerer.Messaging.Emails.Services;

internal class EmailSender : IEmailSender
{
    public Task SendAsync(Email message) => Task.CompletedTask;
    public bool Validate(string? address) => true;
}
