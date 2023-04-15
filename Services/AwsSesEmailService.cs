using Abstractions.Interfaces;
using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace Services
{
    public class AwsSesEmailService : IEmailService
    {
        public async Task Send(string emailAddress, string body)
        {
            using var emailClient = new AmazonSimpleEmailServiceClient(RegionEndpoint.USEast2);

            // Send the email using AWS SES
            await emailClient.SendEmailAsync(new SendEmailRequest
            {
                Source = "thespoilkid@gmail.com",
                Destination = new Destination { 
                    ToAddresses = new List<string> { emailAddress } },
                Message = new Message
                {
                    Subject = new Content("AWS DotNet Console - Resume Uploaded"),
                    Body = new Body { Text = new Content(body) }
                }
            });
        }
    }

}
