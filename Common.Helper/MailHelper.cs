using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

namespace Common.Helper
{
    /// <summary>
    /// This class contain the functionality related to mail sending.
    /// </summary>
    public class MailHelper
    {
        /// <summary>
        /// Build a message based in a html template, and a bunch of paratemers.
        /// </summary>
        /// <param name="htmlTemplatePath">The path of the template</param>
        /// <param name="parameters">Embed parameters in the html text</param>
        /// <returns>string</returns>
        public string BuildMessage(string htmlTemplatePath, List<ParamDictionary> parameters)
        {
            string rawText;

            rawText = File.ReadAllText(htmlTemplatePath);

            foreach (ParamDictionary parameter in parameters)
            {
                rawText = rawText.Replace(parameter.Key, parameter.Value);
            }

            return rawText;
        }

        /// <summary>
        /// get a listo of separeted comman mails, and transform it into a List<MailAddress>
        /// </summary>
        /// <param name="commaSeparetedMail">Comman separated mails</param>
        /// <returns>List<MailAddress></returns>
        public List<MailAddress> BuildMailTo(string commaSeparetedMail)
        {
            string[] mails = null;
            List<MailAddress> mailAddresses = new List<MailAddress>();

            if (string.IsNullOrEmpty(commaSeparetedMail))
            {
                throw new ArgumentNullException("Comma separated mails is null");
            }

            if (!commaSeparetedMail.Contains(","))
            {
                mailAddresses.Add(new MailAddress(commaSeparetedMail));

                return mailAddresses;
            }

            mails = commaSeparetedMail.Split(',');

            foreach (var _mails in mails)
            {
                mailAddresses.Add(new MailAddress(_mails));
            }

            return mailAddresses;
        }

        /// <summary>
        /// Send the mail
        /// </summary>
        /// <param name="from">Who is going to reseive the message</param>
        /// <param name="subject">A title indicating the matter of the message</param>
        /// <param name="body">Te content that is going to be sent</param>
        /// <param name="smtpCredentials">The credentialas of the smtp server</param>
        /// <param name="attachments">A list of attatchments</param>
        /// <param name="mails">The mail the message is going to be copy to</param>
        /// <param name="isHtml">If the content is going to be html</param>
        /// <param name="enableSsl">SSL</param>
        /// <param name="UseDefaultCredentials">If it is going to use the default credentials</param>
        public void SendMail(string from, string subject, string body, SmtpClient smtpCredentials, List<Attachment> attachments, List<MailAddress> mails, bool isHtml = false, bool enableSsl = false, bool UseDefaultCredentials = true)
        {
            MailMessage message = new MailMessage();
            SmtpClient server = smtpCredentials;

            message.From = new MailAddress(from);

            //Mails that are going to reseive the message
            foreach (MailAddress ml in mails)
                message.To.Add(ml);

            //Attatchments been sent
            foreach (Attachment attch in attachments)
                message.Attachments.Add(attch);

            //Message configuration
            message.Subject = subject;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Priority = MailPriority.Normal;
            message.Body = body;
            message.IsBodyHtml = isHtml;

            //server configuration
            server.UseDefaultCredentials = UseDefaultCredentials;
            server.DeliveryMethod = SmtpDeliveryMethod.Network;
            server.EnableSsl = enableSsl;
            server.Send(message);
        }
    }
}
