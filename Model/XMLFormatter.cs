//using Excel = Microsoft.Office.Interop.Excel;

//namespace CourseWork.Model
//{
//    class XMLFormatter
//    {
//        public Excel.Workbook workbook;
//        public Excel.Worksheet worksheet;
//        public Excel.Application excelApp;

//        public XMLFormatter()
//        {
//            excelApp = new();

//            workbook = excelApp.Workbooks.Add();
//            worksheet = (Excel.Worksheet)workbook.Worksheets.get_Item(1);

//            excelApp.Visible = true;
//            excelApp.UserControl = true;
//        }

//        public void TrySave(string outputPath = @"C:\Games\Aрхитектура\XML.xls")
//        {
//            try
//            {
//                workbook.SaveAs(outputPath, Excel.XlFileFormat.xlOpenXMLWorkbook);
//                workbook.Saved = true;
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e.Message);
//            }
//        }

//        public void Close()
//        {
//            TrySave();

//            excelApp.Quit();
//        }
//    }

//}
