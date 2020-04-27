using Onion.Common.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.WebApp.Utils
{
    public class EmailUtils
    {
        public static string GetEmailBody(OpdExpenseVM opdExpense)
        {
            string fileContent = File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailPages/ClaimForm.html"));
            fileContent = fileContent.Replace("__EmployeeName", opdExpense.EmployeeName)
                                     .Replace("__Month", opdExpense.ClaimMonth)
                                     .Replace("__Year", opdExpense.ClaimYear)
                                     .Replace("__Status", opdExpense.Status)
                                     .Replace("__Amount", opdExpense.TotalAmountClaimed.ToString());
            var PatientRows = "";
            var patientRowTeplate = @"<tr>
                <td align='center'>__sr</td>
                <td align='center'>__Name</td>
                <td align='center'>__Age</td>
                <td align='center'>__Relationship</td>
                <td align='center'>__Amount</td>
            </tr>";
            int sr = 1;
            foreach (var patient in opdExpense.OpdExpensePatients)
            {

                PatientRows += patientRowTeplate.Replace("__sr", sr.ToString())
                                                .Replace("__Name", patient.Name)
                                                .Replace("__Age", patient.Age.ToString())
                                                .Replace("__Relationship", patient.RelationshipEmployee)
                                                .Replace("__Amount", patient.OpdExpense.TotalAmountClaimed.ToString());
                sr++;
            }

            fileContent = fileContent.Replace("__PatientRows", PatientRows);

            return fileContent;
        }

        public static MailMessage GetMailMessage(OpdExpenseVM expense)
        {
            var message = new MailMessage();
            message.Body = GetEmailBody(expense);
            message.IsBodyHtml = true;
            //foreach (string to in emailMessage.To)
            //{
            //    message.To.Add(to);
            //}

            message.To.Add("mkhurramadeel@gmail.com");

            message.Subject = "testing email messaging";

            //foreach (var image in expense.OpdExpenseImages)
            //{
            //    message.Attachments.Add(new Attachment(image.ImageName));
            //}

            message.To.Add(new MailAddress("mkhurramadeel@gmail.com")); //replace with valid value
            message.Subject = "Your email subject";
            //message.Body = string.Format(body, model.FromName, model.FromEmail, model.Message);
            message.IsBodyHtml = true;

            return message;
        }
    }
}