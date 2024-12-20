//using Word = Microsoft.Office.Interop.Word;

//namespace CourseWork.Model
//{
//    public class COMFormatter
//    {
//        private readonly Word.Documents worddocuments;
//        public readonly Word.Document worddocument;
//        public Word.Application wordApp;

//        public COMFormatter(string template = @"C:\Games\Aрхитектура\TemplateCource.doc")
//        {
//            wordApp = new()
//            {
//                Visible = true
//            };

//            Object newTemplate = false; //Новый шаблон на основе переданного
//            Object documentType = Word.WdNewDocumentType.wdNewBlankDocument; //Тип документа
//            Object visible = true; //Видимость содержимого в окне Word

//            wordApp.Documents.Add(template, newTemplate, ref documentType, ref visible);

//            worddocuments = wordApp.Documents;
//            worddocument = worddocuments.get_Item(1);
//            worddocument.Activate();
//        }

//        public void Replace(string wordr, string replacement)
//        {
//            Word.Range range = worddocument.StoryRanges[Word.WdStoryType.wdMainTextStory];
//            range.Find.ClearFormatting();

//            range.Find.Execute(FindText: wordr, ReplaceWith: replacement);

//            TrySave();
//        }

//        public void ReplaceBookmark(string bookmark, string replacement)
//        {
//            worddocument.Bookmarks[bookmark].Range.Text = replacement;

//            TrySave();
//        }

//        public void TrySave(string outputPath = @"C:\Games\Aрхитектура\dddddd.doc")
//        {
//            try
//            {
//                worddocument.SaveAs(outputPath, Word.WdSaveFormat.wdFormatDocument);
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e.Message);
//            }
//        }

//        public void Close()
//        {
//            Object saveChanges = Word.WdSaveOptions.wdPromptToSaveChanges;
//            Object originalFormat = Word.WdOriginalFormat.wdWordDocument;
//            Object routeDocument = Type.Missing;
//            wordApp.Quit(ref saveChanges, ref originalFormat, ref routeDocument);
//        }

//    }
//}
