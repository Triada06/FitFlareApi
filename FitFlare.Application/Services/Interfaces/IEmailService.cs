namespace FitFlare.Application.Services.Interfaces;

public interface IEmailService
{
    public Task SendAsync(string emailTo, string topic, string message);
}