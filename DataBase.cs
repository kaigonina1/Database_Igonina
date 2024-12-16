using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Spire.Xls;


namespace Лабораторная_2_С_
{
    internal class DataBaseManager
    {
        private string filePath = "camp_database.bin";
        private Dictionary<int, long> indexTable = new Dictionary<int, long>();
        private Dictionary<string, List<long>> nameTable = new Dictionary<string, List<long>>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, List<long>> locationTable = new Dictionary<string, List<long>>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<double, List<long>> priceTable = new Dictionary<double, List<long>>();
        private Dictionary<DateTime, List<long>> startDateTable = new Dictionary<DateTime, List<long>>();
        private Dictionary<DateTime, List<long>> endDateTable = new Dictionary<DateTime, List<long>>();
        private void SaveIndex()
        {
            try
            {
                using (var writer = new BinaryWriter(File.Open("index.dat", FileMode.Create)))
                {
                    foreach (var kvp in indexTable)
                    {
                        writer.Write(kvp.Key);
                        writer.Write(kvp.Value);
                    }
                }

            }
            catch (IOException ex)
            {
                MessageBox.Show($"Ошибка при сохранении индекса: {ex.Message}");
            }
        }

        private void SaveName()
        {
            try
            {
                using (var writer = new BinaryWriter(File.Open("index_name.dat", FileMode.Create)))
                {
                    foreach (var kvp in nameTable)
                    {
                        writer.Write(kvp.Key);
                        writer.Write(kvp.Value.Count);
                        foreach (var position in kvp.Value)
                        {
                            writer.Write(position);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Ошибка при сохранении индекса Name: {ex.Message}");
            }
        }

        private void SaveLocation()
        {
            try
            {
                using (var writer = new BinaryWriter(File.Open("index_location.dat", FileMode.Create)))
                {
                    foreach (var kvp in locationTable)
                    {
                        writer.Write(kvp.Key);
                        writer.Write(kvp.Value.Count);
                        foreach (var position in kvp.Value)
                        {
                            writer.Write(position);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Ошибка при сохранении индекса Location: {ex.Message}");
            }
        }

        private void SavePrice()
        {
            try
            {
                using (var writer = new BinaryWriter(File.Open("index_price.dat", FileMode.Create)))
                {
                    foreach (var kvp in priceTable)
                    {
                        writer.Write(kvp.Key);
                        writer.Write(kvp.Value.Count);
                        foreach (var position in kvp.Value)
                        {
                            writer.Write(position);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Ошибка при сохранении индекса Price: {ex.Message}");
            }
        }

        private void SaveStartDate()
        {
            try
            {
                using (var writer = new BinaryWriter(File.Open("index_start_date.dat", FileMode.Create)))
                {
                    foreach (var kvp in startDateTable)
                    {
                        if (kvp.Key == default(DateTime))
                        {
                            Debug.WriteLine("Попытка записать некорректную дату.");
                            continue; // Пропускаем некорректные записи
                        }

                        writer.Write(kvp.Key.Date.ToString("yyyy-MM-dd")); // Явный формат для DateTime
                        writer.Write(kvp.Value.Count);
                        foreach (var position in kvp.Value)
                        {
                            writer.Write(position);
                        }
                    }
                }

            }
            catch (IOException ex)
            {
                MessageBox.Show($"Ошибка при сохранении индекса: {ex.Message}");
            }
        }

        private void SaveEndDate()
        {
            try
            {
                using (var writer = new BinaryWriter(File.Open("index_end_date.dat", FileMode.Create)))
                {
                    foreach (var kvp in endDateTable)
                    {
                        if (kvp.Key == default(DateTime))
                        {
                            Debug.WriteLine("Попытка записать некорректную дату.");
                            continue; // Пропускаем некорректные записи
                        }

                        writer.Write(kvp.Key.Date.ToString("yyyy-MM-dd")); // Явный формат для DateTime
                        writer.Write(kvp.Value.Count);
                        foreach (var position in kvp.Value)
                        {
                            writer.Write(position);
                        }
                    }
                }

            }
            catch (IOException ex)
            {
                MessageBox.Show($"Ошибка при сохранении индекса: {ex.Message}");
            }
        }

        private void SaveAll()
        {
            SaveLocation();
            SavePrice();
            SaveName();
            SaveIndex();
            SaveEndDate();
            SaveStartDate();
        }

        public void WriteRecord(CampShiftRecord record, string filePath) // запись в бинарный файл
        {
            if (!File.Exists("index.dat"))
            {
                using (var writer = new BinaryWriter(File.Open(filePath, FileMode.OpenOrCreate)))
                {
                    // Позиция, на которую будем записывать
                    long position = writer.BaseStream.Position;

                    writer.Write(record.ID);
                    writer.Write(record.Name);
                    writer.Write(record.Location);
                    writer.Write(record.Price);
                    writer.Write(record.StartDate.Date.ToString());
                    writer.Write(record.EndDate.Date.ToString());

                    // Обновляем индексы
                    indexTable[record.ID] = position;

                    if (!nameTable.ContainsKey(record.Name))
                    {
                        nameTable[record.Name] = new List<long>();
                    }
                    nameTable[record.Name].Add(position);

                    if (!locationTable.ContainsKey(record.Location))
                    {
                        locationTable[record.Location] = new List<long>();
                    }
                    locationTable[record.Location].Add(position);

                    if (!priceTable.ContainsKey(record.Price))
                    {
                        priceTable[record.Price] = new List<long>();
                    }
                    priceTable[record.Price].Add(position);
                    if (!startDateTable.ContainsKey(record.StartDate.Date))
                    {
                        startDateTable[record.StartDate.Date] = new List<long>();
                    }
                    startDateTable[record.StartDate.Date].Add(position);
                    if (!endDateTable.ContainsKey(record.EndDate.Date))
                    {
                        endDateTable[record.EndDate.Date] = new List<long>();
                    }
                    endDateTable[record.EndDate.Date].Add(position);
                    // Сохранение индексов
                    SaveAll();
                }
            }
            else
            {
                using (var writer = new BinaryWriter(File.Open(filePath, FileMode.Append)))
                {
                    // Позиция, на которую будем записывать
                    long position = writer.BaseStream.Position;

                    writer.Write(record.ID);
                    writer.Write(record.Name);
                    writer.Write(record.Location);
                    writer.Write(record.Price);
                    writer.Write(record.StartDate.Date.ToString());
                    writer.Write(record.EndDate.Date.ToString());

                    // Обновляем индексы
                    indexTable[record.ID] = position;

                    if (!nameTable.ContainsKey(record.Name))
                    {
                        nameTable[record.Name] = new List<long>();
                    }
                    nameTable[record.Name].Add(position);

                    if (!locationTable.ContainsKey(record.Location))
                    {
                        locationTable[record.Location] = new List<long>();
                    }
                    locationTable[record.Location].Add(position);

                    if (!priceTable.ContainsKey(record.Price))
                    {
                        priceTable[record.Price] = new List<long>();
                    }
                    priceTable[record.Price].Add(position);
                    if (!startDateTable.ContainsKey(record.StartDate.Date))
                    {
                        startDateTable[record.StartDate.Date] = new List<long>();
                    }
                    startDateTable[record.StartDate.Date].Add(position);
                    if (!endDateTable.ContainsKey(record.EndDate.Date))
                    {
                        endDateTable[record.EndDate.Date] = new List<long>();
                    }
                    endDateTable[record.EndDate.Date].Add(position);
                    // Сохранение индексов
                    SaveAll();
                }
            }
        }
        public void LoadName()
        {
            if (!File.Exists("index_name.dat"))
                return;

            nameTable.Clear();
            using (var reader = new BinaryReader(File.Open("index_name.dat", FileMode.Open)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    string name = reader.ReadString();
                    int count = reader.ReadInt32();
                    var positions = new List<long>();
                    for (int i = 0; i < count; i++)
                    {
                        positions.Add(reader.ReadInt64());
                    }
                    nameTable[name] = positions;

                }
            }
        }

        public void LoadIndex()
        {
            if (!File.Exists("index.dat"))
                return;

            indexTable.Clear();
            using (var reader = new BinaryReader(File.Open("index.dat", FileMode.Open)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    int id = reader.ReadInt32();
                    long position = reader.ReadInt64();
                    indexTable[id] = position;
                }
            }
        }

        public void LoadLocation()
        {
            if (!File.Exists("index_location.dat"))
                return;

            locationTable.Clear();
            using (var reader = new BinaryReader(File.Open("index_location.dat", FileMode.Open)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    string location = reader.ReadString();
                    int count = reader.ReadInt32();
                    var positions = new List<long>();
                    for (int i = 0; i < count; i++)
                    {
                        positions.Add(reader.ReadInt64());
                    }
                    locationTable[location] = positions;

                }
            }
        }

        public void LoadPrice()
        {
            if (!File.Exists("index_price.dat"))
                return;

            nameTable.Clear();
            using (var reader = new BinaryReader(File.Open("index_price.dat", FileMode.Open)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    double price = reader.ReadInt64();
                    int count = reader.ReadInt32();
                    var positions = new List<long>();
                    for (int i = 0; i < count; i++)
                    {
                        positions.Add(reader.ReadInt64());
                    }
                    priceTable[price] = positions;

                }
            }
        }
        public void LoadStartDate()
        {
            if (!File.Exists("index_start_date.dat"))
                return;

            startDateTable.Clear();
            try
            {
                using (var reader = new BinaryReader(File.Open("index_start_date.dat", FileMode.Open)))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        string datastr = reader.ReadString();
                        if (!DateTime.TryParse(datastr, out DateTime date))
                        {
                            MessageBox.Show($"Некорректный формат даты: {datastr}");
                            continue; // Пропускаем некорректные записи
                        }

                        int count = reader.ReadInt32();
                        var positions = new List<long>();
                        for (int i = 0; i < count; i++)
                        {
                            positions.Add(reader.ReadInt64());
                        }

                        startDateTable[date.Date] = positions;
                    }
                }
            }

            catch (IOException ex)
            {
                MessageBox.Show($"Ошибка при загрузке индекса: {ex.Message}");
            }
        }
        public void LoadEndDate()
        {
            if (!File.Exists("index_end_date.dat"))
                return;

            endDateTable.Clear();
            try
            {
                using (var reader = new BinaryReader(File.Open("index_end_date.dat", FileMode.Open)))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        string datastr = reader.ReadString();
                        if (!DateTime.TryParse(datastr, out DateTime date))
                        {
                            MessageBox.Show($"Некорректный формат даты: {datastr}");
                            continue; // Пропускаем некорректные записи
                        }

                        int count = reader.ReadInt32();
                        var positions = new List<long>();
                        for (int i = 0; i < count; i++)
                        {
                            positions.Add(reader.ReadInt64());
                        }

                        endDateTable[date.Date] = positions;
                    }
                }
            }

            catch (IOException ex)
            {
                MessageBox.Show($"Ошибка при загрузке индекса: {ex.Message}");
            }
        }

        public void Load()
        {
            LoadLocation();
            LoadPrice();
            LoadIndex();
            LoadName();
            LoadEndDate();
            LoadStartDate();
        }

        public void Clear()
        {
            indexTable.Clear();
            nameTable.Clear();
            locationTable.Clear();
            priceTable.Clear();
            startDateTable.Clear();
            endDateTable.Clear();
            if (File.Exists("index.dat"))
            {
                try
                {
                    File.Delete("index.dat");
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Ошибка при удалении файла index.dat: {ex.Message}");
                }
            }
            if (File.Exists("index_price.dat"))
            {
                try
                {
                    File.Delete("index_price.dat");
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Ошибка при удалении файла index_price.dat: {ex.Message}");
                }
            }
            if (File.Exists("index_name.dat"))
            {
                try
                {
                    File.Delete("index_name.dat");
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Ошибка при удалении файла index_name.dat: {ex.Message}");
                }
            }
            if (File.Exists("index_location.dat"))
            {
                try
                {
                    File.Delete("index_location.dat");
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Ошибка при удалении файла index_location.dat: {ex.Message}");
                }
            }
            if (File.Exists("index_start_date.dat"))
            {
                try
                {
                    File.Delete("index_start_date.dat");
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Ошибка при удалении файла index_start_date.dat: {ex.Message}");
                }
            }
            if (File.Exists("index_end_date.dat"))
            {
                try
                {
                    File.Delete("index_end_date.dat");
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Ошибка при удалении файла index_end_date.dat: {ex.Message}");
                }
            }
            if (File.Exists("camp_database.bin"))
            {
                try
                {
                    File.Delete("camp_database.bin");
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Ошибка при удалении файла camp_database.bin: {ex.Message}");
                }
            }
            if (!File.Exists("index.dat") && !File.Exists("camp_database.bin")
                && !File.Exists("index_name.dat") && !File.Exists("index_author.dat")
                && !File.Exists("index_location.dat") && !File.Exists("index_end_date.dat")
                && !File.Exists("index_start_date.dat"))
            {
                MessageBox.Show("Файлы базы данных удалены.");
            }
        }
        public List<CampShiftRecord?> SearchRecordById(int idToFind)
        {
            List<CampShiftRecord?> results = new List<CampShiftRecord?>();
            // Проверяем наличие файла
            if (!File.Exists(filePath))
            {
                return results; // Возвращаем пцстой, если файла не существует
            }

            // Проверяем наличие записи в индексе
            if (indexTable.TryGetValue(idToFind, out long position))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read)))
                {
                    // Переходим к позиции записи
                    reader.BaseStream.Seek(position, SeekOrigin.Begin);

                    // Считываем запись
                    int id = reader.ReadInt32();
                    string name = reader.ReadString();
                    string author = reader.ReadString();
                    double price = reader.ReadDouble();
                    DateTime startdate = DateTime.Parse(reader.ReadString()).Date;
                    DateTime enddate = DateTime.Parse(reader.ReadString()).Date;
                    // Возвращаем найденную запись
                    results.Add(new (id, name, author, price, startdate, enddate));
                }
            }
            return results;
        }



        public List<CampShiftRecord?> SearchRecordsByAuthor(string locationToFind)
        {
            List<CampShiftRecord?> results = new List<CampShiftRecord?>();

            // Проверяем наличие файла и индекса
            if (!File.Exists(filePath)) return results; // если нет, то пустой список 
            if (locationTable.TryGetValue(locationToFind, out List<long> positions))
            {
                // Открываем файл и читаем записи по смещениям
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    foreach (long position in positions)
                    {
                        reader.BaseStream.Seek(position, SeekOrigin.Begin);

                        // Читаем запись
                        int id = reader.ReadInt32();
                        string name = reader.ReadString();
                        string location = reader.ReadString();
                        double price = reader.ReadDouble();
                        DateTime startdate = DateTime.Parse(reader.ReadString()).Date;
                        DateTime enddate = DateTime.Parse(reader.ReadString()).Date;
                        // Добавляем запись в результаты
                        results.Add(new CampShiftRecord(id, name, location, price, startdate, enddate));
                    }
                }
            }
            return results;
        }

        public List<CampShiftRecord?> SearchRecordsByName(string nameToFind)
        {
            List<CampShiftRecord?> results = new List<CampShiftRecord?>();

            // Проверяем наличие файла и индекса
            if (!File.Exists(filePath)) return results; // если нет, то пустой список 
            if (nameTable.TryGetValue(nameToFind, out List<long> positions))
            {
                // Открываем файл и читаем записи по смещениям
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    foreach (long position in positions)
                    {
                        reader.BaseStream.Seek(position, SeekOrigin.Begin);

                        // Читаем запись
                        int id = reader.ReadInt32();
                        string name = reader.ReadString();
                        string location = reader.ReadString();
                        double price = reader.ReadDouble();
                        DateTime startdate = DateTime.Parse(reader.ReadString()).Date;
                        DateTime enddate = DateTime.Parse(reader.ReadString()).Date;
                        // Добавляем запись в результаты
                        results.Add(new CampShiftRecord(id, name, location, price, startdate, enddate));
                    }
                }
            }
            return results;
        }

        public List<CampShiftRecord?> SearchRecordsByPrice(double priceToFind)
        {
            List<CampShiftRecord?> results = new List<CampShiftRecord?>();
            // Проверяем наличие файла и индекса
            if (!File.Exists(filePath)) return results; // если нет, то пустой список 
            if (priceTable.TryGetValue(priceToFind, out List<long> positions))
            {
                // Открываем файл и читаем записи по смещениям
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    foreach (long position in positions)
                    {
                        reader.BaseStream.Seek(position, SeekOrigin.Begin);

                        // Читаем запись
                        int id = reader.ReadInt32();
                        string name = reader.ReadString();
                        string location = reader.ReadString();
                        double price = reader.ReadDouble();
                        DateTime startdate = DateTime.Parse(reader.ReadString()).Date;
                        DateTime enddate = DateTime.Parse(reader.ReadString()).Date;
                        // Добавляем запись в результаты
                        results.Add(new CampShiftRecord(id, name, location, price, startdate, enddate));
                    }
                }
            }
            return results;
        }
        public List<CampShiftRecord?> SearchRecordsByStartDate(string dateToFind)
        {
            List<CampShiftRecord?> results = new List<CampShiftRecord?>();

            // Проверяем наличие файла и индекса
            if (!File.Exists(filePath)) return results; // если нет, то пустой список
            DateTime searchDate = DateTime.Parse(dateToFind).Date;
            if (startDateTable.TryGetValue(DateTime.Parse(dateToFind).Date, out List<long> positions))
            {
                // Открываем файл и читаем записи по смещениям
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    foreach (long position in positions)
                    {
                        reader.BaseStream.Seek(position, SeekOrigin.Begin);

                        // Читаем запись
                        int id = reader.ReadInt32();
                        string name = reader.ReadString();
                        string location = reader.ReadString();
                        double price = reader.ReadDouble();
                        DateTime startdate = DateTime.Parse(reader.ReadString()).Date;
                        DateTime enddate = DateTime.Parse(reader.ReadString()).Date;
                        // Добавляем запись в результаты
                        results.Add(new CampShiftRecord(id, name, location, price, startdate, enddate));
                    }
                }
            }
            return results;
        }
        public List<CampShiftRecord?> SearchRecordsByEndDate(string dateToFind)
        {
            List<CampShiftRecord?> results = new List<CampShiftRecord?>();

            // Проверяем наличие файла и индекса
            if (!File.Exists(filePath)) return results; // если нет, то пустой список
            DateTime searchDate = DateTime.Parse(dateToFind).Date;
            if (endDateTable.TryGetValue(DateTime.Parse(dateToFind).Date, out List<long> positions))
            {
                // Открываем файл и читаем записи по смещениям
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    foreach (long position in positions)
                    {
                        reader.BaseStream.Seek(position, SeekOrigin.Begin);

                        // Читаем запись
                        int id = reader.ReadInt32();
                        string name = reader.ReadString();
                        string location = reader.ReadString();
                        double price = reader.ReadDouble();
                        DateTime startdate = DateTime.Parse(reader.ReadString()).Date;
                        DateTime enddate = DateTime.Parse(reader.ReadString()).Date;
                        // Добавляем запись в результаты
                        results.Add(new CampShiftRecord(id, name, location, price, startdate, enddate));
                    }
                }
            }
            return results;
        }
        public CampShiftRecord? SearchRecordByPosition(FileStream file, long position)
        {
            try
            {
                CampShiftRecord record = new CampShiftRecord();
                using (BinaryReader reader = new BinaryReader(file, Encoding.Default, true)) // Передаем существующий поток
                {
                    reader.BaseStream.Seek(position, SeekOrigin.Begin);
                    int id = reader.ReadInt32();
                    string name = reader.ReadString();
                    string location = reader.ReadString();
                    double price = reader.ReadDouble();
                    DateTime startdate = DateTime.Parse(reader.ReadString()).Date;
                    DateTime enddate = DateTime.Parse(reader.ReadString()).Date;

                    if (id == -1)
                        return null;

                    record.Location = location;
                    record.StartDate = startdate;
                    record.EndDate = enddate;   
                    record.Price = price;
                    record.Name = name;
                    record.ID = id;
                }
                return record;
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Ошибка при поиске записи по позиции: {ex.Message}");
                return null;
            }
        }


        public void DeleteRecordbyId(int id) // удаление записи 
        {
            if (!indexTable.ContainsKey(id))
            {
                MessageBox.Show("Запись не найдена в индексе.");
                return;
            }
            else
            {
                using (var file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    long position = indexTable[id];
                    CampShiftRecord? record = SearchRecordByPosition(file, position);
                    if (record.HasValue)
                    {
                        // Удаляем позицию из индекса Name
                        if (nameTable.ContainsKey(record.Value.Name))
                        {
                            nameTable[record.Value.Name].Remove(position);
                            if (nameTable[record.Value.Name].Count == 0)
                            {
                                nameTable.Remove(record.Value.Name);
                            }
                        }

                        // Удаляем позицию из индекса Author
                        if (locationTable.ContainsKey(record.Value.Location))
                        {
                            locationTable[record.Value.Location].Remove(position);
                            if (locationTable[record.Value.Location].Count == 0)
                            {
                                locationTable.Remove(record.Value.Location);
                            }
                        }
                        // Удаляем позицию из индекса price
                        if (priceTable.ContainsKey(record.Value.Price))
                        {
                            priceTable[record.Value.Price].Remove(position);
                            if (priceTable[record.Value.Price].Count == 0)
                            {
                                priceTable.Remove(record.Value.Price);
                            }
                        }
                        if (startDateTable.ContainsKey(record.Value.StartDate.Date))
                        {
                            startDateTable[record.Value.StartDate.Date].Remove(position);
                            if (startDateTable[record.Value.StartDate.Date].Count == 0)
                            {
                                startDateTable.Remove(record.Value.StartDate.Date);
                            }
                        }
                        if (endDateTable.ContainsKey(record.Value.EndDate.Date))
                        {
                            endDateTable[record.Value.EndDate.Date].Remove(position);
                            if (endDateTable[record.Value.EndDate.Date].Count == 0)
                            {
                                endDateTable.Remove(record.Value.EndDate.Date);
                            }
                        }
                    }
                    file.Seek(position, SeekOrigin.Begin);
                    using (var writer = new BinaryWriter(file, Encoding.Default, true))
                    {
                        writer.Write(-1); // Помечаем ID как удаленный, например, ID = -1.
                    }
                    indexTable.Remove(id);
                    SaveAll();
                }
            }
        }

        public void DeleteRecordbyName(string name) // удаление записи 
        {
            if (!nameTable.ContainsKey(name))
            {
                MessageBox.Show("Запись не найдена.");
                return;
            }
            else
            {
                using (var file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    List<long> positions = nameTable[name];
                    using (var writer = new BinaryWriter(file, Encoding.Default, true))
                    {
                        foreach (long position in positions)
                        {
                            CampShiftRecord? record = SearchRecordByPosition(file, position);
                            if (record.HasValue)
                            {
                                // Удаляем позицию из индекса Name
                                if (indexTable.ContainsKey(record.Value.ID))
                                {
                                    indexTable.Remove(record.Value.ID);
                                }

                                // Удаляем позицию из индекса Author
                                if (locationTable.ContainsKey(record.Value.Location))
                                {
                                    locationTable[record.Value.Location].Remove(position);
                                    if (locationTable[record.Value.Location].Count == 0)
                                    {
                                        locationTable.Remove(record.Value.Location);
                                    }
                                }
                                // Удаляем позицию из индекса price
                                if (priceTable.ContainsKey(record.Value.Price))
                                {
                                    priceTable[record.Value.Price].Remove(position);
                                    if (priceTable[record.Value.Price].Count == 0)
                                    {
                                        priceTable.Remove(record.Value.Price);
                                    }
                                }
                                if (startDateTable.ContainsKey(record.Value.StartDate.Date))
                                {
                                    startDateTable[record.Value.StartDate.Date].Remove(position);
                                    if (startDateTable[record.Value.StartDate.Date].Count == 0)
                                    {
                                        startDateTable.Remove(record.Value.StartDate.Date);
                                    }
                                }
                                if (endDateTable.ContainsKey(record.Value.EndDate.Date))
                                {
                                    endDateTable[record.Value.EndDate.Date].Remove(position);
                                    if (endDateTable[record.Value.EndDate.Date].Count == 0)
                                    {
                                        endDateTable.Remove(record.Value.EndDate.Date);
                                    }
                                }
                            }
                            file.Seek(position, SeekOrigin.Begin);
                            writer.Write(-1); // Помечаем ID как удаленный
                        }
                    }
                    nameTable.Remove(name);
                    SaveAll();
                }
            }
        }

        public void DeleteRecordbyAuthor(string location) // удаление записи 
        {
            if (!locationTable.ContainsKey(location))
            {
                MessageBox.Show("Запись не найдена.");
                return;
            }
            else
            {
                using (var file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    List<long> positions = locationTable[location];
                    using (var writer = new BinaryWriter(file, Encoding.Default, true))
                    {
                        foreach (long position in positions)
                        {
                            CampShiftRecord? record = SearchRecordByPosition(file, position);
                            if (record.HasValue)
                            {
                                // Удаляем позицию из индекса Name
                                if (indexTable.ContainsKey(record.Value.ID))
                                {
                                    indexTable.Remove(record.Value.ID);
                                }

                                // Удаляем позицию из индекса Author
                                if (nameTable.ContainsKey(record.Value.Name))
                                {
                                    nameTable[record.Value.Name].Remove(position);
                                    if (nameTable[record.Value.Name].Count == 0)
                                    {
                                        nameTable.Remove(record.Value.Name);
                                    }
                                }
                                // Удаляем позицию из индекса price
                                if (priceTable.ContainsKey(record.Value.Price))
                                {
                                    priceTable[record.Value.Price].Remove(position);
                                    if (priceTable[record.Value.Price].Count == 0)
                                    {
                                        priceTable.Remove(record.Value.Price);
                                    }
                                }
                                if (startDateTable.ContainsKey(record.Value.StartDate.Date))
                                {
                                    startDateTable[record.Value.StartDate.Date].Remove(position);
                                    if (startDateTable[record.Value.StartDate.Date].Count == 0)
                                    {
                                        startDateTable.Remove(record.Value.StartDate.Date);
                                    }
                                }
                                if (endDateTable.ContainsKey(record.Value.EndDate.Date))
                                {
                                    endDateTable[record.Value.EndDate.Date].Remove(position);
                                    if (endDateTable[record.Value.EndDate.Date].Count == 0)
                                    {
                                        endDateTable.Remove(record.Value.EndDate.Date);
                                    }
                                }
                            }
                            file.Seek(position, SeekOrigin.Begin);
                            writer.Write(-1); // Помечаем ID как удаленный
                        }
                        locationTable.Remove(location);
                        SaveAll();
                    }
                }
            }
        }

        public void DeleteRecordbyPrice(double price) // удаление записи 
        {
            if (!priceTable.ContainsKey(price))
            {
                MessageBox.Show("Запись не найдена.");
                return;
            }
            else
            {
                using (var file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    List<long> positions = priceTable[price];
                    using (var writer = new BinaryWriter(file, Encoding.Default, true))
                    {
                        foreach (long position in positions)
                        {

                            CampShiftRecord? record = SearchRecordByPosition(file, position);
                            if (record.HasValue)
                            {
                                // Удаляем позицию из индекса Name
                                if (indexTable.ContainsKey(record.Value.ID))
                                {
                                    indexTable.Remove(record.Value.ID);
                                }

                                // Удаляем позицию из индекса Author
                                if (nameTable.ContainsKey(record.Value.Name))
                                {
                                    nameTable[record.Value.Name].Remove(position);
                                    if (nameTable[record.Value.Name].Count == 0)
                                    {
                                        nameTable.Remove(record.Value.Name);
                                    }
                                }
                                // Удаляем позицию из индекса price
                                if (locationTable.ContainsKey(record.Value.Location))
                                {
                                    locationTable[record.Value.Location].Remove(position);
                                    if (locationTable[record.Value.Location].Count == 0)
                                    {
                                        locationTable.Remove(record.Value.Location);
                                    }
                                }
                                if (startDateTable.ContainsKey(record.Value.StartDate.Date))
                                {
                                    startDateTable[record.Value.StartDate.Date].Remove(position);
                                    if (startDateTable[record.Value.StartDate.Date].Count == 0)
                                    {
                                        startDateTable.Remove(record.Value.StartDate.Date);
                                    }
                                }
                                if (endDateTable.ContainsKey(record.Value.EndDate.Date))
                                {
                                    endDateTable[record.Value.EndDate.Date].Remove(position);
                                    if (endDateTable[record.Value.EndDate.Date].Count == 0)
                                    {
                                        endDateTable.Remove(record.Value.EndDate.Date);
                                    }
                                }
                            }
                            file.Seek(position, SeekOrigin.Begin);
                            writer.Write(-1); // Помечаем ID как удаленный
                        }
                        priceTable.Remove(price);
                        SaveAll();
                    }
                }
            }
        }

        public void DeleteRecordbyStartDate(DateTime date) // удаление записи 
        {
            if (!startDateTable.ContainsKey(date.Date))
            {
                MessageBox.Show("Запись не найдена.");
                return;
            }
            else
            {
                using (var file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    List<long> positions = startDateTable[date.Date];
                    using (var writer = new BinaryWriter(file, Encoding.Default, true))
                    {
                        foreach (long position in positions)
                        {

                            CampShiftRecord? record = SearchRecordByPosition(file, position);
                            if (record.HasValue)
                            {
                                // Удаляем позицию из индекса Name
                                if (indexTable.ContainsKey(record.Value.ID))
                                {
                                    indexTable.Remove(record.Value.ID);
                                }

                                // Удаляем позицию из индекса Author
                                if (nameTable.ContainsKey(record.Value.Name))
                                {
                                    nameTable[record.Value.Name].Remove(position);
                                    if (nameTable[record.Value.Name].Count == 0)
                                    {
                                        nameTable.Remove(record.Value.Name);
                                    }
                                }
                                // Удаляем позицию из индекса price
                                if (locationTable.ContainsKey(record.Value.Location))
                                {
                                    locationTable[record.Value.Location].Remove(position);
                                    if (locationTable[record.Value.Location].Count == 0)
                                    {
                                        locationTable.Remove(record.Value.Location);
                                    }
                                }
                                
                                if (endDateTable.ContainsKey(record.Value.EndDate.Date))
                                {
                                    endDateTable[record.Value.EndDate.Date].Remove(position);
                                    if (endDateTable[record.Value.EndDate.Date].Count == 0)
                                    {
                                        endDateTable.Remove(record.Value.EndDate.Date);
                                    }
                                }
                                if (priceTable.ContainsKey(record.Value.Price))
                                {
                                    priceTable[record.Value.Price].Remove(position);
                                    if (priceTable[record.Value.Price].Count == 0)
                                    {
                                        priceTable.Remove(record.Value.Price);
                                    }
                                }
                            }
                            file.Seek(position, SeekOrigin.Begin);
                            writer.Write(-1); // Помечаем ID как удаленный
                        }
                        startDateTable.Remove(date.Date);
                        SaveAll();
                    }
                }
            }
        }
        public void DeleteRecordbyEndDate(DateTime date) // удаление записи 
        {
            if (!endDateTable.ContainsKey(date.Date))
            {
                MessageBox.Show("Запись не найдена.");
                return;
            }
            else
            {
                using (var file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    List<long> positions = endDateTable[date.Date];
                    using (var writer = new BinaryWriter(file, Encoding.Default, true))
                    {
                        foreach (long position in positions)
                        {

                            CampShiftRecord? record = SearchRecordByPosition(file, position);
                            if (record.HasValue)
                            {
                                // Удаляем позицию из индекса Name
                                if (indexTable.ContainsKey(record.Value.ID))
                                {
                                    indexTable.Remove(record.Value.ID);
                                }

                                // Удаляем позицию из индекса Author
                                if (nameTable.ContainsKey(record.Value.Name))
                                {
                                    nameTable[record.Value.Name].Remove(position);
                                    if (nameTable[record.Value.Name].Count == 0)
                                    {
                                        nameTable.Remove(record.Value.Name);
                                    }
                                }
                                // Удаляем позицию из индекса price
                                if (locationTable.ContainsKey(record.Value.Location))
                                {
                                    locationTable[record.Value.Location].Remove(position);
                                    if (locationTable[record.Value.Location].Count == 0)
                                    {
                                        locationTable.Remove(record.Value.Location);
                                    }
                                }

                                if (priceTable.ContainsKey(record.Value.Price))
                                {
                                    priceTable[record.Value.Price].Remove(position);
                                    if (priceTable[record.Value.Price].Count == 0)
                                    {
                                        priceTable.Remove(record.Value.Price);
                                    }
                                }
                                if (startDateTable.ContainsKey(record.Value.StartDate.Date))
                                {
                                    startDateTable[record.Value.StartDate.Date].Remove(position);
                                    if (startDateTable[record.Value.StartDate.Date].Count == 0)
                                    {
                                        startDateTable.Remove(record.Value.StartDate.Date);
                                    }
                                }
                            }
                            file.Seek(position, SeekOrigin.Begin);
                            writer.Write(-1); // Помечаем ID как удаленный
                        }
                        endDateTable.Remove(date.Date);
                        SaveAll();
                    }
                }
            }
        }

        public bool EditRecordById(int id, string? newName = null, string? newLocation = null, double? newPrice = null, DateTime? newStartDate = null, DateTime? newEndDate = null)
        {
            if (!indexTable.ContainsKey(id))
            {
                MessageBox.Show("Запись не найдена.");
                return false;
            }

            // Получаем позицию записи в файле
            long position = indexTable[id];

            try
            {
                using (var file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                using (var writer = new BinaryWriter(file, Encoding.Default, true))
                using (var reader = new BinaryReader(file, Encoding.Default, true))
                {
                    // Считываем текущую запись
                    file.Seek(position, SeekOrigin.Begin);
                    int recordId = reader.ReadInt32();
                    string currentname = reader.ReadString();
                    string currentlocation = reader.ReadString();
                    double currentprice = reader.ReadDouble();
                    DateTime currentstartdate = DateTime.Parse(reader.ReadString()).Date;
                    DateTime currentenddate = DateTime.Parse(reader.ReadString()).Date;
                    if (recordId != id)
                    {
                        MessageBox.Show("Ошибка: ID записи не совпадает.");
                        return false;
                    }

                    // Обновляем индексные таблицы
                    if (newName != null && newName != currentname)
                    {
                        if (nameTable.ContainsKey(currentname))
                        {
                            nameTable[currentname].Remove(position);
                            if (nameTable[currentname].Count == 0)
                            {
                                nameTable.Remove(currentname);
                            }
                        }
                        if (!nameTable.ContainsKey(newName))
                        {
                            nameTable[newName] = new List<long>();
                        }
                        nameTable[newName].Add(position);
                    }

                    if (newLocation != null && newLocation != currentlocation)
                    {
                        if (locationTable.ContainsKey(currentlocation))
                        {
                            locationTable[currentlocation].Remove(position);
                            if (locationTable[currentlocation].Count == 0)
                            {
                                locationTable.Remove(currentlocation);
                            }
                        }
                        if (!locationTable.ContainsKey(newLocation))
                        {
                            locationTable[newLocation] = new List<long>();
                        }
                        locationTable[newLocation].Add(position);
                    }

                    if (newPrice != null && newPrice != currentprice)
                    {
                        if (priceTable.ContainsKey(currentprice))
                        {
                            priceTable[currentprice].Remove(position);
                            if (priceTable[currentprice].Count == 0)
                            {
                                priceTable.Remove(currentprice);
                            }
                        }
                        if (!priceTable.ContainsKey(newPrice.Value))
                        {
                            priceTable[newPrice.Value] = new List<long>();
                        }
                        priceTable[newPrice.Value].Add(position);
                    }

                    if (newStartDate != null && newStartDate.Value.Date != currentstartdate.Date)
                    {
                        if (startDateTable.ContainsKey(currentstartdate.Date))
                        {
                            startDateTable[currentstartdate.Date].Remove(position);
                            if (startDateTable[currentstartdate.Date].Count == 0)
                            {
                                startDateTable.Remove(currentstartdate.Date);
                            }
                        }
                        if (!startDateTable.ContainsKey(newStartDate.Value.Date))
                        {
                            startDateTable[newStartDate.Value.Date] = new List<long>();
                        }
                        startDateTable[newStartDate.Value.Date].Add(position);
                    }
                    if (newEndDate != null && newEndDate.Value.Date != currentenddate.Date)
                    {
                        if (endDateTable.ContainsKey(currentenddate.Date))
                        {
                            endDateTable[currentenddate.Date].Remove(position);
                            if (endDateTable[currentenddate.Date].Count == 0)
                            {
                                endDateTable.Remove(currentenddate.Date);
                            }
                        }
                        if (!endDateTable.ContainsKey(newEndDate.Value.Date))
                        {
                            endDateTable[newEndDate.Value.Date] = new List<long>();
                        }
                        endDateTable[newStartDate.Value.Date].Add(position);
                    }

                    // Перезапись записи в файл
                    file.Seek(position, SeekOrigin.Begin);
                    writer.Write(recordId); // ID остаётся прежним
                    writer.Write(newName ?? currentname);
                    writer.Write(newLocation ?? currentlocation);
                    writer.Write(newPrice ?? currentprice);
                    writer.Write((newEndDate ?? currentenddate).Date.ToString());
                    writer.Write((newStartDate ?? currentstartdate).Date.ToString());

                    SaveAll(); // Сохраняем обновлённые индексы
                    return true;
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Ошибка при редактировании записи: {ex.Message}");
                return false;
            }
        }
        public void SaveToXlsx()
        {
            // Проверка, существует ли файл базы данных
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Файл базы данных не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Создаем новую Excel-книгу
            Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets[0]; // Первый лист

            // Заголовки столбцов
            sheet.Range["A1"].Text = "ID";
            sheet.Range["B1"].Text = "Name";
            sheet.Range["C1"].Text = "Location";
            sheet.Range["D1"].Text = "Price";
            sheet.Range["E1"].Text = "StartDate";
            sheet.Range["F1"].Text = "EndDate";

            int row = 2; // Начинаем со второй строки для данных

            try
            {
                // Открываем файл для чтения данных
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                using (var reader = new BinaryReader(fileStream, Encoding.Default, true))
                {
                    foreach (var kvp in indexTable)
                    {
                        int id = kvp.Key;
                        long position = kvp.Value;

                        // Переходим к позиции записи в файле
                        fileStream.Seek(position, SeekOrigin.Begin);

                        // Читаем данные
                        int recordId = reader.ReadInt32();
                        string name = reader.ReadString();
                        string location = reader.ReadString();
                        double price = reader.ReadDouble();
                        DateTime startdate = DateTime.Parse(reader.ReadString()).Date;
                        DateTime enddate = DateTime.Parse(reader.ReadString()).Date;

                        // Записываем данные в Excel
                        sheet.Range[$"A{row}"].NumberValue = recordId;
                        sheet.Range[$"B{row}"].Text = name;
                        sheet.Range[$"C{row}"].Text = location;
                        sheet.Range[$"D{row}"].NumberValue = price;
                        sheet.Range[$"E{row}"].Text = startdate.ToString("yyyy-MM-dd");
                        sheet.Range[$"F{row}"].Text = enddate.ToString("yyyy-MM-dd");

                        row++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Сохраняем Excel-файл с использованием диалога
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel файлы (*.xlsx)|*.xlsx|Все файлы (*.*)|*.*";
                saveFileDialog.Title = "Сохранить базу данных как Excel файл";
                saveFileDialog.FileName = "Database.xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        workbook.SaveToFile(saveFileDialog.FileName, ExcelVersion.Version2016);
                        MessageBox.Show("База данных успешно сохранена в файл Excel!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }
    }
    [Serializable]
    public struct CampShiftRecord
    {
        public int ID;
        public string Name;
        public string Location;
        public double Price;
        public DateTime StartDate;
        public DateTime EndDate;

        public CampShiftRecord(int id, string name, string location, double price, DateTime startDate, DateTime endDate)
        {
            ID = id;
            Name = name;
            Location = location;
            Price = price;
            StartDate = startDate.Date;
            EndDate = endDate.Date;
        }
    }
    
}

