
using System.Net;
using System.Net.Mail;

namespace MyNotes.Email
{
	public class EmailSender : IEmailSender
	{
		private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration _configuration)
        {
			this._configuration = _configuration;

		}
        public async Task SendEmailAsync(string email, string subject, string message)
		{
			var SmtpClient = new SmtpClient(_configuration["Email:SmtpServer"])
			{
				Port = int.Parse(_configuration["Email:Port"]!),
				Credentials = new NetworkCredential(_configuration["Email:Username"], _configuration["Email:Password"]),
				EnableSsl=true

			};

			var MailMessage = new MailMessage()
			{
				From = new MailAddress(_configuration["Email:From"]!),
				Subject = subject,
				Body = message,
				IsBodyHtml = true
			};

			MailMessage.To.Add(email);

			await SmtpClient.SendMailAsync(MailMessage);

			
		}

	}
}
