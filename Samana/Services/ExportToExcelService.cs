using Microsoft.Office.Core;
using Samana.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.InteropServices;
using Microsoft.Ajax.Utilities;
using System.IO;
using DataSource.Model;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using System.Drawing;
using Xdr = DocumentFormat.OpenXml.Drawing.Spreadsheet;
using A = DocumentFormat.OpenXml.Drawing;
using Samana.Services;
using ClosedXML.Excel;

namespace Samana.Services
{
    public class ExportToExcelService
    {
        LidService _lidService = new LidService();
        public void CreatePackage(string sFile, string imageFileName)
        {

            // Create a spreadsheet document by supplying the filepath. 
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.
                Create(sFile, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document. 
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart. 
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook. 
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
                AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook. 
            Sheet sheet = new Sheet()
            {
                Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Mentor"
            };
            sheets.Append(sheet);

            var drawingsPart = worksheetPart.AddNewPart<DrawingsPart>();

            if (!worksheetPart.Worksheet.ChildElements.OfType<Drawing>().Any())
            {
                worksheetPart.Worksheet.Append(new Drawing { Id = worksheetPart.GetIdOfPart(drawingsPart) });
            }

            if (drawingsPart.WorksheetDrawing == null)
            {
                drawingsPart.WorksheetDrawing = new WorksheetDrawing();
            }

            var worksheetDrawing = drawingsPart.WorksheetDrawing;

            var imagePart = drawingsPart.AddImagePart(ImagePartType.Png);

            using (var stream = new FileStream(imageFileName, FileMode.Open))
            {
                imagePart.FeedData(stream);
            }

            Bitmap bm = new Bitmap(imageFileName);
            DocumentFormat.OpenXml.Drawing.Extents extents = new DocumentFormat.OpenXml.Drawing.Extents();
            var extentsCx = (long)bm.Width * (long)((float)914400 / bm.HorizontalResolution);
            var extentsCy = (long)bm.Height * (long)((float)914400 / bm.VerticalResolution);
            bm.Dispose();

            var colOffset = 0;
            var rowOffset = 0;
            int colNumber = 1;
            int rowNumber = 1;

            var nvps = worksheetDrawing.Descendants<Xdr.NonVisualDrawingProperties>();
            var nvpId = nvps.Count() > 0 ?
                (UInt32Value)worksheetDrawing.Descendants<Xdr.NonVisualDrawingProperties>().Max(p => p.Id.Value) + 1 :
                1U;

            var oneCellAnchor = new Xdr.OneCellAnchor(
                new Xdr.FromMarker
                {
                    ColumnId = new Xdr.ColumnId((colNumber - 1).ToString()),
                    RowId = new Xdr.RowId((rowNumber - 1).ToString()),
                    ColumnOffset = new Xdr.ColumnOffset(colOffset.ToString()),
                    RowOffset = new Xdr.RowOffset(rowOffset.ToString())
                },
                new Xdr.Extent { Cx = extentsCx, Cy = extentsCy },
                new Xdr.Picture(
                    new Xdr.NonVisualPictureProperties(
                        new Xdr.NonVisualDrawingProperties { Id = nvpId, Name = "Picture " + nvpId, Description = imageFileName },
                        new Xdr.NonVisualPictureDrawingProperties(new A.PictureLocks { NoChangeAspect = true })
                    ),
                    new Xdr.BlipFill(
                        new A.Blip { Embed = drawingsPart.GetIdOfPart(imagePart), CompressionState = A.BlipCompressionValues.Print },
                        new A.Stretch(new A.FillRectangle())
                    ),
                    new Xdr.ShapeProperties(
                        new A.Transform2D(
                            new A.Offset { X = 0, Y = 0 },
                            new A.Extents { Cx = extentsCx, Cy = extentsCy }
                        ),
                        new A.PresetGeometry { Preset = A.ShapeTypeValues.Rectangle }
                    )
                ),
                new Xdr.ClientData()
            );

            worksheetDrawing.Append(oneCellAnchor);
            workbookpart.Workbook.Save();
            spreadsheetDocument.Close();
        }


        public void CreateMentorSheet(string excelFilePath, int? id)
        {

            XLWorkbook wb = new XLWorkbook(excelFilePath);
            var ws = wb.Worksheets.Worksheet(1);

            LidViewModel kernlid = _lidService.LidToLidViewModel(_lidService.GetLid(id));

            var daterow = ws.Row(7);
            daterow.Cell(2).SetValue<string>(Convert.ToString(DateTime.Now.ToString("MMMM") + " " + DateTime.Today.Year.ToString()));

            var kernlidrow = ws.Row(9);
            kernlidrow.Cell(2).Value = kernlid.Voornaam + " " + kernlid.Achternaam;
            kernlidrow.Style.Font.SetBold(true);

            int i = 0;
            int startdata = 10;
            foreach (var lid in kernlid.Beschermelingen)
            {
                i++;
                var row = ws.Row(startdata + i);
                row.Cell(1).Value = i;
                row.Cell(2).Value = lid.Voornaam + " " + lid.Achternaam + " (" + lid.Lidsoort.Soort + ")";
                row.Cell(3).Value = lid.Adres + " " + lid.HuisNr;

            }

            i += 2;
            var aantalpersrow = ws.Row(startdata + i);
            aantalpersrow.Cell(2).Value = kernlid.Beschermelingen.Count + " personen";

            i++;
            var aantalhuizenrow = ws.Row(startdata + i);
            aantalhuizenrow.Cell(2).Value = _lidService.AantalHuizen(kernlid.Beschermelingen) + " huizen";

            ws.Rows().Style.Font.FontSize = 14;
            ws.Columns().AdjustToContents();

            wb.SaveAs(excelFilePath);
        }
    }
}