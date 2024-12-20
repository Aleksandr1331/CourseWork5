using CourseWork.Model;
using CourseWork.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.ViewModel
{
    public static class DatabaseManager
    {
        public static void DataToBD(ParserData parserData)
        {
            using SmartPhoneContext db = new();

            Brand? brand = db.Brands.FirstOrDefault(b => b.BrandName == parserData.Brand);
            if (brand is null)
            {
                brand = new Brand { BrandName = parserData.Brand, Models = [], BrandId = Guid.NewGuid() };
                db.Brands.Add(brand);
            };


            Models.Model? model = db.Models.FirstOrDefault(m => m.Model1 == parserData.Model);
            if (model is null)
            {
                model = new Models.Model { Model1 = parserData.Model, Brand = brand, Phones = [], ModelId = Guid.NewGuid() };
                db.Models.Add(model);
            };


            if (!brand.Models.Contains(model))
            {
                brand.Models.Add(model);
            }


            Phone NewPhone = new()
            {
                PhoneId = Guid.NewGuid(),
                Color = parserData.Color,
                Rem = parserData.REM,
                BatteryCapacity = parserData.BatteryCapacity,
                ScreenDiagonal = parserData.Screen_Diagonal,
                CurrentPrice = parserData.CurrentPrice,
                OldPrice = parserData.OldPrice,
                Discont = parserData.Dicsount,
                Model = model,
            };
            db.Phones.Add(NewPhone);
            model.Phones.Add(NewPhone);


            MatrixType? matrixType = db.MatrixTypes.FirstOrDefault(m => m.MatrixType1 == parserData.MatrixType);
            if (matrixType is null)
            {
                matrixType = new MatrixType { MatrixType1 = parserData.MatrixType, Phones = [], MatrixTypeId = Guid.NewGuid() };
                db.MatrixTypes.Add(matrixType);
            }
            matrixType.Phones.Add(NewPhone);
            NewPhone.MatrixTypes.Add(matrixType);


            foreach (string tagName in parserData.Tags)
            {
                Tag? tag = db.Tags.Local.FirstOrDefault(t => t.TagName == tagName)
                    ?? db.Tags.FirstOrDefault(t => t.TagName == tagName);
                if (tag is null)
                {
                    tag = new Tag { TagName = tagName, Phones = [], TagId = Guid.NewGuid() };
                    db.Tags.Add(tag);
                }
                tag.Phones.Add(NewPhone);
                NewPhone.Tags.Add(tag);
            }


            foreach (string EquipmentName in parserData.Equipment)
            {
                Equipment? equipment = db.Equipment.FirstOrDefault(e => e.EquipmentName == EquipmentName);
                if (equipment is null)
                {
                    equipment = new Equipment { EquipmentName = EquipmentName, Phones = [], EquipmentId = Guid.NewGuid() };
                    db.Equipment.Add(equipment);
                };
                equipment.Phones.Add(NewPhone);
                NewPhone.Equipment.Add(equipment);
            }

            foreach (СameraData cameraData in parserData.Camers)
            {
                CameraType? cameraType = db.CameraTypes.FirstOrDefault(c => c.Type == cameraData.CameraType);
                if (cameraType is null)
                {
                    cameraType = new CameraType { Type = cameraData.CameraType, CameraInPhones = [], TypeId = Guid.NewGuid() };
                    db.CameraTypes.Add(cameraType);
                };


                CameraInPhone cameraInPhone = new()
                {
                    CameraId = Guid.NewGuid(),
                    Specifications = cameraData.Specification,
                    Type = cameraType,
                    Phone = NewPhone
                };
                db.CameraInPhones.Add(cameraInPhone);
                cameraType.CameraInPhones.Add(cameraInPhone);
                NewPhone.CameraInPhones.Add(cameraInPhone);
            }


            db.SaveChanges();
        }


        public static List<ParserData> GetData()
        {
            List<ParserData> data = [];

            using SmartPhoneContext db = new();

            List<Phone> phones = db.Phones
                .Include(p => p.Model)
                    .ThenInclude(m => m.Brand)
                .Include(p => p.Tags)
                .Include(p => p.Equipment)
                .Include(p => p.CameraInPhones)
                    .ThenInclude(c => c.Type)
                .Include(p => p.MatrixTypes)
                .ToList();

            foreach (Phone phone in phones)
            {
                ParserData newData = new()
                {
                    PhoneID = phone.PhoneId,

                    Model = phone.Model.Model1,
                    Brand = phone.Model.Brand.BrandName,

                    CurrentPrice = phone.CurrentPrice,
                    Dicsount = phone.Discont,
                    OldPrice = phone.OldPrice,

                    Color = phone.Color,
                    REM = phone.Rem,
                    BatteryCapacity = phone.BatteryCapacity,
                    Screen_Diagonal = phone.ScreenDiagonal,
                    MatrixType = phone.MatrixTypes.First().MatrixType1,
                    Tags = phone.Tags.Select(t => t.TagName).ToList(),
                    Equipment = phone.Equipment.Select(e => e.EquipmentName).ToList()
                };

                foreach (var cameraType in phone.CameraInPhones)
                {
                    newData.AddCamera(cameraType.Type.Type, cameraType.Specifications);
                };

                data.Add(newData);
            }

            return data;
        }


        public static List<string> GetMatrixType()
        {
            List<string> types = [];

            using SmartPhoneContext db = new SmartPhoneContext();
            types = db.MatrixTypes.Select(m => m.MatrixType1).ToList() ?? [];

            return types;
        }


        public static string DeletePhone(ParserData parserData)
        {
            string message = "Ошибка в удалении";
            using SmartPhoneContext db = new();

            Phone? currentPhone = db.Phones
                .Include(p => p.Tags)
                .Include(p => p.Equipment)
                .Include(p => p.MatrixTypes)
                .Include(p => p.CameraInPhones)
                    .ThenInclude(c => c.Type)
                .Include(p => p.Model)
                    .ThenInclude(m => m.Brand)
                .FirstOrDefault(p => p.PhoneId == parserData.PhoneID);

            if (currentPhone != null)
            {
                foreach (var tag in currentPhone.Tags.ToList())
                {
                    tag.Phones.Remove(currentPhone);
                    currentPhone.Tags.Remove(tag);
                    //var TagsBD = db.Tags.Include.FirstOrDefault(t => t.TagName == tag.TagName)?.Phones.ToList();
                    var TagsBD = db.Tags.Where(t => t.TagName == tag.TagName).ToList();
                    if (TagsBD != null && TagsBD.Count == 0)
                    {
                        db.Tags.Remove(tag);
                    }
                }

                foreach (var equipment in currentPhone.Equipment.ToList())
                {
                    equipment.Phones.Remove(currentPhone);
                    currentPhone.Equipment.Remove(equipment);
                    var EquipDB = db.Equipment.Where(e => e.EquipmentName == equipment.EquipmentName).ToList();
                    if (EquipDB != null && EquipDB.Count == 0)
                    {
                        db.Equipment.Remove(equipment);
                    }
                }

                foreach (var matrix in currentPhone.MatrixTypes.ToList())
                {
                    matrix.Phones.Remove(currentPhone);
                    currentPhone.MatrixTypes.Remove(matrix);
                    var MatrixDB = db.MatrixTypes.Where(m => m.MatrixType1 == matrix.MatrixType1).ToList();
                    if (MatrixDB != null && MatrixDB.Count == 0)
                    {
                        db.MatrixTypes.Remove(matrix);
                    }
                }

                foreach (var camera in currentPhone.CameraInPhones.ToList())
                {

                    CameraType cameraType = camera.Type;
                    cameraType.CameraInPhones.Remove(camera);
                    db.CameraInPhones.Remove(camera);
                    var CameraTypeBD = db.CameraInPhones.Include(C => C.Type).Where(c => c.Type.Type == cameraType.Type).ToList();
                    if (CameraTypeBD != null && CameraTypeBD.Count == 0)
                    {
                        db.CameraTypes.Remove(cameraType);
                    }
                }
                db.Phones.Remove(currentPhone);

                try
                {
                    db.SaveChanges();
                    message = "Запись удалена";
                }
                catch (DbUpdateException ex)
                {
                    message = "Ошибка при удалении: " + ex.InnerException?.Message;
                }
            }

            return message;
        }


        public static string DeleteCamera(Guid phoneId, СameraData cameraData)
        {
            string message = "Ошибка в удалении";
            using SmartPhoneContext db = new();

            Phone? currentPhone = db.Phones
                .Include(p => p.CameraInPhones)
                    .ThenInclude(c => c.Type)
                .FirstOrDefault(p => p.PhoneId == phoneId);

            if (currentPhone != null)
            {
                CameraInPhone? camera = currentPhone.CameraInPhones
                    .FirstOrDefault(c => c.Type.Type == cameraData.CameraType);

                if (camera != null)
                {
                    CameraType cameraType = camera.Type;
                    cameraType.CameraInPhones.Remove(camera);

                    var CameraTypeBD = db.CameraInPhones.Include(C => C.Type).Where(c => c.Type.Type == cameraType.Type).ToList();
                    if (CameraTypeBD != null && CameraTypeBD.Count == 0)
                    {
                        db.CameraTypes.Remove(cameraType);
                    }

                    db.CameraInPhones.Remove(camera);

                    try
                    {
                        db.SaveChanges();
                        message = "Камера удалена";
                    }
                    catch (DbUpdateException ex)
                    {
                        message = "Ошибка при удалении камеры: " + ex.InnerException?.Message;
                    }
                }
            }

            return message;
        }


        public static string DeleteEquipment(Guid phoneId, string equipmentName)
        {
            string message = "Ошибка в удалении";
            using SmartPhoneContext db = new();

            Phone? currentPhone = db.Phones
                .Include(p => p.Equipment)
                .FirstOrDefault(p => p.PhoneId == phoneId);

            if (currentPhone != null)
            {
                Equipment? equipment = currentPhone.Equipment
                    .FirstOrDefault(e => e.EquipmentName == equipmentName);

                if (equipment != null)
                {
                    equipment.Phones.Remove(currentPhone);
                    currentPhone.Equipment.Remove(equipment);

                    //Можно через цикл и если находит 2, то все
                    var EquipmentBD = db.Phones.Include(p => p.Equipment).Where(e => e.Equipment.FirstOrDefault(e => e.EquipmentName == equipment.EquipmentName) != null).ToList();
                    if (equipment.Phones.Count == 0)
                    {
                        db.Equipment.Remove(equipment);
                    }

                    // Сохраняем изменения
                    try
                    {
                        db.SaveChanges();
                        message = "Оборудование удалено";
                    }
                    catch (DbUpdateException ex)
                    {
                        message = "Ошибка при удалении оборудования: " + ex.InnerException?.Message;
                    }
                }
            }

            return message;
        }
    }
}