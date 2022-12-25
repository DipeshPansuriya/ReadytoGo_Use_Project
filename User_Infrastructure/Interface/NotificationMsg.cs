using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace User_Infrastructure.Interface
{
    public class NotificationMsg : INotificationMsg
    {
        private readonly IBackgroundJob _backgroundJob;

        public NotificationMsg(IBackgroundJob backgroundJob)
        {
            _backgroundJob = backgroundJob;
        }

        public void SaveMailNotification(string From, string To, string Subject, string Body)
        {
            NotficationCls notfication = new()
            {
                MsgFrom = From,
                MsgTo = To,
                MsgSubject = Subject,
                MsgBody = Body,
                MsgSatus = NotificationStatus.Pending.ToString(),
                MsgType = NotificationType.Mail.ToString(),
                CreatedDate = System.DateTime.Now,
            };

            SendAsync(notfication).Wait();
            //_backgroundJob.AddEnque<IDapper<NotficationCls>>(x => x.SaveNotificationAsync(notfication));
        }

        public async Task<bool> SendAsync(NotficationCls notfication)
        {
            return (object)notfication.MsgType.ToString() switch
            {
                "Mail" => SendMail(notfication),
                "SMS" => true,
                "Whatsapp" => true,
                _ => true,
            };
        }

        private bool SendMail(NotficationCls notfication)
        {
            try
            {
                // create email message
                MimeMessage email = new();
                email.From.Add(MailboxAddress.Parse(notfication.MsgFrom));
                email.To.Add(MailboxAddress.Parse(notfication.MsgTo));
                email.Subject = notfication.MsgSubject;
                email.Body = new TextPart(TextFormat.Html) { Text = notfication.MsgBody };

                // send email
                using SmtpClient smtp = new();
                //smtp.Connect(APISetting.EmailConfiguration.SMTPAddress, APISetting.EmailConfiguration.Port, SecureSocketOptions.StartTls);
                //smtp.Authenticate(APISetting.EmailConfiguration.UserId, APISetting.EmailConfiguration.Password);
                _ = smtp.Send(email);
                smtp.Disconnect(true);

                notfication.MsgSatus = NotificationStatus.Success.ToString();
                notfication.UpdatedDate = DateTime.Now;
                //await _repository.UpdateNotificationAsync(notfication);
                return true;
            }
            catch (Exception ex)
            {
                notfication.MsgSatus = NotificationStatus.Fail.ToString();
                notfication.UpdatedDate = DateTime.Now;
                notfication.FailDetails = ex.InnerException != null ? ex.Message + ex.InnerException.Message : ex.Message;

                //await _repository.UpdateNotificationAsync(notfication);
                return false;
            }
        }
    }
}