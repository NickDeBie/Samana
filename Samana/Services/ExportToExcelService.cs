using Microsoft.Office.Core;
using Samana.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.InteropServices;
using Microsoft.Ajax.Utilities;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using DataSource.Model;

namespace Samana.Services
{
    public class ExportToExcelService
    {
        private LidService _lidService = new LidService();

        public void CreateExcelApp()
        {
            var xlApp = new Excel.Application();
            Excel.Workbook wb = xlApp.Workbooks.Add();
        }
       public Excel.Worksheet CreateVerjaardagenXL(Excel.Application xlApp, Lid lid)
        {
            Excel.Workbook wb = new Excel.Workbook();
            Excel.Worksheet ws = wb.Sheets[1];
            


           

            return ws;
        }
    }
}