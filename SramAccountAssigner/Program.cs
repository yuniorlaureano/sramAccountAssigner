using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SramAccountAssigner.Data;
using SramAccountAssigner.Entity;
using System.Data;
using System.Configuration;
using System.IO;
using System.Net.Mail;

namespace SramAccountAssigner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Assigner assigner = new Assigner(Country.PRI, ENVVAR.PRI);
            DataTable resultset = null;
            string user = string.Empty;
            string auditors = string.Empty;
            string mailToSend = string.Empty;
            AssignerLogic assignerLogic = null;

            try
            {
                user = ConfigurationManager.AppSettings["RESPONSIBLE"].ToString();
                auditors = ConfigurationManager.AppSettings["AUDITORS"].ToString();
                mailToSend = ConfigurationManager.AppSettings["MailToSend"].ToString();

               

                resultset = assigner.AutoAssign(user, auditors);

                if (resultset.Rows.Count > 0)
                {
                    assignerLogic = new AssignerLogic(resultset, AppDomain.CurrentDomain.BaseDirectory + "HtmlMailTemplate\\Assigment.html");

                    //string path = assigner.WriteToExcel(resultset, "ASSIGNED", "asigned-accounts", Directory.GetCurrentDirectory() + "\\");

                    Console.WriteLine("-------------------------------");
                    Console.WriteLine("Sending mails");

                    SendNotification(
                              "Asignación de cuentas automática."
                             , assignerLogic.HtmlMailMessageBody()
                             , new List<Attachment>()//{ new Attachment(path) }
                             , MialTo(mailToSend)
                             , true
                        );

                    Console.WriteLine("...........Sending sent..........");
                }
                else
                {
                    SendNotification(
                         "Asignación de cuentas automática."
                        , "No hay cuentas pendientes por asignar."
                        , new List<Attachment>()
                        , MialTo(mailToSend)
                    );
                }
            }
            catch (Exception except)
            {
                SendNotification(
                         "Asignación de cuentas automática-ERROR"
                        , "Error al asignar las cuentas. Contactar a: Y.Laureano@caribemedia.com.do. ["+except.Message+"]"
                        ,new List<Attachment>()
                        , MialTo(mailToSend)
                    );
                throw except;
            }
            
        }

        public static void SendNotification(string subject, string body, List<Attachment> attachments, List<MailAddress> mails, bool isHtml = false)
        {

            MailMessage message = new MailMessage();
            message.From = new MailAddress("Desarrollo/sistemas@paginasamarillas.com.do");

            foreach (MailAddress ml in mails)
                message.To.Add(ml);

            message.Subject = subject;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Priority = MailPriority.Normal;
            message.Body = body;
            message.IsBodyHtml = isHtml;

            foreach (Attachment attch in attachments)
                message.Attachments.Add(attch);

            SmtpClient server = new SmtpClient("172.27.136.11", 25);
            server.UseDefaultCredentials = true;
            server.DeliveryMethod = SmtpDeliveryMethod.Network;
            server.EnableSsl = false;
            server.Send(message);
        }

        public static List<MailAddress> MialTo(string separatedCommaMails)
        {
            string[] mails = null;
            List<MailAddress> mailTo = new List<MailAddress>();

            if (!separatedCommaMails.Contains(","))
            {
                mailTo.Add(new MailAddress(separatedCommaMails));
                return mailTo;
            }

            mails = separatedCommaMails.Split(',');

            foreach (string _mailto in mails)
            {
                mailTo.Add(new MailAddress(_mailto));
            }

            return mailTo;
        }
    }
}
