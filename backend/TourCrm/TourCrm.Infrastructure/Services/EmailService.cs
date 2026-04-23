using System.Text.RegularExpressions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using TourCrm.Application.Interfaces;

namespace TourCrm.Infrastructure.Services;

public class EmailService(IConfiguration config) : IEmailService
{
    public async Task SendCodeAsync(string toEmail, string subject, string message, string htmlMessage)
    {
        System.Net.ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
        {
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                return true;

            return sslPolicyErrors == System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors &&
                   chain.ChainStatus.Any(status => status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.RevocationStatusUnknown);
        };

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(MailboxAddress.Parse(config["EmailSettings:Sender"]));
        emailMessage.To.Add(MailboxAddress.Parse(toEmail));
        emailMessage.Subject = subject;
        var builder = new BodyBuilder
        {
            HtmlBody = htmlMessage,
            TextBody = StripHtml(htmlMessage)
        };
        emailMessage.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
        await smtp.ConnectAsync(config["EmailSettings:SmtpServer"], 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(
            config["EmailSettings:Sender"],
            config["EmailSettings:AppPassword"]);
        await smtp.SendAsync(emailMessage);
        await smtp.DisconnectAsync(true);
    }
    
    public string GenerateConfirmationCode() => new Random().Next(100000, 999999).ToString();
    private static string StripHtml(string html)
        => Regex.Replace(html, "<.*?>", string.Empty).Trim();
}