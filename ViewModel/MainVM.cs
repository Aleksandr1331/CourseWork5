using CourseWork.Commands;
using CourseWork.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace CourseWork.ViewModel
{
    public class MainVM : BasicVM
    {
        public ObservableCollection<ParserData> Datas { get; set; } = [];

        public List<string> MatrixTypes { get; set; } = [];

        public string ChartCount { get; set; } = "0";
        public List<string> ChartTypes { get; set; } = [];

        public ICommand StartParsingCommand { get; set; }
        public ICommand UploadDataCommand { get; set; }
        public ICommand DeletePhoneCommand { get; set; }
        public ICommand DeleteCameraCommand { get; set; }
        public ICommand DeleteEquipCommand { get; set; }
        public ICommand UpdateDataCommand { get; set; }

        public ParserData? Selected { get; set; }

        public MainVM()
        {
            StartParsingCommand = new RelayCommand(StartParser);
            UploadDataCommand = new RelayCommand(UploadData);
            UpdateDataCommand = new RelayCommand(UploadData);
            DeletePhoneCommand = new RelayCommandT(DeletePhone);
            DeleteCameraCommand = new RelayCommandT(DeleteCamera);
            DeleteEquipCommand = new RelayCommandT(DeleteEquip);

            foreach (var a in DataAnalyzer.Criteries.Keys)
                ChartTypes.Add(a);
        }

        private void StartParser()
        {
            List<string> links = Parser.GetLinks("https://2droida.ru/catalog/smartfony", 200);
            foreach (string link in links)
            {
                ParserData data = Parser.Parse(link);
                DatabaseManager.DataToBD(data);
            }
        }

        private void UploadData()
        {
            Datas.Clear();
            List<ParserData> data = DatabaseManager.GetData();
            data.ForEach(d => Datas.Add(d));
            DatabaseManager.GetMatrixType()?.ForEach(d => MatrixTypes.Add(d));
        }

        private void DeletePhone(object parameter)
        {
            if (parameter is ParserData Param)
            {

                DatabaseManager.DeletePhone(Selected!);
                Datas.Remove(Selected!);
            }
        }

        private void DeleteCamera(object parameter)
        {
            if (parameter is СameraData camera)
            {
                DatabaseManager.DeleteCamera(Selected!.PhoneID, camera);
                СameraData? cameraData = Selected.Camers.FirstOrDefault(c => c.CameraType == camera.CameraType);
                Selected.Camers.Remove(cameraData!);
            }
        }

        private void DeleteEquip(object parameter)
        {
            if (parameter is string equipment)
            {
                DatabaseManager.DeleteEquipment(Selected!.PhoneID, equipment);
                Selected.Equipment.Remove(equipment);
            }
        }


        private void UpdateData()
        {

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
