using DataSource.Model;
using Samana.Services;
using Samana.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using Microsoft.Ajax.Utilities;
using System;
using System.Linq;
using System.Reflection;

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
            return View(_ledenViewModel);
        }

        public void GeneralSetup()
        {
            foreach (var pers in _noodPersoonService.GetAlleNoodPersonen())
            {
                _ledenViewModel.Leden.Find(m => m.Id == pers.LidId).NoodPersonen.Add(pers);
            }
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
            return View(_ledenViewModel);
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
            var xlApp = new Excel.Application();
            Excel.Workbook wb = xlApp.Workbooks.Add();
            Excel.Worksheet ws = wb.Sheets[1];

            _exportToExcelService.CreateExcelApp();

            ws.Shapes.AddPicture(@"c:\Users\Nick\Google Drive\Projects\Samana\Samana\Img\samana.png", MsoTriState.msoFalse, MsoTriState.msoCTrue, 0, 0, 195, 71);

            Lid lid = _lidService.GetLid(id);
            int teller = 1;
            int row = 7;

            var style = ws.Application.ActiveWorkbook.Styles.Add("bold");
            style.Font.Bold = true;
            ((Excel.Range)ws.Cells[row, 2]).Style = "bold";
            ws.Cells[row, 2] = DateTime.Now.Year + " " + DateTime.Now.ToString("MMMM");
            ws.Cells[row + 2, 2] = lid.Achternaam + " " + lid.Voornaam;

            int persTeller = 1;
            ws.Cells[row + 4, 1] = persTeller;
            ws.Cells[row + 4, 2] = lid.Achternaam + " " + lid.Voornaam + " (" + _lidService.GetLidSoort(lid.LidsoortId) + ")";
            ws.Cells[row + 4, 4] = lid.Adres + " " + lid.HuisNr;
            teller += 3;
            foreach (var pers in lid.Beschermelingen)
            {
                teller++;
                persTeller++;
                ws.Cells[row + teller, 1] = persTeller;
                ws.Cells[row + teller, 2] = pers.Achternaam + " " + pers.Voornaam + " (" + _lidService.GetLidSoort(pers.LidsoortId) + ")";
                ws.Cells[row + teller, 4] = pers.Adres + " " + pers.HuisNr;
            }
            teller += 2;
            ws.Cells[row + teller, 2] = lid.Beschermelingen.Count + 1 + " personen";
            teller += 1;
            
            ws.Cells[row + teller, 2] = lid.Beschermelingen.Count + 1 + " huizen";
            ws.Columns.Font.Size = "16";
            ws.Columns.AutoFit();
            


            Excel.Worksheet ws2 = wb.Sheets.Add() as Excel.Worksheet;
            
            ws2.Cells[1, 2] = "Verjaardagen";
            ((Excel.Range)ws2.Cells[1, 1]).Style = "bold";
            int wsRij = 3;
            foreach (var pers in lid.Beschermelingen.OrderBy(o => o.GeboorteDatum))
            {
                ws2.Cells[wsRij, 1] = wsRij - 2;
                ws2.Cells[wsRij, 2] = pers.Achternaam + " " + pers.Voornaam + " (" + _lidService.GetLidSoort(pers.LidsoortId) + ")";
                ws2.Cells[wsRij, 3] = pers.GeboorteDatum;
                ws2.Cells[wsRij, 4] = _lidService.BerekenLeeftijd(pers.GeboorteDatum) + " jaar";
                wsRij++;
            }
            ws2.Columns.Font.Size = "16";
            ws2.Columns.AutoFit();
            
            var filename = xlApp.GetSaveAsFilename("test.xls", "Excel 2000-2003 Workbook (*.xls), *.xls");
            if (!(filename is bool))
            {
                wb.SaveAs(filename, Excel.XlFileFormat.xlWorkbookNormal);
                wb.Close();
                Marshal.FinalReleaseComObject(wb);
                xlApp.Quit();
            }

            

            GeneralSetup();
            return RedirectToAction("Index");
        }

    }
}