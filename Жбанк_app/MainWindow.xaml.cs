using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Жбанк_app
{
   
    public partial class MainWindow : Window
    {
       
        public MainWindow()
        {
            InitializeComponent();
        }
        public static class DataContainer
        {
            public static string logtxt { get; set; } //Объявление дата контейнера для логина
            public static string pintxt { get; set; } //Объявление дата контейнера для пин-кода
        }

        private void Vhod_Click(object sender, RoutedEventArgs e) //Обработчик события нажаия на кнопку вход
        {
            try //Блок try-catch для обработки возможных ошибок при работе программы
            {
                string logtxt = Convert.ToString(LogBox.Text); //Переменная получающая логин вводимый пользователем в текст бокс
                string Pintxt = Convert.ToString(PinBox.Text); //Переменная получающая пин-код вводимый пользователем в текст бокс

              
                using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString)) //Строка подключения к БД
                {
                    sqlcon.Open();

                    NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM users WHERE card_num = @login and pin = @Pin", sqlcon); //Запрос авторизации
                    command.Parameters.AddWithValue("@login", logtxt);//Строка для защиты от sql инъекции
                    command.Parameters.AddWithValue("@Pin", Pintxt);//Строка для защиты от sql инъекции
                    NpgsqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())//Проверка, возвращаются ли данные по запросу 
                    {
                        DataContainer.logtxt = logtxt;//Дата контейнер передающий логин в другие формы
                        DataContainer.pintxt = Pintxt;//Дата контейнер передающий пин-код в другие формы
                        ViborOperacii VibOp = new ViborOperacii();//Объявление нового экземпляра формы выбор операции
                        VibOp.Show();
                        this.Close();
                    }
                    else //Событие в случае введения неверного пароля
                    {
                        MessageBox.Show("Неверный логин или пароль");
                        LogBox.Clear();
                        PinBox.Clear();
                    }

                }
            }
            catch (NpgsqlException ex) //Блок обработки ошибок при работе програмы
            {
                MessageBox.Show($"Ошибка базы данных: {ex.Message}");
            }
            catch (Exception ex) //Блок обработки ошибок при работе програмы
            {
                MessageBox.Show($"Неожиданная ошибка: {ex.Message}");
            }
        }

        private void LogBox_PreviewTextInput(object sender, TextCompositionEventArgs e) //Обработчик события запрещающий вводить символы кроме цифр
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void PinBox_PreviewTextInput(object sender, TextCompositionEventArgs e) //Обработчик события запрещающий вводить символы кроме цифр
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
    }
}
