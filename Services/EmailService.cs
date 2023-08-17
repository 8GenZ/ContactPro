using ContactPro.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Drawing.Text;

namespace ContactPro.Services
{
    public class EmailService : IEmailSender
    {
		private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

            try
            {
                var emailAddress = _emailSettings.EmailAddress ?? Environment.GetEnvironmentVariable("EmailAddress");
                var emailPassword = _emailSettings.EmailPassword ?? Environment.GetEnvironmentVariable("EmailPassword");
                var emailHost = _emailSettings.EmailHost ?? Environment.GetEnvironmentVariable("EmailHost");
                var emailPort = _emailSettings.EmailPort != 0 ? _emailSettings.EmailPort : int.Parse(Environment.GetEnvironmentVariable("EmailPort")!);

                MimeMessage newEmail = new MimeMessage();

                //Attatch the email sender
                newEmail.Sender = MailboxAddress.Parse(emailAddress);

                //Attach the email recipients
                foreach (string address in email.Split(";"))
                {
                    newEmail.To.Add(MailboxAddress.Parse(address));
                }

                //Set the Subject
                newEmail.Subject = subject;

                //Format the body
                BodyBuilder emailBody = new BodyBuilder();
                emailBody.HtmlBody = htmlMessage;
                newEmail.Body = emailBody.ToMessageBody();

                //prep the service and send the email
                using SmtpClient smtpClient = new SmtpClient();
                try
                {
                    //Go's to gmail.com
                    await smtpClient.ConnectAsync(emailHost, emailPort, SecureSocketOptions.StartTls);
                    //Log's into gmail.com
                    await smtpClient.AuthenticateAsync(emailAddress, emailPassword);
                    //Send's the email we just made
                    await smtpClient.SendAsync(newEmail);
                    //Log's out
                    await smtpClient.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    //var error = ex.message
                    throw;
                }


            }
            catch (Exception)
			{

				throw;
			}
        }
    }
}
