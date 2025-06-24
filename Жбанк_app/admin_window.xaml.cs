using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static MaterialDesignThemes.Wpf.Theme;
using static Жбанк_app.admin_window;
using static Жбанк_app.MainWindow;

namespace Жбанк_app
{
    /// <summary>
    /// Логика взаимодействия для admin_window.xaml
    /// </summary>
    public partial class admin_window : Window
    {
        public admin_window()
        {
            InitializeComponent();
        }
        public class users
        {
            public int user_id { get; set; }
            public string card_num { get; set; }
            public string pin { get; set; }
            public decimal ballance { get; set; }
            public int role_id { get; set; }
            public string role { get; set; }
        }
        public List<users> GetDataUsers()
        {
            var data = new List<users>();
            using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
            {
                sqlcon.Open();
                NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM users", sqlcon);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    data.Add(new users
                    {
                        user_id = reader.GetInt32(0),
                        card_num = reader.GetString(1),
                        pin = reader.GetString(2),
                        ballance = reader.GetDecimal(3),
                        role_id = reader.GetInt32(4),
                        role = reader.GetString(5)
                    });
                }
            }
            return data;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                UsersTable.ItemsSource = GetDataUsers(); 
                JKHTable.ItemsSource = GetDataJkh(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных в таблицу: {ex.Message}");
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            {
                MainWindow MW = new MainWindow();
                MW.Show();
                this.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UsersTable.ItemsSource = GetDataUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}");
            }
        }

        private void AddUsers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string card_num = NumCardBox.Text;
                string pin = PinBox.Text;
                int role_id = Convert.ToInt32(RoleIdBox.Text);
                string logtxt = DataContainer.logtxt;
                string Pintxt = DataContainer.pintxt;
                decimal ballance = 0;
                int max_user_id;
                string role;

                if (role_id < 1 || role_id > 2)
                {
                    MessageBox.Show("Введите корректный ID роли где 1 admin, а 2 user");
                    RoleIdBox.Clear();
                }
                else if (card_num.Length != 16)
                {
                    MessageBox.Show("Введите корректный номер карты длинной 16 символов");
                    NumCardBox.Clear();
                }
                else if (pin.Length != 4)
                {
                    MessageBox.Show("Введите корректный pin длинной 4 символа");
                    PinBox.Clear();
                }
                else
                {
                    using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
                    {
                        sqlcon.Open();

                        NpgsqlCommand command = new NpgsqlCommand("SELECT MAX(user_id) as max_user_id FROM users", sqlcon);
                        max_user_id = (int)command.ExecuteScalar();
                    }
                    max_user_id += 1;
                    using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
                    {
                        sqlcon.Open();

                        NpgsqlCommand command = new NpgsqlCommand("SELECT role FROM roles WHERE role_id = @role_id", sqlcon);
                        command.Parameters.AddWithValue("@role_id", role_id);
                        role = (string)command.ExecuteScalar();
                    }
                    using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
                    {
                        sqlcon.Open();

                        NpgsqlCommand command = new NpgsqlCommand("INSERT INTO users (user_id, card_num, pin, ballance, role_id, role) " +
                            "VALUES (@user_id, @card_num, @pin, @ballance, @role_id, @role)", sqlcon);
                        command.Parameters.AddWithValue("@user_id", max_user_id);
                        command.Parameters.AddWithValue("@card_num", card_num);
                        command.Parameters.AddWithValue("@pin", pin);
                        command.Parameters.AddWithValue("@ballance", ballance);
                        command.Parameters.AddWithValue("@role_id", role_id);
                        command.Parameters.AddWithValue("@role", role);
                        command.ExecuteNonQuery();

                    }
                    MessageBox.Show("Запись добавлена");
                    NumCardBox.Clear();
                    PinBox.Clear();
                    RoleIdBox.Clear();
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

        private void NumCardBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

        private void RoleIdBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        public class jkh
        {
            public int trans_jkh_id { get; set; }
            public int num_scheta { get; set; }
            public decimal sum_k_oplate { get; set; }
            public int gvs_counter { get; set; }
            public int hvs_counter { get; set; }
            public int day_counter { get; set; }
            public int night_counter { get; set; }
            public int user_id { get; set; }
            public bool oplachena { get; set; }

        }
        public List<jkh> GetDataJkh()
        {
            var data = new List<jkh>();
            using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
            {
                sqlcon.Open();
                NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM oplata_jkh", sqlcon);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    data.Add(new jkh
                    {
                        trans_jkh_id = reader.GetInt32(0),
                        num_scheta = reader.GetInt32(1),
                        sum_k_oplate = reader.GetDecimal(2),
                        gvs_counter = reader.GetInt32(3),
                        hvs_counter = reader.GetInt32(4),
                        day_counter = reader.GetInt32(5),
                        night_counter = reader.GetInt32(6),
                        user_id = reader.GetInt32(7),
                        oplachena = reader.GetBoolean(8)
                    });
                }
            }
            return data;
        }
        private void RefreshJKH_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                JKHTable.ItemsSource = GetDataJkh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}");
            }
        }

        private void AddJKH_Click(object sender, RoutedEventArgs e)
        {
            int NumScheta = Convert.ToInt32(NumSchetaBox.Text);
            string NumSchetaLenght = NumSchetaBox.Text;
            decimal K_oplate = Convert.ToDecimal(sumKoplateBox.Text);
            bool oplacheno = false;
            int gvs_counter = 0;
            int hvs_counter = 0;
            int day_counter = 0;
            int night_counter = 0;
            int user_id = 1;

            if (NumSchetaLenght.Length != 8)
            {
                MessageBox.Show("Введите корректный номер счёта длинной 8 символов");
                NumSchetaBox.Clear();
            }
            else
            {
                try
                {
                    using(NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
                    {
                        sqlcon.Open();

                        NpgsqlCommand command = new NpgsqlCommand("INSERT INTO oplata_jkh (num_scheta, sum_k_oplate, oplachena, gvs_counter, hvs_counter, day_counter, night_counter, user_id) " +
                            "VALUES (@num_scheta, @sum_k_oplate, @oplachena, @gvs_counter, @hvs_counter, @day_counter, @night_counter, @user_id)", sqlcon);
                        command.Parameters.AddWithValue("@num_scheta", NumScheta);
                        command.Parameters.AddWithValue("@sum_k_oplate", K_oplate);
                        command.Parameters.AddWithValue("@oplachena", oplacheno);
                        command.Parameters.AddWithValue("@gvs_counter", gvs_counter);
                        command.Parameters.AddWithValue("@hvs_counter", hvs_counter);
                        command.Parameters.AddWithValue("@day_counter", day_counter);
                        command.Parameters.AddWithValue("@night_counter", night_counter);
                        command.Parameters.AddWithValue("@user_id", user_id);
                        command.ExecuteNonQuery();
                        NumSchetaBox.Clear();
                        sumKoplateBox.Clear();
                    }
                    MessageBox.Show("Запись добавлена");
                    
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
        }

        private void sumKoplateBox_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ".")
               && (sumKoplateBox.Text.Contains(".")
               && sumKoplateBox.Text.Length != 0)))
            {
                e.Handled = true;
            }
        }

        private void NumSchetaBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
    }
}
