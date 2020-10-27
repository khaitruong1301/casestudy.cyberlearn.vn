using MailKit.Net.Smtp;
using MimeKit;
using ApiBase.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApiBase.Service.Services
{
    public interface IMailService
    {
        Task SendMailAsync(string toName, string toEmail);
    }

    public class MailService : IMailService
    {
        private readonly IMailSettings _mailSettings;

        public MailService(IMailSettings mailSettings)
        {
            _mailSettings = mailSettings;
        }

        private async Task ExcuteAsync(MimeMessage mimeMessage)
        {   // Sử dụng thư viện MailKit.Net.Smtp 
            using (SmtpClient client = new SmtpClient())
            {   // Tham số cuối cùng ở đây là sử dụng SSL (Nên sử dụng)
                //client.Connect("smtp.gmail.com", 465, true);
                client.Connect(_mailSettings.SmtpServer, _mailSettings.SmtpPort, true);
                // Xóa mọi chức năng OAuth, chúng ta sẽ không sử dụng nó.
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                // Đăng nhập mail gửi
                client.Authenticate(_mailSettings.SmtpUsername, _mailSettings.SmtpPassword);
                // Thực hiện gửi mail
                await client.SendAsync(mimeMessage);
                // Gửi xong ngắt kết nối
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendMailAsync(string toName, string toEmail)
        {
            MimeMessage message = new MimeMessage();
            try
            {
                message.Subject = _mailSettings.Subject;
                // Thông tin mail gửi
                MailboxAddress from = new MailboxAddress(_mailSettings.FromName, _mailSettings.FromAddresses);
                message.From.Add(from);
                // Thông tin mail nhận
                MailboxAddress to = new MailboxAddress(toName, toEmail);
                message.To.Add(to);
                // Nội dung mail
                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = _mailSettings.Content;
                message.Body = bodyBuilder.ToMessageBody();

                await ExcuteAsync(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
