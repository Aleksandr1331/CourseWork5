using CourseWork.Commands;
using CourseWork.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace CourseWork.ViewModel
{
    public class MainVM
    {
        public ObservableCollection<ParserData> Dates { get; set; } = [];
        public ICommand StartParsingCommand { get; set; }

        public MainVM()
        {
            StartParsingCommand = new RelayCommand(async () => await GetLinksAsync());
        }

        private async Task GetLinksAsync()
        {
            Parser parser = new("https://www.tripadvisor.ru/Hotels-g665310-Tomsk_Tomsk_Oblast_Siberian_District-Hotels.html");
            List<string> links = await parser.GetLinks();

            Dates.Clear();
            foreach (string link in links)
            {
                Dates.Add(await Parser.Parse(link));
            }
            //using (var db = new HotelsContext())
            //{
            //    foreach (string link in links)
            //    {
            //        await Parser.Parse(link);
            //    }
            //}
        }  
    }

    public class BasicVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
