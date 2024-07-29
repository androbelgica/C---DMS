using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace DMS.LOGIN
{
    public class EmailService
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService()
        {
            _apiKey = ConfigurationManager.AppSettings["MailjetApiKey"];
            _apiSecret = ConfigurationManager.AppSettings["MailjetApiSecret"];
            _fromEmail = ConfigurationManager.AppSettings["MailjetFromEmail"];
            _fromName = ConfigurationManager.AppSettings["MailjetFromName"];
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            MailjetClient client = new MailjetClient(_apiKey, _apiSecret);
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
            .Property(Send.FromEmail, _fromEmail)
            .Property(Send.FromName, _fromName)
            .Property(Send.Subject, "Password Reset")
            .Property(Send.HtmlPart, $"<h3>Dear User,</h3><p>Please reset your password using the following link: <a href='{resetLink}'>Reset Password</a></p>")
            .Property(Send.To, toEmail);

            MailjetResponse response = await client.PostAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Total: {response.GetTotal()}, Count: {response.GetCount()}\n");
                Console.WriteLine(response.GetData());
            }
            else
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}\n");
                Console.WriteLine($"ErrorInfo: {response.GetErrorInfo()}\n");
                Console.WriteLine(response.GetData());
                Console.WriteLine($"ErrorMessage: {response.GetErrorMessage()}\n");
            }
        }
    }
}