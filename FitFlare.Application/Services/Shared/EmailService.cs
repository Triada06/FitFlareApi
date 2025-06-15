using FitFlare.Application.Services.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace FitFlare.Application.Services.Shared;

public class EmailService(IConfiguration config) : IEmailService
{
    public async Task SendAsync(string emailTo, string topic, string message)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(config["EmailSettings:From"]));
        email.To.Add(MailboxAddress.Parse(emailTo));
        email.Subject = topic;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(config["EmailSettings:SmtpServer"], int.Parse(config["EmailSettings:Port"]), true);
        await smtp.AuthenticateAsync(config["EmailSettings:Username"], config["EmailSettings:Password"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}