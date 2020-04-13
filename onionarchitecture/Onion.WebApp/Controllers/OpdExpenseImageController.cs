using Onion.Interfaces.Services;
using Onion.WebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.WebApp.Controllers
{
    public class OpdExpenseImageController : Controller
    {
        // GET: OPDEXPENSEIMAGE

        private readonly IOpdExpense_ImageService _opdExpense_ImageService;

        public OpdExpenseImageController(IOpdExpense_ImageService opdExpenseImageService)
        {
            _opdExpense_ImageService = opdExpenseImageService;

        }



        public ActionResult Index(int id, String opdType)
        {
            if (Request.IsAuthenticated)
            {
                AuthenticateUser();

                ViewData["OPDEXPENSE_ID"] = id;

                ViewData["OPDTYPE"] = opdType;


                ImgViewModel model = new ImgViewModel { FileAttach = null, ImgLst = new List<OpdExpense_ImageVM>() };

                model.ImgLst = _opdExpense_ImageService.GetOpdExpenses_ImageAgainstOpdExpenseId(Convert.ToInt32(id));

                model.OPDExpense_ID = id;
                return this.View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");

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
            // Initialization.
            //string fileContent = string.Empty;
            //string fileContentType = string.Empty;

            if (Request.IsAuthenticated)
            {
                AuthenticateUser();

                //int opdExpense_Id = Convert.ToInt32(Request.Url.Segments[3].ToString());

                //opdExpense_Image.OPDEXPENSE_ID = opdExpense_Id;

                //opdExpense_Image.OPDType = Request.Form["opdType"];

                //ViewData["OPDEXPENSE_ID"] = model.OPDExpense_ID;

                //ViewData["OPDTYPE"] = model.OPDType;

                if (ModelState.IsValid)
                {
                    // Converting to bytes.
                    byte[] uploadedFile = new byte[model.FileAttach.InputStream.Length];
                    model.FileAttach.InputStream.Read(uploadedFile, 0, uploadedFile.Length);

                    OpdExpense_ImageVM opdExpense_Image = new OpdExpense_ImageVM();


                // Initialization.
                    opdExpense_Image.OPDEXPENSE_ID = model.OPDExpense_ID;
                    opdExpense_Image.IMAGE_BASE64 = Convert.ToBase64String(uploadedFile);
                    opdExpense_Image.IMAGE_EXT = model.FileAttach.ContentType;                  
                    opdExpense_Image.CreatedDate = DateTime.Now;
                    opdExpense_Image.IMAGE_NAME = model.FileAttach.FileName;
                    opdExpense_Image.NAME_EXPENSES = model.ExpenseName;
                    opdExpense_Image.EXPENSE_AMOUNT = model.ExpenseAmount;
                    OpdExpense_ImageVM OpdExpensePatient_Obj = _opdExpense_ImageService.CreateOpdExpense_Image(opdExpense_Image);
               
                //// Settings.
                model.ImgLst = _opdExpense_ImageService.GetOpdExpenses_ImageAgainstOpdExpenseId(Convert.ToInt32(model.OPDExpense_ID));

                // Info
                return this.View(model);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
            return View();
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
           
            // try
            // {
            // Loading dile info.
             var fileInfo = _opdExpense_ImageService.GetOpdExpensesImagesAgainstId(fileId); 

            // Info.
             return this.GetFile(fileInfo.IMAGE_BASE64, fileInfo.IMAGE_EXT);
            //}
            //catch (Exception ex)
            //{
            // Info
            //Console.Write(ex);
            //}

            // Info.
            //return this.View(model);

            //return View();
        }


        // POST: OPDEXPENSEIMAGE/Delete/5
        public ActionResult Delete(int id , int opdexpenseid, string opdType)
        {

            if (Request.IsAuthenticated)
            {
                AuthenticateUser();

                _opdExpense_ImageService.DeleteOpdExpense_Image(id);

                ViewData["OPDTYPE"] = opdType;
                // Info.
                return RedirectToAction("Index", "OPDEXPENSEIMAGE", new { id = opdexpenseid ,opdType = opdType });
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
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

            ViewBag.RollType = managerController.AuthenticateUser();

            ViewBag.UserName = managerController.GetName();

        }



    }
}