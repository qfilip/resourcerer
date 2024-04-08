using Resourcerer.Logic;

namespace Resourcerer.Api.Services;

public class EmailService : IEmailService
{
    public Task Send(string content, string email) => Task.CompletedTask;
    public bool Validate(string email) => email.Contains("@");
}
