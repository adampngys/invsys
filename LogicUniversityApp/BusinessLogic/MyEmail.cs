using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace LogicUniversityApp
{
    public class MyEmail
    {
        /// <summary>
        /// Send an email to target location
        /// </summary>
        /// <param name="mailTo">target email address</param>
        /// <param name="mailSubject">subject of the email</param>
        /// <param name="mailContent">content of the email</param>
        /// <returns></returns>
        public static bool SendEmail(string mailTo, string mailSubject, string mailContent)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.Host = "smtp.gmail.com";
                client.Port = 587;

                // setup Smtp authentication
                System.Net.NetworkCredential credentials =
                    new System.Net.NetworkCredential("sa47team1@gmail.com", "sa47team1@gmail");
                client.UseDefaultCredentials = false;
                client.Credentials = credentials;

                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("sa47team1@gmail.com");
                msg.To.Add(new MailAddress(mailTo));

                msg.Subject = mailSubject;
                msg.IsBodyHtml = true;
                msg.Body = string.Format("<html><head></head><body><b>" + mailContent + "</b></body>");

                try
                {
                    client.Send(msg);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}