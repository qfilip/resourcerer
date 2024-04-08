namespace Resourcerer.Logic;

public interface IEmailService
{
    bool Validate(string email);
    Task Send(string content, string email);
}
