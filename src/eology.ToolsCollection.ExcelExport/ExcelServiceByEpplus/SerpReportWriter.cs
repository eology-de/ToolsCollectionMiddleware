using System.Drawing;

using eology.SharedLibs.ExternalApis.SERanking;

using Newtonsoft.Json;

using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace eology.ToolsCollection.Features.ExcelExport
{
    public class SerpReportWriter
    {
        public static void WriteSerpData(SerpItem serpItem, ExcelPackage package)
        {
            try
            {
                //Erzeuge Sheet
                var sheet = package.Workbook.Worksheets.Add("SerpResults");

                Console.WriteLine("Schreibe in Tabelle: " + sheet.Name);

                // Schreibe HauptDaten
                WriteSheetData(serpItem.RequestName, serpItem.QueueItems.ToList(), serpItem.ResultCount, sheet);

                sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ex.ReThrow();
            }
        }

        public static void WriteSheetData(string name, List<SerpQueueItem> serpQueueItems, int serpCount, ExcelWorksheet workSheet)
        {
            Color eoBlue = Color.FromArgb(92, 196, 236);
            workSheet.Cells[1, 1, 1, serpCount + 1].Merge = true;
            using (var range = workSheet.Cells["A1"])
            {
                range.LoadFromText($"Top {serpCount} Rankings {name}");
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(eoBlue);
                range.Style.Font.Color.SetColor(Color.White);
                range.Style.Font.Bold = true;
                range.Style.Font.Size = 18;
            }

            workSheet.Cells[2, 1].LoadFromText("Keyword");

            int i = 2;
            int position = 1;
            while (i < serpCount + 2)
            {
                var range = workSheet.Cells[2, i];
                range.LoadFromText($"Position {position}");
                i++;
                position++;
            }

            workSheet.Rows[2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Rows[2].Style.Font.Bold = true;

            try
            {
                int currentRowIndex = 3;
                serpQueueItems.ForEach(queueItem =>
                {
                    List<SER_SERPResultItem>? serpResults = new();
                    if (!string.IsNullOrEmpty(queueItem.QueryResultSerialized))
                        serpResults = JsonConvert.DeserializeObject<List<SER_SERPResultItem>>(queueItem.QueryResultSerialized);

                    string csvInsertString = "";
                    csvInsertString += queueItem.Query;
                    int queueCount = 1;
                    serpResults?.ForEach(serpResult =>
                    {
                        if (queueCount <= serpCount)
                        {
                            csvInsertString += "," + serpResult.Url;
                        }
                        queueCount++;
                    });

                    workSheet.Cells[currentRowIndex, 1].LoadFromText(csvInsertString);
                    currentRowIndex++;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                ex.ReThrow();
            }
        }
    }
}
