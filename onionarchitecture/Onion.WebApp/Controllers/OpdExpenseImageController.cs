using NLog;
using Onion.Interfaces.Services;
using Onion.WebApp.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TCO.TFM.WDMS.Common.Utils;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.WebApp.Controllers
{
    public class OpdExpenseImageController : Controller
    {
        // GET: OPDEXPENSEIMAGE

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IOpdExpenseImageService _opdExpenseImageService;
        private readonly IOpdExpenseService _opdExpenseService;
        private const string UrlIndex = "Index";
        private const string UrlHome = "Home";
        public OpdExpenseImageController(IOpdExpenseImageService opdExpenseImageService,IOpdExpenseService opdExpenseService)
        {
            _opdExpenseImageService = opdExpenseImageService;

            _opdExpenseService = opdExpenseService;

        }



        public ActionResult Index(string id)
        {
            if (Request.IsAuthenticated)
            {
                AuthenticateUser();

                int idDecrypted = Security.DecryptId(Convert.ToString(id));

                var opdExpenseService = _opdExpenseService.GetOpdExpensesAgainstId(Convert.ToInt32(idDecrypted));

                ViewData["OPDTYPE"] = opdExpenseService.OpdType;
                ViewData["OPDEXPENSE_ID"] = idDecrypted;

                ImgViewModel model = new ImgViewModel { FileAttach = null, ImgLst = new List<OpdExpenseImageVM>() };

                model.ImgLst = _opdExpenseImageService.GetOpdExpensesImageAgainstOpdExpenseId(Convert.ToInt32(idDecrypted));

                model.OPDExpenseID = idDecrypted;
                return this.View(model);
            }
            else
            {
                return RedirectToAction(UrlIndex, UrlHome);

            }

        }




        #region POST: /Img/Index

        /// <summary>
        /// POST: /Img/Index
        /// </summary>
        /// <param name="model">Model parameter</param>
        /// <returns>Return - Response information</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ImgViewModel model)
        {

            if (Request.IsAuthenticated)
            {
                AuthenticateUser();              

                if (ModelState.IsValid)
                {
                    // Converting to bytes.
                    byte[] uploadedFile = new byte[model.FileAttach.InputStream.Length];
                    model.FileAttach.InputStream.Read(uploadedFile, 0, uploadedFile.Length);

                    OpdExpenseImageVM opdExpense_Image = new OpdExpenseImageVM();

                    ViewData["OPDTYPE"] = model.OPDType;      

                    if (model.OPDExpenseID == 0)
                    {
                        model.OPDExpenseID = Security.DecryptId(Request.Url.Segments[3].ToString());
                    }

                    string emailAddress = GetEmailAddress();
                    ViewData["OPDEXPENSE_ID"] = model.OPDExpenseID;
                    // Initialization.
                    opdExpense_Image.OpdExpenseId = model.OPDExpenseID;
                    opdExpense_Image.ImageBase64 = Convert.ToBase64String(uploadedFile);
                    opdExpense_Image.ImageExt = model.FileAttach.ContentType;                  
                    opdExpense_Image.CreatedDate = DateTime.Now;
                    opdExpense_Image.ImageName = model.FileAttach.FileName;
                    opdExpense_Image.NameExpenses = model.ExpenseName;
                    opdExpense_Image.ExpenseAmount = model.ExpenseAmount;
                    OpdExpenseImageVM OpdExpensePatient_Obj = _opdExpenseImageService.CreateOpdExpenseImage(opdExpense_Image, emailAddress);

                    ImgViewModel modelUploaded = new ImgViewModel { FileAttach = null, ImgLst = new List<OpdExpenseImageVM>() };
                    ModelState.Clear();                  
                    modelUploaded.ImgLst = _opdExpenseImageService.GetOpdExpensesImageAgainstOpdExpenseId(Convert.ToInt32(model.OPDExpenseID));                    
               
                return this.View(modelUploaded);
                }
                else
                {                  

                    if (model.OPDExpenseID == 0) {
                        model.OPDExpenseID = Security.DecryptId(Request.Url.Segments[3].ToString());
                            }                  
                    model.ImgLst = _opdExpenseImageService.GetOpdExpensesImageAgainstOpdExpenseId(Convert.ToInt32(model.OPDExpenseID));

                    // Info
                    return this.View(model);
                }
            }
            else
            {
                return RedirectToAction(UrlIndex, UrlHome);

            }            
        }

        #endregion


        #region Download file methods

        #region GET: /Img/DownloadFile

        /// <summary>
        /// GET: /Img/DownloadFile
        /// </summary>
        /// <param name="fileId">File Id parameter</param>
        /// <returns>Return download file</returns>
        public ActionResult DownloadFile(int fileId)
        {          
         
             var fileInfo = _opdExpenseImageService.GetOpdExpensesImagesAgainstId(fileId); 

             return this.GetFile(fileInfo.ImageBase64, fileInfo.ImageExt);
         
        }


        // POST: OPDEXPENSEIMAGE/Delete/5
        public ActionResult Delete(int id , int opdexpenseid)
        {

            if (Request.IsAuthenticated)
            {
                AuthenticateUser();
                string emailAddress = GetEmailAddress();

                _opdExpenseImageService.DeleteOpdExpenseImage(id, emailAddress);
            
                // Info.
                return RedirectToAction(UrlIndex, "OPDEXPENSEIMAGE", new { id = Security.EncryptId(opdexpenseid)});
            }
            else
            {
                return RedirectToAction(UrlIndex, UrlHome);

            }
        }

        [HttpPost]
        public ActionResult DeleteOPDEXPENSEIMAGE(int id)
        {
            try
            {

                string emailAddress = GetEmailAddress();
                // Loading dile info.
                _opdExpenseImageService.DeleteOpdExpenseImage(id, emailAddress);
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }

            // Info.
            return new EmptyResult();
        }
        #endregion

        #endregion

        #region Helpers

        #region Get file method.

        /// <summary>
        /// Get file method.
        /// </summary>
        /// <param name="fileContent">File content parameter.</param>
        /// <param name="fileContentType">File content type parameter</param>
        /// <returns>Returns - File.</returns>
        private FileResult GetFile(string fileContent, string fileContentType)
        {
            // Initialization.
            FileResult file;
            try
            {
                // Get file.
                byte[] byteContent = Convert.FromBase64String(fileContent);
                file = this.File(byteContent, fileContentType);
            }
            catch (Exception ex)
            {
                // Info.
                throw ex;
            }

            // info.
            return file;
        }

        #endregion

        #endregion
        private void AuthenticateUser()
        {
            OfficeManagerController managerController = new OfficeManagerController();        

            UserAuthorization user = new UserAuthorization(_opdExpenseService);

            string userRoll = user.AuthenticateUser();

            if (user.ValidateEmailAddressManagerTravelApproval())
            {
                ViewBag.RollTypeTravel = "MANTRAVEL";
            }
          
            ViewBag.RollType = userRoll;
           

            ViewBag.UserName = managerController.GetName();

        }     
        private string GetEmailAddress()
        {
            OfficeManagerController managerController = new OfficeManagerController();
            string emailAddress = managerController.GetEmailAddress();

            return emailAddress;

        }

    }
}