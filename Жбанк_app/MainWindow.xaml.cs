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
            public static string logtxt { get; set; }
            public static string pintxt { get; set; }
        }

        private void Vhod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string logtxt = Convert.ToString(LogBox.Text);
                string Pintxt = Convert.ToString(PinBox.Text);


                using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
                {
                    sqlcon.Open();

                    NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM users WHERE card_num = @login and pin = @Pin", sqlcon);
                    command.Parameters.AddWithValue("@login", logtxt);
                    command.Parameters.AddWithValue("@Pin", Pintxt);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        DataContainer.logtxt = logtxt;
                        DataContainer.pintxt = Pintxt;
                        ViborOperacii VibOp = new ViborOperacii();
                        VibOp.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль");
                        LogBox.Clear();
                        PinBox.Clear();
                    }

                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Ошибка базы данных: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неожиданная ошибка: {ex.Message}");
            }
        }

        private void LogBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void PinBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
    }
}
