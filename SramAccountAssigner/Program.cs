using System;
using System.Collections.Generic;
using SramAccountAssigner.Data;
using SramAccountAssigner.Entity;
using System.Data;
using System.Configuration;
using System.Net.Mail;

namespace SramAccountAssigner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Country country = (Country) Enum.Parse(typeof(Country), ConfigurationManager.AppSettings["country"].ToString());
            ENVVAR envar = (ENVVAR) Enum.Parse(typeof(ENVVAR), ConfigurationManager.AppSettings["country"].ToString());

            Assigner assigner = new Assigner(country, envar);
            DataTable resultset = null;
            string user = string.Empty;
            string auditors = string.Empty;
            string subject = string.Empty;
            AssignerLogic assignerLogic = null;

            string commaSeparatedMails = ConfigurationManager.AppSettings["Mails_" + country];
            string fromMail = ConfigurationManager.AppSettings["FromMail"];
            int smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
            string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];

            Common.Helper.MailHelper mailHelper = new Common.Helper.MailHelper();

            try
            {
                user = ConfigurationManager.AppSettings["RESPONSIBLE_" + country].ToString();
                auditors = ConfigurationManager.AppSettings["AUDITORS_" + country].ToString();
                subject = ConfigurationManager.AppSettings["Subject_" + country].ToString();
                

                resultset = assigner.AutoAssign(user, auditors);

                if (resultset.Rows.Count > 0)
                {
                    assignerLogic = new AssignerLogic(resultset);

                    List<Common.Helper.ParamDictionary> parameters = new List<Common.Helper.ParamDictionary>
                    {
                        new Common.Helper.ParamDictionary { Key = "quantity", Value = assignerLogic.GetTotalAssigned().ToString() },
                        new Common.Helper.ParamDictionary { Key = "dates", Value = assignerLogic.GetSalesDate() },
                        new Common.Helper.ParamDictionary { Key = "responsible", Value = ConfigurationManager.AppSettings["RESPONSIBLE_NAME_" + country].ToString() },
                        new Common.Helper.ParamDictionary { Key = "country", Value = ConfigurationManager.AppSettings["country_" + country].ToString() },
                        new Common.Helper.ParamDictionary { Key = "htmlcontent", Value = assignerLogic.GetHtml().ToString() },
                        new Common.Helper.ParamDictionary { Key = "total", Value = assignerLogic.GetTotalAssigned().ToString() }
                    };

                    subject = string.Format(subject, assignerLogic.GetSalesDate());

                    Console.WriteLine("-------------------------------");
                    Console.WriteLine("Sending mails");

                    mailHelper.SendMail(
                            from: fromMail,
                            subject: subject,
                            body: mailHelper.BuildMessage(AppDomain.CurrentDomain.BaseDirectory + "HtmlMailTemplate\\Assigment.html", parameters),
                            smtpCredentials: new SmtpClient(smtpHost, smtpPort),
                            attachments: new List<Attachment>(),
                            mails: mailHelper.BuildMailTo(commaSeparatedMails),
                            isHtml: true,
                            UseDefaultCredentials: true
                        );

                    Console.WriteLine("...........Sending sent..........");
                }
                else
                {
                    mailHelper.SendMail(
                            from: fromMail,
                            subject: subject,
                            body: "No hay cuentas pendientes por asignar.",
                            smtpCredentials: new SmtpClient(smtpHost, smtpPort),
                            attachments: new List<Attachment>(),
                            mails: mailHelper.BuildMailTo(commaSeparatedMails),
                            UseDefaultCredentials: true
                        );
                }
            }
            catch (Exception except)
            {
                mailHelper.SendMail(
                            from: fromMail,
                            subject: subject + "-ERROR",
                            body: "Error al asignar las cuentas. Contactar a: Y.Laureano@caribemedia.com.do. [" + except.Message + "]",
                            smtpCredentials: new SmtpClient(smtpHost, smtpPort),
                            attachments: new List<Attachment>(),
                            mails: mailHelper.BuildMailTo(commaSeparatedMails),
                            UseDefaultCredentials: true
                        );

                throw except;
            }
        }
    }
}
