using OfficeOpenXml;
using System.ComponentModel;


namespace NjalaUniversityAttendanceAPI.Helpers
{
    public static class ExcelHelper
    {
        public static byte[] GenerateExcel<T>(List<T> list)
        {
            // EPPlus 8+: licensing already set in Program.cs.
            // Optionally, you can repeat here:
            ExcelPackage.License.SetNonCommercialOrganization("Njala University");

            using var pkg = new ExcelPackage();
            var ws = pkg.Workbook.Worksheets.Add("Report");
            ws.Cells[1, 1].LoadFromCollection(list, true);
            return pkg.GetAsByteArray();
        }
    }
}
