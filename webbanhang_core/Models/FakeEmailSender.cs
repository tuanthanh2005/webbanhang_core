namespace webbanhang_core.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

public class FakeEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // In ra console để debug
        Console.WriteLine($"SendEmailAsync to: {email} | subject: {subject}");
        Console.WriteLine(htmlMessage);
        return Task.CompletedTask;
    }
}
