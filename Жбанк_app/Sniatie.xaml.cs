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
    /// Логика взаимодействия для Sniatie.xaml
    /// </summary>
    public partial class Sniatie : Window
    {
        public Sniatie()
        {
            InitializeComponent();
        }

        private void SniatieBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int sum = Convert.ToInt32(sumBox.Text);
                int user_id;
                int last_trans_id;
                int ostatok_v_bankomate;
                decimal ballance;
                string Sniatie = "Снятие";
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
                    NpgsqlCommand command = new NpgsqlCommand("SELECT MAX(trans_id) as max_trans_id FROM transactions;", sqlcon);
                    last_trans_id = (int)command.ExecuteScalar();
                }
                using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
                {
                    sqlcon.Open();
                    NpgsqlCommand command = new NpgsqlCommand("SELECT ostatok_v_bankomate FROM transactions WHERE trans_id = @last_trans_id;", sqlcon);
                    command.Parameters.AddWithValue("@last_trans_id", last_trans_id);
                    ostatok_v_bankomate = (int)command.ExecuteScalar();
                }
                using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
                {
                    sqlcon.Open();
                    NpgsqlCommand command = new NpgsqlCommand("SELECT ballance FROM users WHERE user_id = @user_id;", sqlcon);
                    command.Parameters.AddWithValue("@user_id", user_id);
                    ballance = (decimal)command.ExecuteScalar();
                }
                if (ballance - sum < 0)
                {
                    sum = 0;
                    MessageBox.Show("На вашем счёте недостаточно средств");
                    MainWindow MaW = new MainWindow();
                    MaW.Show();
                    this.Close();
                }
                else if (ostatok_v_bankomate - sum < 0)
                {
                    sum = 0;
                    MessageBox.Show("Извините в банкомате недостаточно средств");
                    MainWindow MaW = new MainWindow();
                    MaW.Show();
                    this.Close();
                }
                else if (sum % 100 != 0)
                {
                    sum = 0;
                    MessageBox.Show("Пожалуйста введите сумму кратную 100");
                    Sniatie Sn = new Sniatie();
                    Sn.Show();
                    this.Close();
                }
                else
                {
                    ballance -= sum;
                    ostatok_v_bankomate -= sum;

                    using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
                    {
                        sqlcon.Open();
                        NpgsqlCommand command = new NpgsqlCommand("INSERT INTO TRANSACTIONS (USER_ID, TIP_OPERACII, SUMMA_OPERACII, OSTATOK_V_BANKOMATE) " +
                            "VALUES (@user_id, @Sniatie, @sum, @ostatok_v_bankomate);", sqlcon);
                        command.Parameters.AddWithValue("@user_id", user_id);
                        command.Parameters.AddWithValue("@Sniatie", Sniatie);
                        command.Parameters.AddWithValue("@sum", sum);
                        command.Parameters.AddWithValue("@ostatok_v_bankomate", ostatok_v_bankomate);
                        command.ExecuteNonQuery();
                    }
                    using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
                    {
                        sqlcon.Open();
                        NpgsqlCommand command = new NpgsqlCommand("UPDATE USERS SET  ballance=@ballance WHERE user_id = @user_id ", sqlcon);
                        command.Parameters.AddWithValue("@ballance", ballance);
                        command.Parameters.AddWithValue("@user_id", user_id);
                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show($"Возьмите наличные: {sum}\n " +
                        $"Ваш баланс: {ballance}"); ;
                    MainWindow MaWi = new MainWindow();
                    MaWi.Show();
                    this.Close();
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Ошибка базы данных: {ex.Message}");
                sumBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неожиданная ошибка: {ex.Message}");
                sumBox.Clear();
            }
            finally
            {
              sumBox.Clear();
            }
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

        private void sumBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ViborOperacii VO = new ViborOperacii();
            VO.Show();
            this.Close();
        }
    }
}
