using DataSource.Model;
using Samana.Services;
using Samana.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;
using System;
using System.Linq;
using ClosedXML.Excel;
using System.Data;
using System.IO;
using DataSource;
using System.Net;

namespace Samana.Controllers
{
    public class LedenController : Controller
    {
        private LedenViewModel _ledenViewModel = new LedenViewModel();
        private LidService _lidService = new LidService();
        private NoodPersoonService _noodPersoonService = new NoodPersoonService();
        private ExportToExcelService _exportToExcelService = new ExportToExcelService();
        // GET: Leden
        public ActionResult Index()
        {
            GeneralSetup();
            SetInfo();

            return View(_ledenViewModel);
        }

      
        [HttpPost]
        public ActionResult Index(int? soortId, string tekst, string tekstParam, string sortParam)
        {
            if (soortId == null && tekst == null && sortParam == null)
            {
                _ledenViewModel.Leden = _lidService.LedenToViewModelList(_lidService.GetAlleLeden());
            }
            else
            {
                //zoeken op lidsoort
                if (soortId != null)
                {
                    if (soortId == 0)
                    {
                        _ledenViewModel.Leden = _lidService.LedenToViewModelList(_lidService.GetAlleLeden());
                    }
                    else
                    {
                        _ledenViewModel.Leden = _lidService.LedenToViewModelList(_lidService.GetLedenPerSoort(soortId));
                    }
                }

                //zoeken op naam
                if (tekst != null && tekstParam != null)
                {
                    switch (tekstParam)
                    {
                        case "Voornaam":
                            _ledenViewModel.Leden = _lidService.LedenToViewModelList(_lidService.GetLedenOpVoorNaam(tekst));
                            break;
                        case "Achternaam":
                            _ledenViewModel.Leden = _lidService.LedenToViewModelList(_lidService.GetLedenOpAchterNaam(tekst));
                            break;
                    }
                }
            }
            foreach (var pers in _noodPersoonService.GetAlleNoodPersonen())
            {
                LidViewModel lid = _ledenViewModel.Leden.Find(m => m.Id == pers.LidId);
                if (lid != null)
                {
                    lid.NoodPersonen.Add(pers);
                }
            }

            SetInfo();

            return View(_ledenViewModel);
        }

        public void GeneralSetup()
        {
            
            foreach (var pers in _noodPersoonService.GetAlleNoodPersonen())
            {
                if(_ledenViewModel.Leden.Find(m=>m.Id == pers.LidId) != null){
                    _ledenViewModel.Leden.Find(m => m.Id == pers.LidId).NoodPersonen.Add(pers);
                }
                
            }
        }

        public void SetInfo()
        {
            LidViewModel oudstelid = _lidService.GetOudsteLid(_ledenViewModel.Leden);
            TempData["oudstelid"] = oudstelid.Voornaam + " " + oudstelid.Achternaam + " (" + oudstelid.Leeftijd + " jaar)";

            LidViewModel jongstelid = _lidService.GetjongsteLid(_ledenViewModel.Leden);
            TempData["jongstelid"] = jongstelid.Voornaam + " " + jongstelid.Achternaam + " (" + jongstelid.Leeftijd + " jaar)";

            TempData["aantalkernleden"] = _ledenViewModel.Leden.Count(m => m.LidsoortId == 1);
            TempData["aantalmedewerkers"] = _ledenViewModel.Leden.Count(m => m.LidsoortId == 2);
            TempData["aantalgewoneleden"] = _ledenViewModel.Leden.Count(m => m.LidsoortId == 3);
        }

        public ActionResult Create()
        {
            _ledenViewModel.Leden = new List<LidViewModel>();
            _ledenViewModel.Leden.Add(new LidViewModel());
            return View(_ledenViewModel);
        }

        [HttpPost]
        public ActionResult Create(LidViewModel lidViewModel)
        {
            _ledenViewModel.Leden = new List<LidViewModel>();
            _ledenViewModel.Leden.Add(lidViewModel);
            if (ModelState.IsValid)
            {
                _lidService.SaveNewLidViewModelToDb(lidViewModel);
                return RedirectToAction("Index");
            }
            else
            {
                return View(_ledenViewModel);
            }
        }
        public ActionResult Edit(int? id)
        {
            LidViewModel lidviewModel = _ledenViewModel.Leden.Find(m => m.Id == id);
            _ledenViewModel.Leden = new List<LidViewModel>();
            _ledenViewModel.Leden.Add(lidviewModel);
            return View(_ledenViewModel);
        }


        [HttpPost]
        public ActionResult Edit(LidViewModel lidViewModel)
        {
            _ledenViewModel.Leden = new List<LidViewModel>();
            _ledenViewModel.Leden.Add(lidViewModel);
            if (ModelState.IsValid)
            {
                _lidService.SaveLidViewModelToDb(lidViewModel);
                return RedirectToAction("Index");
            }
            else
            {
                return View(_ledenViewModel);
            }
        }

        public ActionResult Delete(int? id)
        {
            LidViewModel lidViewModel = _ledenViewModel.Leden.Find(m => m.Id == id);
            return View(lidViewModel);
        }

        [HttpPost]
        public ActionResult Delete(LidViewModel lidViewModel)
        {
            _lidService.deleteLidViewModelFromDb(lidViewModel.Id);
            return RedirectToAction("Index");
        }

        public ActionResult AddNoodPersoon(int? id)
        {
            LidViewModel lidViewModel = _ledenViewModel.Leden.Find(m => m.Id == id);
            NoodPersoon noodpersoon = new NoodPersoon();
            noodpersoon.Id = _noodPersoonService.GetMaxId() + 1;
            noodpersoon.LidId = lidViewModel.Id;
            Session["Lid"] = lidViewModel;
            return View(noodpersoon);
        }


        [HttpPost]
        public ActionResult AddNoodPersoon(NoodPersoon pers)
        {

            if (ModelState.IsValid)
            {
                pers.LidId = (Session["Lid"] as LidViewModel).Id;
                _noodPersoonService.SaveNoodPersoon(pers);
                return RedirectToAction("Index");
            }
            else
            {
                return View(pers);
            }
        }

        [HttpPost]
        public ActionResult RemoveNoodPersoon(int? id)
        {
            _noodPersoonService.RemoveNoodPersoon(id);
            GeneralSetup();
            return View(_ledenViewModel);
        }

        public ActionResult SelectMentor(int? id)
        {
            _ledenViewModel.Leden = new List<LidViewModel>();
            LidViewModel lidViewModel = _lidService.LidToLidViewModel(_lidService.GetLid(id));
            _ledenViewModel.Leden.Add(lidViewModel);
            Session["Lid"] = lidViewModel;
            return View(_ledenViewModel);
        }

        [HttpPost]
        public ActionResult SelectMentor(int mentorId)
        {
            LidViewModel lidViewModel = Session["Lid"] as LidViewModel;
            if (ModelState.IsValid)
            {

                lidViewModel.MentorId = mentorId;
                _lidService.SaveLidViewModelToDb(lidViewModel);
                return RedirectToAction("Index");
            }
            else
            {
                return View(lidViewModel.Id);
            }
        }

        public ActionResult MentorToExcel(int? id)
        {
            string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            //string excelFilePath = @"C:\Users\Nick\Google Drive\Projects\Samana\Samana\Files\mentor_verjaardagen.xlsx";
            //string picPath = @"C:\Users\Nick\Google Drive\Projects\Samana\Samana\Img\samana_meerhout.png";

            string excelFilePath = @"C:\inetpub\wwwroot\Samana\files\mentor_verjaardagen.xlsx";
            string picPath = @"C:\inetpub\wwwroot\Samana\Img\samana_meerhout.png";

            _exportToExcelService.CreatePackage(excelFilePath, picPath);
            _exportToExcelService.CreateMentorSheet(excelFilePath, id);


            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachement;filename= mentoren_verjaardagen.xlsx");

            XLWorkbook wb = new XLWorkbook(excelFilePath);

            using (MemoryStream mStream = new MemoryStream())
            {
                wb.SaveAs(mStream);
                mStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }

            GeneralSetup();
            return RedirectToAction("Index");
        }

       

       


    }
}