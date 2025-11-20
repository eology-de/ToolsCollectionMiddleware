using System;
using System.Linq;
using eology.ToolsCollection.Features.ExcelExport;
using OfficeOpenXml;

namespace KeywordManagementConsole.Helpers
{
    public class ReportSpecificHelpers
    {

        public static ExcelWorksheet GetSheetByName(string sheetName, ExcelPackage package)
        {
            ExcelWorksheet sheet = package.Workbook.Worksheets.ToList().SingleOrDefault(item => item.Name == sheetName);
            if (sheet == null)
            {
                //XConsole.WriteLine("Sheet nicht gefunden: " + sheetName);
            }

            return sheet;

        }

        public static ExcelWorksheet EnsureCompetitorSheet(string sheetName, ExcelPackage package)
        {
            var compSheetName = sheetName + " - Comp";


            var worksheet = ReportSpecificHelpers.GetSheetByName(sheetName, package);
            var competitorSheet = ReportSpecificHelpers.GetSheetByName(compSheetName, package);

            if (competitorSheet == null)
            {
                package.Workbook.Worksheets.Add(compSheetName);
                competitorSheet = ReportSpecificHelpers.GetSheetByName(compSheetName, package);
                var competitorSheetPos = package.Workbook.Worksheets.ToList().IndexOf(competitorSheet);
                var workSheetPos = package.Workbook.Worksheets.ToList().IndexOf(worksheet);
                package.Workbook.Worksheets.MoveAfter(competitorSheetPos, workSheetPos + 1);
            }

            return competitorSheet;

        }

        //public static int GetSheetPositionByName(string sheetName, ExcelPackage package)
        //{
        //    ExcelWorksheet sheet = package.Workbook.Worksheets.ToList().SingleOrDefault(item => item.Name == sheetName);






        //    if (sheet == null)
        //    {
        //        XConsole.WriteLine("Sheet nicht gefunden: " + sheetName);
        //    }

        //    return sheet;

        //}


        public static ExcelWorksheet GetSheetByTypeAndName(string sheetName, ExcelPackage package, ReportType reportType)
        {

            if (reportType == ReportType.Serp)
            {
                sheetName += " - Vorlage";
            }
            return GetSheetByName(sheetName, package);
        }


        public static int GetColIndexRequestItemByReportType(ReportType reportType, ExcelWorksheet sheet, string columnName = "Keyword")
        {
            int colIndexRequestItem = -1;
            //switch (reportType)
            //{

            //    case ReportType.Keywordset:
            //        colIndexRequestItem = ColIndexHelper.GetHeaderColumnByName(columnName, ParseType.StartsWith, sheet);
            //        break;
            //    case ReportType.SEO:
            //        colIndexRequestItem = ColIndexHelper.GetHeaderColumnByName(columnName, ParseType.StartsWith, sheet);
            //        break;
            //    case ReportType.KeywordAndUrl:
            //        colIndexRequestItem = ColIndexHelper.GetHeaderColumnByName(columnName, ParseType.StartsWith, sheet);
            //        break;
            //}

            return colIndexRequestItem;
        }

    }
}
