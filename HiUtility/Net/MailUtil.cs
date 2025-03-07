
using MimeKit;
using MailKit.Net.Smtp;

namespace MrHihi.HiUtility.Net;

public class MailUtil {
    public class SmtpInfo {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public required string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool WithoutAuthentication { get { return string.IsNullOrEmpty(UserName); } }
        public bool UseSSL { get; set; }
    }

    public class MessageInfo {
        public required string Sender { get; set; }
        public IList<string> Reciver { get; set; } = new List<string>();
        public IList<string> Cc { get; set; } = new List<string>();
        public required string Subject { get; set; }
        public required string Content { get; set; }
        private IList<MimePart> attachments { get; set; } = new List<MimePart>();

        public void AddAttachment(string mediaType, (Stream stream, string FileName) file)
        {
            var m = mediaType.Split("/");
            var att = new MimePart(m[0], m[1]) {
                Content = new MimeContent(file.stream),
                ContentDisposition = new MimeKit.ContentDisposition(MimeKit.ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = file.FileName
            };
            attachments.Add(att);
        }

        public void AddAttachment(string mediaType, (string FilePath, string FileName) file)
        {
            var fileContent = File.OpenRead(file.FilePath);
            var m = mediaType.Split("/");
            AddAttachment(mediaType, (fileContent, file.FileName));
        }

        private MimeEntity GetContent()
        {
            var body = new TextPart("plain") {
                                Text = this.Content
                            };
            if (attachments.Count == 0) return body;

            var result = new Multipart("mixed");
            result.Add(body);
            foreach(var att in attachments)
            {
                result.Add(att);
            }
            return result;
        }

        static private MailboxAddress parseMailAddress(string addr)
        {
            
            var r =  addr.Split('@');
            var rr = (name: r[0], address: r[1]);
            return new MailboxAddress(rr.name, addr);
        }

        public MimeMessage ToMessage()
        {
            var msg = new MimeKit.MimeMessage();
            msg.From.Add(parseMailAddress(this.Sender));
            if (this.Reciver ==  null || this.Reciver.Count == 0) {
                throw new Exception("Recipient list cannot be empty!");
            }
            msg.To.AddRange((from r in this.Reciver select parseMailAddress(r)));
            if (this.Cc != null && this.Cc.Count > 0) {
                msg.Cc.AddRange((from r in this.Cc select parseMailAddress(r)));
            }
            msg.Subject = this.Subject;
            msg.Body = this.GetContent();
            return msg;
        }
    }

    public static void SendMail(SmtpInfo smtp, MessageInfo minfo)
    {
        SendMail(smtp, minfo.ToMessage());
    }

    public static void SendMail(SmtpInfo smtp, MimeMessage message)
    {
        SendMail(smtp, new List<MimeMessage> { message });
    }

    public static void SendMail(SmtpInfo smtp, IList<MimeMessage> messages)
    {
        using(var client = new SmtpClient())
        {
            try
            {
                if (smtp.UseSSL)
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.CheckCertificateRevocation = false;
                    client.Connect(smtp.SmtpServer, smtp.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                }
                else
                {
                    client.Connect(smtp.SmtpServer, smtp.SmtpPort);
                }
                
                if (!smtp.WithoutAuthentication) {
                    client.Authenticate(smtp.UserName, smtp.Password);
                }
                foreach(var msg in messages) {
                    client.Send(msg);
                }
                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"SMTP: {smtp.SmtpServer}:{smtp.SmtpPort}");
                Console.WriteLine($"Authenticate: {smtp.UserName}");
                throw;
            }
        }
    }
}
