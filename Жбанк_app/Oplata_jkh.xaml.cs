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
    /// Логика взаимодействия для Oplata_jkh.xaml
    /// </summary>
    public partial class Oplata_jkh : Window
    {
        
        
        public Oplata_jkh()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ViborOperacii VO = new ViborOperacii();
            VO.Show();
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

        private void SchetChek_Click(object sender, RoutedEventArgs e)
        {
            try //Блок try-catch для обработки возможных ошибок при работе программы
            {
                int max_trans_jkh_id;
                decimal sum_k_oplate;
                bool Oplacheno;
                int num_scheta = Convert.ToInt32(SchetChekBox.Text);
                string K_Oplate = "К оплате:";;
                using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
                {
                    sqlcon.Open();

                    NpgsqlCommand command = new NpgsqlCommand("SELECT MAX(trans_jkh_id) as max_trans_jkh_id FROM oplata_jkh WHERE num_scheta = @num_scheta", sqlcon);
                    command.Parameters.AddWithValue("@num_scheta", num_scheta);
                    max_trans_jkh_id = (int)command.ExecuteScalar();
                }
                using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
                {
                    sqlcon.Open();

                    NpgsqlCommand command = new NpgsqlCommand("SELECT sum_k_oplate FROM oplata_jkh WHERE trans_jkh_id = @max_trans_jkh_id", sqlcon);
                    command.Parameters.AddWithValue("@max_trans_jkh_id", max_trans_jkh_id);
                    sum_k_oplate = (decimal)command.ExecuteScalar();
                }
                using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
                {
                    sqlcon.Open();

                    NpgsqlCommand command = new NpgsqlCommand("SELECT oplachena FROM oplata_jkh WHERE trans_jkh_id = @max_trans_jkh_id", sqlcon);
                    command.Parameters.AddWithValue("@max_trans_jkh_id", max_trans_jkh_id);
                    Oplacheno = (bool)command.ExecuteScalar();
                }

                if (Oplacheno == true)
                {
                    MessageBox.Show("Данная квитанция уже оплачена");
                    Oplata_jkh OJ = new Oplata_jkh();
                    OJ.Show();
                    this.Close();
                }
                else
                {
                    K_Oplate = K_Oplate + Convert.ToString(sum_k_oplate);
                    KOplate.Content = K_Oplate;
                    MessageBox.Show("Номер счёта корректный");
                }
            }
            catch (NpgsqlException ex) //Блок обработки ошибок при работе програмы
            {
                MessageBox.Show($"Ошибка базы данных: {ex.Message}");
                SchetChekBox.Clear();
            }
            catch (Exception ex) //Блок обработки ошибок при работе програмы
            {
                MessageBox.Show($"Неожиданная ошибка: {ex.Message}");
                SchetChekBox.Clear();
            }

        }

        private void SchetChekBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void ElDayBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void ElNightBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void HVSBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void GVSBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void OplatitBtn_Click(object sender, RoutedEventArgs e)
        {
            try //Блок try-catch для обработки возможных ошибок при работе программы
            {
                int max_trans_jkh_id;
                decimal sum_k_oplate;
                int num_scheta = Convert.ToInt32(SchetChekBox.Text);
                int user_id;
                decimal ballance;
                string OplataJKH = "Оплата ЖКХ";
                string logtxt = DataContainer.logtxt;
                string Pintxt = DataContainer.pintxt;
                int GVS = Convert.ToInt32(GVSBox.Text);
                int HVS = Convert.ToInt32(HVSBox.Text);
                int ElDay = Convert.ToInt32(ElDayBox.Text);
                int ElNight = Convert.ToInt32(ElNightBox.Text);
                bool Oplacheno;
                int last_trans_id;
                int ostatok_v_bankomate;

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
                using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
                {
                    sqlcon.Open();

                    NpgsqlCommand command = new NpgsqlCommand("SELECT MAX(trans_jkh_id) as max_trans_jkh_id FROM oplata_jkh WHERE num_scheta = @num_scheta", sqlcon);
                    command.Parameters.AddWithValue("@num_scheta", num_scheta);
                    max_trans_jkh_id = (int)command.ExecuteScalar();
                }
                using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
                {
                    sqlcon.Open();

                    NpgsqlCommand command = new NpgsqlCommand("SELECT sum_k_oplate FROM oplata_jkh WHERE trans_jkh_id = @max_trans_jkh_id", sqlcon);
                    command.Parameters.AddWithValue("@max_trans_jkh_id", max_trans_jkh_id);
                    sum_k_oplate = (decimal)command.ExecuteScalar();
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

                    NpgsqlCommand command = new NpgsqlCommand("SELECT oplachena FROM oplata_jkh WHERE trans_jkh_id = @max_trans_jkh_id", sqlcon);
                    command.Parameters.AddWithValue("@max_trans_jkh_id", max_trans_jkh_id);
                    Oplacheno = (bool)command.ExecuteScalar();
                }
                if (ballance - sum_k_oplate < 0) //Проверка на наличие достаточной суммы на балансе
                {
                    MessageBox.Show("На вашем счёте недостаточно средств");
                    Oplata_jkh OJ = new Oplata_jkh();
                    OJ.Show();
                    this.Close();
                }
                else if (Oplacheno == true)
                {
                    MessageBox.Show("Данная квитанция уже оплачена");
                    Oplata_jkh OJ = new Oplata_jkh();
                    OJ.Show();
                    this.Close();
                }
                else
                {
                    ballance -= sum_k_oplate;
                    Oplacheno = true;

                    using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
                    {
                        sqlcon.Open();
                        NpgsqlCommand command = new NpgsqlCommand("INSERT INTO TRANSACTIONS (USER_ID, TIP_OPERACII, SUMMA_OPERACII, OSTATOK_V_BANKOMATE, trans_jkh_id) " +
                            "VALUES (@user_id, @OplataJKH, @sum, @ostatok_v_bankomate, @trans_jkh_id);", sqlcon);
                        command.Parameters.AddWithValue("@user_id", user_id);
                        command.Parameters.AddWithValue("@OplataJKH", OplataJKH);
                        command.Parameters.AddWithValue("@sum", sum_k_oplate);
                        command.Parameters.AddWithValue("@ostatok_v_bankomate", ostatok_v_bankomate);
                        command.Parameters.AddWithValue("@trans_jkh_id", max_trans_jkh_id);
                        command.ExecuteNonQuery();
                    }
                    using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
                    {
                        sqlcon.Open();
                        NpgsqlCommand command = new NpgsqlCommand("UPDATE OPLATA_JKH SET  gvs_counter=@GVS, hvs_counter=@HVS, day_counter=@ElDay, night_counter=@ElNight, oplachena=@Oplacheno, user_id = @user_id " +
                            "WHERE trans_jkh_id = @max_trans_jkh_id ", sqlcon);
                        command.Parameters.AddWithValue("@GVS", GVS);
                        command.Parameters.AddWithValue("@HVS", HVS);
                        command.Parameters.AddWithValue("@ElDay", ElDay);
                        command.Parameters.AddWithValue("@ElNight", ElNight);
                        command.Parameters.AddWithValue("@Oplacheno", Oplacheno);
                        command.Parameters.AddWithValue("@user_id", user_id);
                        command.Parameters.AddWithValue("@max_trans_jkh_id", max_trans_jkh_id);
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

                    MessageBox.Show("Оплата спешно завершена"); ;
                    MainWindow MaWi = new MainWindow();
                    MaWi.Show();
                    this.Close();
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
    }
}
