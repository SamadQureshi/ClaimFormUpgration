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

        private const string HR_EMAIL_SUBJECT = "Opd Claim for HR approval";
        private const string FINANCE_EMAIL_SUBJECT = "Opd claim for Finance approval";
        private const string MANAGEMENT_EMAIL_SUBJECT = "Opd claim for Management approval";

        private const string HR_EMAIL_KEY = "HR:List";
        private const string FINANCE_EMAIL_KEY = "FIN:List";
        private const string MANAGEMENT_EMAIL_KEY = "MAN:List";

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
            </tr>";
            int sr = 1;
            foreach (var patient in opdExpense.OpdExpensePatients)
            {

                PatientRows += patientRowTeplate.Replace("__sr", sr.ToString())
                                                .Replace("__Name", patient.Name)
                                                .Replace("__Age", patient.Age.ToString())
                                                .Replace("__Relationship", patient.RelationshipEmployee);
                sr++;
            }

            fileContent = fileContent.Replace("__PatientRows", PatientRows);

            string commentsTemplate = "<h2>__department</h2><strong>__FromName</strong><p>__comments</p>";
            if(!string.IsNullOrEmpty(opdExpense.HrComment))
            {
                fileContent = fileContent.Replace("__HRComments", 
                    commentsTemplate.Replace("__department", "HR")
                   .Replace("__FromName", opdExpense.HrName)
                   .Replace("__comments", opdExpense.HrComment)
                    );
            }
            else
            {
                fileContent = fileContent.Replace("__HRComments", "");
            }

            if (!string.IsNullOrEmpty(opdExpense.FinanceComment))
            {
                fileContent = fileContent.Replace("__FinanceComments",
                    commentsTemplate.Replace("__department", "Finance")
                   .Replace("__FromName", opdExpense.FinanceName)
                   .Replace("__comments", opdExpense.FinanceComment)
                    );
            }
            else
            {
                fileContent = fileContent.Replace("__FinanceComments", "");
            }


            if (!string.IsNullOrEmpty(opdExpense.ManagementComment))
            {
                fileContent = fileContent.Replace("__ManagerComments",
                    commentsTemplate.Replace("__department", "Management")
                   .Replace("__FromName", opdExpense.ManagementName)
                   .Replace("__comments", opdExpense.ManagementComment)
                    );
            }
            else
            {
                fileContent = fileContent.Replace("__ManagerComments", "");
            }


            return fileContent;
        }

        public static MailMessage GetMailMessage(OpdExpenseVM expense)
        {
            var message = new MailMessage();
            message.Body = GetEmailBody(expense);
            message.IsBodyHtml = true;
            string toEmail = "";
            if (expense.Status == ClaimStatus.SUBMITTED)
            {
                toEmail = ConfigUtil.GetConfigValue(HR_EMAIL_KEY);
                toEmail = "mkhurramadeel@gmail.com";
                message.Subject = HR_EMAIL_SUBJECT;
                message.To.Add(new MailAddress(toEmail)); //replace with valid value
            }
            else if (expense.Status == ClaimStatus.HRAPPROVED)
            {
                toEmail = ConfigUtil.GetConfigValue(FINANCE_EMAIL_KEY);
                toEmail = "mkhurramadeel@gmail.com";

                message.Subject = FINANCE_EMAIL_SUBJECT;
                message.To.Add(new MailAddress(toEmail)); //replace with valid value
            }
            else if (expense.Status == ClaimStatus.FINAPPROVED )
            {
                toEmail = ConfigUtil.GetConfigValue(MANAGEMENT_EMAIL_KEY);
                message.Subject = MANAGEMENT_EMAIL_SUBJECT;
                message.To.Add(new MailAddress(toEmail)); //replace with valid value
            }
            else if (expense.Status == ClaimStatus.MANGAPPROVED)
            {
                toEmail = ConfigUtil.GetConfigValue(MANAGEMENT_EMAIL_KEY);
                toEmail = "mkhurramadeel@gmail.com";
                message.Subject = MANAGEMENT_EMAIL_SUBJECT;
                message.To.Add(new MailAddress(toEmail)); //replace with valid value
            }
            else if (expense.Status == ClaimStatus.MANAPPROVED)
            {
                toEmail = ConfigUtil.GetConfigValue(MANAGEMENT_EMAIL_KEY);
                toEmail = "mkhurramadeel@gmail.com";
                message.Subject = MANAGEMENT_EMAIL_SUBJECT;
                message.To.Add(new MailAddress(toEmail)); //replace with valid value
            }


            return message;
        }
    }
}