using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Services.Description;
using Riipen_SSD.AdminViewModels;

namespace Riipen_SSD.BusinessLogic
{

    public class MailHelper
    {
        public const string SUCCESS
        = "Success! Your email has been sent.  Please allow up to 48 hrs for a reply.";
        const string BY = "RiipenAdmin@gmail.com";
        public string EmailFromArvixe(AdminViewModels.Message message)
        {

            // Use credentials of the Mail account that you created with the steps above.
            const string FROM = "riipenjudgeathon@gmail.com";
            const string FROM_PWD = "123Pa$$w0rd";
            const bool USE_HTML = true;

            // Get the mail server obtained in the steps described above.
            const string SMTP_SERVER = "smtp.gmail.com";
            try
            {
                MailMessage mailMsg = new MailMessage(FROM, message.Sender);
                mailMsg.Subject = message.Subject;
                mailMsg.Body = message.Body + "<br/>sent by: " + BY;
                mailMsg.IsBodyHtml = USE_HTML;

                SmtpClient smtp = new SmtpClient();
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Host = SMTP_SERVER;
                smtp.Credentials = new System.Net.NetworkCredential(FROM, FROM_PWD);
                smtp.Send(mailMsg);
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }
            return SUCCESS;
        }
    }

}
