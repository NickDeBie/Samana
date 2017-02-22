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
using Samana.Models;

namespace Samana.Controllers
{
    public class LedenController : Controller
    {
        private LedenViewModel _ledenViewModel = new LedenViewModel();
        private LidService _lidService = new LidService();
        private NoodPersoonService _noodPersoonService = new NoodPersoonService();
        private ExportToExcelService _exportToExcelService = new ExportToExcelService();
        private int limit = 5;
        private Models.SearchOption _searchOption = new Models.SearchOption();
        // GET: Leden
        public ActionResult Index(int? id)
        {
            GeneralSetup();            
            if (Request.IsAjaxRequest())
            {
                _searchOption = SetSearchOption();               
                _searchOption.PageId = id ?? 0;
                _ledenViewModel.Leden = SetLeden(_searchOption);                
                return PartialView("~/Views/Partials/LedenData.cshtml",_ledenViewModel);
            }
            else
            {
                Session["searchoption"] = null;
                _searchOption = SetSearchOption();
                SetInfo(_lidService.LedenToViewModelList(_lidService.GetAlleLeden()));
            }

            return View(_ledenViewModel);
        }
        public Models.SearchOption SetSearchOption()
        {
            if(Session["searchoption"] == null)
            {
                _searchOption.Option = SearchOptionEnum.AlleLeden.ToString();
                Session["searchoption"] = _searchOption;
            }

            return Session["searchoption"] as Models.SearchOption;
        }
        

        [HttpPost]
        public ActionResult Index(int? id, int? soortId, string tekst, string tekstParam, string sortParam)
        {
            _searchOption.PageId = id;

            //zoeken op lidsoort
            if (soortId != null)
            {
                if (soortId == 0)
                {
                    _searchOption.Option = SearchOptionEnum.AlleLeden.ToString();
                }
                else
                {
                    _searchOption.Option = SearchOptionEnum.Lidsoort.ToString();
                    _searchOption.IntValue = soortId;
                }
            }

            //zoeken op naam
            else if (tekst != null && tekstParam != null)
            {
                if (tekstParam == "Voornaam") {
                    _searchOption.Option = SearchOptionEnum.Voornaam.ToString();
                }
                if(tekstParam == "Achternaam")
                {
                    _searchOption.Option = SearchOptionEnum.Achternaam.ToString();
                }
                _searchOption.StringValue1 = tekst;
            }
            else
            {
                _searchOption.Option = SearchOptionEnum.AlleLeden.ToString();
            }



            foreach (var pers in _noodPersoonService.GetAlleNoodPersonen())
            {
                LidViewModel lid = _ledenViewModel.Leden.Find(m => m.Id == pers.LidId);
                if (lid != null)
                {
                    lid.NoodPersonen.Add(pers);
                }
            }           

            Session["searchoption"] = _searchOption;
            
            _ledenViewModel.Leden = SetLeden(_searchOption);
            return View(_ledenViewModel);
        }

        public List<LidViewModel> SetLeden(Models.SearchOption searchOption)
        {
            int page = Convert.ToInt16(searchOption.PageId) * limit;
            List<LidViewModel> ledenlist = new List<LidViewModel>();
            switch ((SearchOptionEnum)Enum.Parse(typeof(SearchOptionEnum), searchOption.Option))
            {
                case SearchOptionEnum.AlleLeden:
                    _ledenViewModel.Leden = _lidService.LedenToViewModelList(_lidService.GetAlleLedenByStep(limit, page));
                    ledenlist = _lidService.LedenToViewModelList(_lidService.GetAlleLeden());
                    SetInfo(ledenlist);
                    break;
                case SearchOptionEnum.Lidsoort:
                    _ledenViewModel.Leden = _lidService.LedenToViewModelList(_lidService.GetLedenPerSoortByStep(_searchOption.IntValue, limit, page));
                    ledenlist = _lidService.LedenToViewModelList(_lidService.GetLedenPerSoort(_searchOption.IntValue));
                    SetInfo(ledenlist);
                    break;
                case SearchOptionEnum.Voornaam:
                    _ledenViewModel.Leden = _lidService.LedenToViewModelList(_lidService.GetLedenOpVoorNaamByStep(_searchOption.StringValue1, limit, page));
                    ledenlist = _lidService.LedenToViewModelList(_lidService.GetLedenOpVoorNaam(_searchOption.StringValue1));
                    SetInfo(ledenlist);
                    break;
                case SearchOptionEnum.Achternaam:
                    _ledenViewModel.Leden = _lidService.LedenToViewModelList(_lidService.GetLedenOpAchterNaamByStep(_searchOption.StringValue1, limit, page));
                    ledenlist = _lidService.LedenToViewModelList(_lidService.GetLedenOpAchterNaam(_searchOption.StringValue1));
                    SetInfo(ledenlist);
                    break;
            }
            return _ledenViewModel.Leden;
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

        public void SetInfo(List<LidViewModel> ledenlist)
        {

            LidViewModel oudstelid = _lidService.GetOudsteLid(ledenlist);
            TempData["oudstelid"] = oudstelid.Voornaam + " " + oudstelid.Achternaam + " (" + oudstelid.Leeftijd + " jaar)";

            LidViewModel jongstelid = _lidService.GetjongsteLid(ledenlist);
            TempData["jongstelid"] = jongstelid.Voornaam + " " + jongstelid.Achternaam + " (" + jongstelid.Leeftijd + " jaar)";

            TempData["aantalLeden"] = ledenlist.Count();
            TempData["aantalkernleden"] = ledenlist.Count(m => m.LidsoortId == 1);
            TempData["aantalmedewerkers"] = ledenlist.Count(m => m.LidsoortId == 2);
            TempData["aantalgewoneleden"] = ledenlist.Count(m => m.LidsoortId == 3);
            TempData["aantalmannen"] = ledenlist.Count(m => m.Geslacht == DataSource.Enums.Geslacht.man);
            TempData["aantalvrouwen"] = ledenlist.Count(m => m.Geslacht == DataSource.Enums.Geslacht.vrouw);

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

            string excelFilePath = @"C:\Users\Nick\Google Drive\Projects\Samana\Samana\Files\mentor_verjaardagen.xlsx";
            string picPath = @"C:\Users\Nick\Google Drive\Projects\Samana\Samana\Img\samana_meerhout.png";

            //string excelFilePath = @"C:\inetpub\wwwroot\Samana\files\mentor_verjaardagen.xlsx";
            //string picPath = @"C:\inetpub\wwwroot\Samana\Img\samana_meerhout.png";

            _exportToExcelService.CreatePackage(excelFilePath, picPath);
            _exportToExcelService.CreateMentorSheet(excelFilePath, id);
            _exportToExcelService.CreateVerjaardagen(excelFilePath, id);

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