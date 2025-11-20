using OfficeOpenXml;

namespace eology.ToolsCollection.Features.ExcelExport
{
    public class ExcelServiceByEpplus
    {
        public static byte[] WriteExcelSerpReport(SerpItem serpItem, string fileName)
        {
            // Lets converts our object data to Datatable for a simplified logic.
            // Datatable is most easy way to deal with complex datatypes for easy reading and formatting.
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using MemoryStream memoryStream = new();
            try
            {
                using var package = new ExcelPackage(memoryStream);
                SerpReportWriter.WriteSerpData(serpItem, package);

                // Save to file
                Console.WriteLine("Schreibe: " + fileName);
                package.Save();

                //var packageAsArray = package.GetAsByteArray();

                return package.GetAsByteArray();
            }
            catch (Exception ex)
            {
                throw new Volo.Abp.UserFriendlyException(ex.Message);
            }
        }
    }
}
