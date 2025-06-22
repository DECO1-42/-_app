using Npgsql;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using static Жбанк_app.MainWindow;

namespace Жбанк_app
{
    /// <summary>
    /// Логика взаимодействия для ViborOperacii.xaml
    /// </summary>
    public partial class ViborOperacii : Window
    {
        public ViborOperacii()
        {
            InitializeComponent();
        }

        private void SniatBtn_Click(object sender, RoutedEventArgs e) //Обработчик события для перехода на форму снятия
        {
            Sniatie sn = new Sniatie();
            sn.Show();
            this.Close();
        }

        private void PopolBtn_Click(object sender, RoutedEventArgs e) //Обработчик события для перехода на форму пополнения
        {
            Popolnenie pop = new Popolnenie();
            pop.Show();
            this.Close();
        }

        private void Back_Click(object sender, RoutedEventArgs e) //Обработчик события для перехода на форму авторизации
        {
            MainWindow MW = new MainWindow();
            MW.Show();
            this.Close();
        }

        private void Balance_Click(object sender, RoutedEventArgs e) //Кнопка для отображения баланса
        {
            int user_id;
            decimal ballance;
            string logtxt = DataContainer.logtxt;
            string Pintxt = DataContainer.pintxt;
            using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
            {
                sqlcon.Open();

                NpgsqlCommand command = new NpgsqlCommand("SELECT user_id FROM users WHERE card_num = @login and pin = @Pin", sqlcon);
                command.Parameters.AddWithValue("@login", logtxt);
                command.Parameters.AddWithValue("@Pin", Pintxt);
                user_id = (int)command.ExecuteScalar();
            }
            using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
            {
                sqlcon.Open();

                NpgsqlCommand command = new NpgsqlCommand("SELECT ballance FROM users WHERE user_id = @user_id;", sqlcon);
                command.Parameters.AddWithValue("@user_id", user_id);
                ballance = (decimal)command.ExecuteScalar();
            }
            Balance.Content = ballance;
        }

        private void PhonePopolBtn_Click(object sender, RoutedEventArgs e)
        {
            PhonePopolnenie PP = new PhonePopolnenie();
            PP.Show();
            this.Close();
        }

        private void JkhBtn_Click(object sender, RoutedEventArgs e)
        {
            Oplata_jkh OJ = new Oplata_jkh();
            OJ.Show();
            this.Close();
        }
    }
}
