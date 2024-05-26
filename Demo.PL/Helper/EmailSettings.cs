using System.Net;
using System.Net.Mail;

namespace Demo.PL.Helper
{
    public static class EmailSettings
    {
        public static void SendEmail(Email email)  
        {
            var client = new SmtpClient("smtp.gmail.com", 587);

            client.EnableSsl = true;

            client.Credentials = new NetworkCredential("adelashraf349@gmail.com", "xfrpdfgqwesvmrkj");

            client.Send("adelashraf349@gmail.com", email.To, email.Title, email.Body);
        }
    }
}
