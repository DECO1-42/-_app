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

        private void SniatBtn_Click(object sender, RoutedEventArgs e)
        {
            Sniatie sn = new Sniatie();
            sn.Show();
            this.Close();
        }

        private void PopolBtn_Click(object sender, RoutedEventArgs e)
        {
            Popolnenie pop = new Popolnenie();
            pop.Show();
            this.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
  
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow MW = new MainWindow();
            MW.Show();
            this.Close();
        }

        private void Balance_Click(object sender, RoutedEventArgs e)
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
    }
}
