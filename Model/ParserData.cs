namespace CourseWork.Model
{
    public class ParserData
    {
        public Guid PhoneID { get; set; } = Guid.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public double CurrentPrice { get; set; } = double.NaN;
        public int? Dicsount { get; set; } = 0;
        public double? OldPrice { get; set; } = double.NaN;
        public string MatrixType { get; set; } = string.Empty;
        public double Screen_Diagonal { get; set; } = double.NaN;
        public int BatteryCapacity { get; set; } = 0;
        public int REM {  get; set; } = 0;



        public List<string> Equipment { get; set; } = [];
        public List<string> Tags { get; set; } = [];
        public List<СameraData> Camers { get; set; } = [];


        public void AddCamera(string cameraType, string specification)
        {
            СameraData cameraData = new(cameraType, specification);
            Camers.Add(cameraData);
        }
    }

    public class СameraData(string cameraType, string specification)
    {
        public string CameraType { get; set; } = cameraType;
        public string Specification { get; set; } = specification;
    }
}