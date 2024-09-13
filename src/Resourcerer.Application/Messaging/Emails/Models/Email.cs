namespace Resourcerer.Application.Messaging.Emails.Models;

public class Email
{
    public Email() { }

    public Email(string? address, string? subject, string? content)
    {
        Address = address;
        Subject = subject;
        Content = content;
    }
    public string? Address { get; set; }
    public string? Subject { get; set; }
    public string? Content { get; set; }
}
