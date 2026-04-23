namespace TourCrm.Application.Interfaces;

public interface IEmailService
{
    Task SendCodeAsync(string toEmail, string subject, string message, string htmlMessage);
    string GenerateConfirmationCode();
}