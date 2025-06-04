using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using Shared.Email.Entities;
using Shared.Extensions;

namespace Shared.Email.Services;

public class EmailService(EmailConfiguration emailConfig): IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new EmailMessage(
            To: email,
            Subject: subject,
            Content: htmlMessage,
            "",
            false);
        
        var emailMessage = await CreateEmailMessage(message, new CancellationToken());

        if (emailConfig.UseCC)
        {
            emailMessage.To.AddRange(emailConfig.CC!.Split(",").ToList().Select(x => new MailboxAddress("cc", x)));
        }

        if (!emailConfig.Disabled || message.Force)
        {
            await Send(emailMessage, new CancellationToken());
        }
        else
        {
            var log = $"<= EmailSender {DateTime.UtcNow:yyyy-MM-dd hh:mm:ss}, Sending email is disabled, so not sending this email regarding: {message.Subject} to {string.Join(", ", message.To)} with body: {message.Content}";
            Console.WriteLine(log);
        }
    }
    
    public async Task<bool> OldSendEmailAsync(EmailMessage message, CancellationToken ct)
    {
        var emailMessage = await CreateEmailMessage(message, ct);

        if (emailConfig.UseCC)
        {
            emailMessage.To.AddRange(emailConfig.CC!.Split(",").ToList().Select(x => new MailboxAddress("cc", x)));
        }

        if (!emailConfig.Disabled || message.Force)
        {
            await Send(emailMessage, ct);
        }
        else
        {
            var log = $"<= EmailSender {DateTime.UtcNow:yyyy-MM-dd hh:mm:ss}, Sending email is disabled, so not sending this email regarding: {message.Subject} to {string.Join(", ", message.To)} with body: {message.Content}";
            Console.WriteLine(log);
        }

        return true;
    }
    private async Task Send(MimeMessage mailMessage, CancellationToken ct)
    {
        await Task.Delay(0);
        using MailKit.Net.Smtp.SmtpClient client = new();
        try
        {
            if (emailConfig.UseAuthentication)
            {
                client.Connect(emailConfig.SmtpServer, emailConfig.Port, true, ct);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(emailConfig.UserName, emailConfig.Password, ct);
            }
            else
            {
                client.Connect(emailConfig.SmtpServer, emailConfig.Port, false, ct);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
            }
            client.Send(mailMessage, ct);
            var log = $"<= EmailSender {DateTime.UtcNow:yyyy-MM-dd hh:mm:ss}, Sending email regarding: {mailMessage.Subject} to {string.Join(", ", mailMessage.To)} with body: {mailMessage.Body}";
            Console.WriteLine(log);
        }
        catch (Exception ex)
        {
            string log = $"<= EmailSender {DateTime.UtcNow:yyyy-MM-dd hh:mm:ss}, Error sending email regarding: {mailMessage.Subject} to {string.Join(", ", mailMessage.To)} with body: {mailMessage.Body} " + ex;
            Console.WriteLine(log);
        }
        finally
        {
            client.Disconnect(true, ct);
            client.Dispose();
        }
    }
    private async Task<MimeMessage> CreateEmailMessage(EmailMessage message, CancellationToken ct)
    {
        await Task.Delay(0);
        MimeMessage emailMessage = new();
        emailMessage.From.Add(new MailboxAddress(emailConfig.From, emailConfig.From));
        emailMessage.To.Add(new MailboxAddress(message.To, message.To));
        emailMessage.Subject = message.Subject;

        BodyBuilder builder = new()
        {
            HtmlBody = message.Content.IsHtml() ? message.Content : message.Content.ParseAsHtml(),
            TextBody = message.Content.IsHtml() ? message.Content.ParseAsPlain() : message.Content
        };

        if (message.Attachment.Length > 3)
        {
            builder.Attachments.Add(message.Attachment);
        }
        emailMessage.Body = builder.ToMessageBody();

        return emailMessage;
    }

}



