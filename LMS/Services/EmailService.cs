using System.Net.Mail;
using System.Net;

namespace LMS.Services
{
    public interface IEmailService
    {
        Task SendEmail(string to, string subject, string body, bool isHtml = false);
        Task SendConfirmationEmailAsync(string to, string confirmationLink);
        Task SendForgotPassEmailAsync(string to, string resetLink);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpHost = _configuration["EMAIL_CONFIG:HOST"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(_configuration["EMAIL_CONFIG:PORT"] ?? "587");
            _smtpUsername = _configuration["EMAIL_CONFIG:USERNAME"] ?? throw new ArgumentNullException("EMAIL_CONFIG:USERNAME");
            _smtpPassword = _configuration["EMAIL_CONFIG:PASSWORD"] ?? throw new ArgumentNullException("EMAIL_CONFIG:PASSWORD");
            _fromEmail = _configuration["EMAIL_CONFIG:FROM_EMAIL"] ?? "smtpUsername";
            _fromName = _configuration["EMAIL_CONFIG:FROM_NAME"] ?? "App Name";
        }

        public async Task SendEmail(string to, string subject, string body, bool isHtml = false)
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            message.To.Add(to);

            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }

        public async Task SendConfirmationEmailAsync(string to, string confirmationLink)
        {
            var subject = "Confirm your email";
            var body = $@"
                <table style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border-collapse: collapse; border: 1px solid #e0e0e0;'>
                     <tr style='background-color: #28a745; color: white;'>
                    <td style='padding: 20px; text-align: center;'>
                        <h2>Confirm Your Email Address</h2>
                     </td>
                    </tr>
                    <tr>
                    <td style='padding: 20px;'>
                        <p>Hello,</p>
                        <p>Thank you for registering. Please confirm your email address by clicking the button below:</p>
                    <p style='text-align: center;'>
                     <a href='{confirmationLink}' style='background-color: #28a745; color: white; padding: 10px 20px; text-decoration: none; border-radius: 4px;'>Confirm Email</a>
                    </p>
                    <p>If the button above doesn’t work, copy and paste this link into your browser:</p>
                    <p style='word-break: break-all; color: #555;'>{confirmationLink}</p>
                    <p>If you did not register for an account, please ignore this email.</p>
                    <p>Thank you,<br/>Your Team</p>
                    </td>
                </tr>
                </table>"; ;

            await SendEmail(to, subject, body, true);
        }

        public async Task SendForgotPassEmailAsync(string to, string resetLink)
        {
            var subject = "Reset Your Password";
            var body = $@"
                           <table style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border-collapse: collapse; border: 1px solid #e0e0e0;'>
                                  <tr style='background-color: #4a90e2; color: white;'>
                                       <td style='padding: 20px; text-align: center;'>
                                             <h2>Password Reset Request</h2>
                                        </td>
                                   </tr>
                                   <tr>
                                        <td style='padding: 20px;'>
                                               <p>Hello,</p>
                                                <p>We received a request to reset the password for your account associated with this email address.</p>
                                                <p>If you made this request, please click the button below to reset your password:</p>
                               <p style='text-align: center;'>
                             <a href='{resetLink}' style='background-color: #4a90e2; color: white; padding: 10px 20px; text-decoration: none; border-radius: 4px;'>Reset Password</a>
                             </p>
                                   <p>If the button above doesn’t work, copy and paste this link into your web browser:</p>
                             <p style='word-break: break-all; color: #555;'>{resetLink}</p>
                             <p>This link will expire in 24 hours. If you did not request a password reset, no further action is required.</p>
                                           <p>Thank you,<br/>LMS Team</p>
                                             </td>
                                   </tr>
                           </table>";
            await SendEmail(to, subject, body, true);
        }
    }
}
