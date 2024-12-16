using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Лабораторная_2_С_
{
    public partial class ButtonSaveXcel : Form
    {
        private DataBaseManager manager;
        public ButtonSaveXcel()
        {
            InitializeComponent();
            manager = new DataBaseManager();
            try
            {
                manager.Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void LoadDataToGrid()
        {
            dataGridView1.Rows.Clear(); // Очищаем таблицу

            if (!File.Exists("camp_database.bin"))
            {
                return;
            }

            try
            {
                using (var reader = new BinaryReader(File.Open("camp_database.bin", FileMode.Open)))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        try
                        {
                            // Попытка прочитать поля
                            int id = reader.ReadInt32();
                            string name = reader.ReadString();
                            string author = reader.ReadString();
                            double price = reader.ReadDouble();
                            string startdateString = reader.ReadString();
                            string enddateString = reader.ReadString();

                            if (!DateTime.TryParse(startdateString, out DateTime startdate))
                            {
                                MessageBox.Show("Некорректный формат даты.");
                                break;
                            }
                            if (!DateTime.TryParse(enddateString, out DateTime enddate))
                            {
                                MessageBox.Show("Некорректный формат даты.");
                                break;
                            }

                            if (id != -1) // Пропускаем удаленные записи
                            {
                                dataGridView1.Rows.Add(id, name, author, price, startdate.ToString("yyyy-MM-dd"), enddate.ToString("yyyy-MM-dd"));
                            }
                        }
                        catch (EndOfStreamException)
                        {
                            MessageBox.Show("Достигнут конец файла при чтении данных.");
                            break;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при чтении записи: {ex.Message}");
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии файла: {ex.Message}");
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns.Clear(); // Очищаем предыдущие колонки
            dataGridView1.Columns.Add("ID", "ID");
            dataGridView1.Columns.Add("Name", "Название");
            dataGridView1.Columns.Add("Location", "Локация");
            dataGridView1.Columns.Add("Price", "Цена");
            dataGridView1.Columns.Add("StartDate", "Дата заезда");
            dataGridView1.Columns.Add("EndDate", "Дата выезда");

            comboBox.Items.AddRange(new string[] { "ID", "Название", "Локация", "Цена", "Дата заезда", "Дата выезда" });
            comboBox.SelectedIndex = 0;

            LoadDataToGrid();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string selectedField = comboBox.SelectedItem?.ToString(); // смотрим по какому параметру ищем
            List<CampShiftRecord?> results = new List<CampShiftRecord?>();
            string searchValueString;
            double searchValueDouble;
            int searchValueInt;
            DateTime searchValueDateTime;
            switch (selectedField)
            {
                case "Название":
                    searchValueString = textBoxName.Text.Trim();
                    if (!string.IsNullOrEmpty(searchValueString))
                    {
                        results = manager.SearchRecordsByName(searchValueString);
                    }
                    else
                    {
                        MessageBox.Show("Выбранное поле пусто.");
                    }
                    break;
                case "Локация":
                    searchValueString = textBoxAuthor.Text.Trim();
                    if (!string.IsNullOrEmpty(searchValueString))
                    {
                        results = manager.SearchRecordsByAuthor(searchValueString);
                    }
                    else
                    {
                        MessageBox.Show("Выбранное поле пусто.");
                    }
                    break;
                case "ID":
                    if (!int.TryParse(textBoxID.Text, out searchValueInt))
                    {
                        MessageBox.Show("Введите корректный числовой ID.");
                        return;
                    }
                    results = manager.SearchRecordById(searchValueInt);
                    break;
                case "Цена":
                    if (!double.TryParse(textBoxPrice.Text, out searchValueDouble))
                    {
                        MessageBox.Show("Введите корректную цену.");
                        return;
                    }
                    results = manager.SearchRecordsByPrice(searchValueDouble);
                    break;
                case "Дата заезда":
                    if (!DateTime.TryParse(dateTimePicker.Text, out searchValueDateTime))
                    {
                        MessageBox.Show("Ошибка при чтении даты.");
                        return;
                    }
                    results = manager.SearchRecordsByStartDate(searchValueDateTime.Date.ToString("yyyy-MM-dd"));
                    break;
                case "Дата выезда":
                    if (!DateTime.TryParse(endDateTimePicker.Text, out searchValueDateTime))
                    {
                        MessageBox.Show("Ошибка при чтении даты.");
                        return;
                    }
                    results = manager.SearchRecordsByStartDate(searchValueDateTime.Date.ToString("yyyy-MM-dd"));
                    break;
                default:
                    MessageBox.Show("Выбранное поле не поддерживается для поиска.");
                    return;
            }
            if (results.Count > 0)
            {
                string res = $"Найдена запись:  \n";
                int i = 1;
                foreach (CampShiftRecord record in results)
                {
                    res += $"{i}) Название: {record.Name}, автор: {record.Location}," +
                        $" цена: {record.Price}, дата заезда: {record.StartDate.ToString("yyyy-MM-dd")}, дата выезда: {record.EndDate.ToString("yyyy - MM - dd")}\n";
                    i++;
                }
                MessageBox.Show(res);
            }
            else
            {
                MessageBox.Show("Запись не найдена!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                // Считываем данные из текстовых полей
                if (!int.TryParse(textBoxID.Text, out int id))
                {
                    MessageBox.Show("Введите корректный ID.");
                    return;
                }

                string name = textBoxName.Text.Trim();
                string author = textBoxAuthor.Text.Trim();

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(author))
                {
                    MessageBox.Show("Название и локация смены не могут быть пустыми.");
                    return;
                }

                if (!double.TryParse(textBoxPrice.Text, out double price) || price <= 0)
                {
                    MessageBox.Show("Введите корректную цену.");
                    return;
                }

                DateTime startdate = dateTimePicker.Value;
                DateTime enddate = endDateTimePicker.Value;

                // Проверяем, существует ли запись с данным ID
                var existingRecord = manager.SearchRecordById(id);

                if (existingRecord.Count > 0)
                {
                    MessageBox.Show("Запись с таким ID уже существует.");
                    return;
                }

                // Создаем новую запись
                var newRecord = new CampShiftRecord(id, name, author, price, startdate, enddate);

                // Добавляем запись в файл
                manager.WriteRecord(newRecord, "camp_database.bin");
                LoadDataToGrid(); // Обновляем таблицу
            }
            catch (FormatException)
            {
                MessageBox.Show("Некорректный формат данных. Проверьте введённые значения.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void EnterID_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxID_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string selectedField = comboBox.SelectedItem?.ToString();
            string searchValueString;
            double searchValueDouble;
            int searchValueInt;
            DateTime searchValueDate;
            switch (selectedField)
            {
                case "Название":
                    searchValueString = textBoxName.Text.Trim();
                    if (!string.IsNullOrEmpty(searchValueString))
                    {
                        manager.DeleteRecordbyName(searchValueString);
                    }
                    else
                    {
                        MessageBox.Show("Выбранное поле пусто.");
                    }
                    break;

                case "Локация":
                    searchValueString = textBoxAuthor.Text.Trim();
                    if (!string.IsNullOrEmpty(searchValueString))
                    {
                        manager.DeleteRecordbyAuthor(searchValueString);
                    }
                    else
                    {
                        MessageBox.Show("Выбранное поле пусто.");
                    }
                    break;

                case "ID":
                    if (!int.TryParse(textBoxID.Text, out searchValueInt))
                    {
                        MessageBox.Show("Введите корректный числовой ID.");
                        return;
                    }
                    manager.DeleteRecordbyId(searchValueInt);
                    break;

                case "Цена":
                    if (!double.TryParse(textBoxPrice.Text, out searchValueDouble))
                    {
                        MessageBox.Show("Введите корректную цену.");
                        return;
                    }
                    manager.DeleteRecordbyPrice(searchValueDouble);
                    break;
                case "Дата заезда":
                    if (!DateTime.TryParse(dateTimePicker.Text, out searchValueDate))
                    {
                        MessageBox.Show("Некорректная дата.");
                        return;
                    }
                    manager.DeleteRecordbyStartDate(searchValueDate.Date);
                    break;
                case "Дата выезда":
                    if (!DateTime.TryParse(endDateTimePicker.Text, out searchValueDate))
                    {
                        MessageBox.Show("Некорректная дата.");
                        return;
                    }
                    manager.DeleteRecordbyEndDate(searchValueDate.Date);
                    break;

                default:
                    MessageBox.Show("Выбранное поле не поддерживается для удаления.");
                    return;
            }
            LoadDataToGrid(); // Обновляем таблицу
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear(); // Очищаем строки
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("ID", "ID");
            dataGridView1.Columns.Add("Name", "Название");
            dataGridView1.Columns.Add("Location", "Локация");
            dataGridView1.Columns.Add("Price", "Цена");
            dataGridView1.Columns.Add("StartDate", "Дата заезда");
            dataGridView1.Columns.Add("EndDate", "Дата выезда");
            manager.Clear();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void EnterPrice_Click(object sender, EventArgs e)
        {

        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("В поля для записи ввдите ID (обязательно) и необходимые поля записи, которую хотите отредактировать.");
            try
            {
                if (!int.TryParse(textBoxID.Text, out int id))
                {
                    MessageBox.Show("Введите корректный ID.");
                    return;
                }
                string? newName = string.IsNullOrWhiteSpace(textBoxName.Text) ? null : textBoxName.Text; // Поле для нового имени
                string? newLocation = string.IsNullOrWhiteSpace(textBoxAuthor.Text) ? null : textBoxAuthor.Text; // Поле для нового автора
                double? newPrice = string.IsNullOrWhiteSpace(textBoxPrice.Text) ? null : double.Parse(textBoxPrice.Text); // Поле для новой цены
                DateTime? newStartDate = string.IsNullOrWhiteSpace(dateTimePicker.Text) ? null : DateTime.Parse(dateTimePicker.Text); // Поле для новой даты
                DateTime? newEndDate = string.IsNullOrWhiteSpace(endDateTimePicker.Text) ? null : DateTime.Parse(dateTimePicker.Text);
                bool result = manager.EditRecordById(id, newName, newLocation, newPrice, newStartDate, newEndDate);

                if (result)
                {
                    LoadDataToGrid(); // Обновляем таблицу
                    MessageBox.Show("Запись успешно отредактирована.");
                }
                else
                {
                    MessageBox.Show("Не удалось отредактировать запись.");
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show($"Ошибка ввода данных: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            manager.SaveToXlsx();
        }

        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
