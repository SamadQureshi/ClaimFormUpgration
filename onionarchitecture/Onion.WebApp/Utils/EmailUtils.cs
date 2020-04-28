using Onion.Common.Constants;
using Onion.WebApp.Controllers;
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
            if (!string.IsNullOrEmpty(opdExpense.HrComment))
            {
                fileContent = fileContent.Replace("__CommentsHeading", "<h3>Comments</h3>");

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
                fileContent = fileContent.Replace("__CommentsHeading", "<h3>Comments</h3>");
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
                fileContent = fileContent.Replace("__CommentsHeading", "<h3>Comments</h3>");
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

            fileContent = fileContent.Replace("__CommentsHeading", "");


            return fileContent;
        }

        public static MailMessage GetMailMessage(OpdExpenseVM expense)
        {
            string HrEmailSubject = "Opd Claim for HR approval";
            string FinanceEmailSubject = "Opd claim for Finance approval";
            string ManagementEmailSubject = "Opd claim for Management approval";
            string FinanceRejectedSubject = "Opd claim rejected by finance";
            string HrRejectedSubject = "Opd claim rejected by HR";
            string EmployeeApprovedSubject = "opd claim approved";
            string ManagementRejectedSubject = "Opd claim rejected by management";

            string HrEmailKey = "HR:List";
            string FinanceEmailKey = "FIN:List";
            string ManagementEmailKey = "MAN:List";



            OfficeManagerController managerController = new OfficeManagerController();
            string RoleType = managerController.AuthenticateUser();

            var message = new MailMessage();
            message.Body = GetEmailBody(expense);
            message.IsBodyHtml = true;
            string toEmail = "";
            if (expense.Status == ClaimStatus.SUBMITTED)
            {
                toEmail = ConfigUtil.GetConfigValue(HrEmailKey);
                message.Subject = HrEmailSubject;
                message.To.Add(new MailAddress(toEmail));
            }
            else if (expense.Status == ClaimStatus.HRAPPROVED)
            {
                toEmail = ConfigUtil.GetConfigValue(FinanceEmailKey);
                message.Subject = FinanceEmailSubject;
                message.To.Add(new MailAddress(toEmail));
            }
            else if (expense.Status == ClaimStatus.HRREJECTED)
            {
                toEmail = expense.EmployeeEmailAddress;
                message.Subject = HrRejectedSubject;
                message.To.Add(new MailAddress(toEmail));
            }
            else if (expense.Status == ClaimStatus.FINAPPROVED)
            {
                toEmail = ConfigUtil.GetConfigValue(ManagementEmailKey);
                message.Subject = ManagementEmailSubject;
                message.To.Add(new MailAddress(toEmail));
            }
            else if (expense.Status == ClaimStatus.FINREJECTED)
            {
                toEmail = ConfigUtil.GetConfigValue(HrEmailKey);
                message.Subject = FinanceRejectedSubject;
                message.To.Add(new MailAddress(toEmail));
            }
            else if (expense.Status == ClaimStatus.MANAPPROVED)
            {
                toEmail = expense.EmployeeEmailAddress;
                message.Subject = EmployeeApprovedSubject;
                message.To.Add(new MailAddress(toEmail));
            }
            else if (RoleType == "HR" && expense.Status == ClaimStatus.MANINPROCESS)
            {
                toEmail = ConfigUtil.GetConfigValue(ManagementEmailKey);
                message.Subject = ManagementEmailSubject;
                message.To.Add(new MailAddress(toEmail));
            }
            else if (expense.Status == ClaimStatus.MANREJECTED)
            {
                toEmail = ConfigUtil.GetConfigValue(HrEmailKey);
                if (expense.FinanceApproval.HasValue && expense.FinanceApproval.Value)
                    toEmail = ConfigUtil.GetConfigValue(FinanceEmailKey);

                message.Subject = ManagementRejectedSubject;
                message.To.Add(new MailAddress(toEmail));
            }
            return message;
        }
    }
}