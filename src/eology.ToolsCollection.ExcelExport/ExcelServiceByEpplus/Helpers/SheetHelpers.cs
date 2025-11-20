using System;
using System.Linq;
using OfficeOpenXml;

namespace KeywordManagementConsole
{
    public class SheetHelpers
    {
        public static int GetLastRowWithContent(ExcelWorksheet sheet)
        {
            return sheet.Cells.Where(cell => !string.IsNullOrEmpty(cell.Value?.ToString() ?? string.Empty)).LastOrDefault().End.Row;
        }
    }
}
