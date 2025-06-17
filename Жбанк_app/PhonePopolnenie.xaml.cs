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
using static MaterialDesignThemes.Wpf.Theme;
using static Жбанк_app.MainWindow;

namespace Жбанк_app
{
    /// <summary>
    /// Логика взаимодействия для PhonePopolnenie.xaml
    /// </summary>
    public partial class PhonePopolnenie : Window
    {
        public PhonePopolnenie()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ViborOperacii VO = new ViborOperacii();
            VO.Show();
            this.Close();
        }

        private void PopolBtn_Click(object sender, RoutedEventArgs e)
        {
            try //Блок try-catch для обработки возможных ошибок при работе программы
            {
                int sum = Convert.ToInt32(sumBox.Text);
                string phone = Convert.ToString(8);
                phone += Convert.ToString(PhoneNum.Text);
                int user_id;
                int last_trans_id;
                int last_phone_trans_id;
                int ostatok_v_bankomate;
                decimal ballance;
                string PhonePopolnenie = "Пополнение номера телефона";
                string logtxt = DataContainer.logtxt;
                string Pintxt = DataContainer.pintxt;

                if (sum % 100 != 0) //Проверка на корректность "вносимых" купюр
                {
                    sum = 0; //Сумма операции приравнивается к нулю для избежания ошибочных транзакций
                    MessageBox.Show("Данные купюры не обрабатываются");
                    PhoneNum.Clear();
                    sumBox.Clear();
                }
                else if (sum == 0) //Проверка на корректность введённой
                {
                    sum = 0; //Сумма операции приравнивается к нулю для избежания ошибочных транзакций
                    MessageBox.Show("Пожалуйста внесите сумму кратную 100");
                    PhoneNum.Clear();
                    sumBox.Clear();
                }
                else if (PhoneNum.Text.Length < 10)//Проверка на полноценность номера телефона
                {
                    sum = 0; //Сумма операции приравнивается к нулю для избежания ошибочных транзакций
                    MessageBox.Show("Пожалуйста введите полный номер телефона");
                    PhoneNum.Clear();
                    sumBox.Clear();
                }
                else
                {

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
                    using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
                    {
                        sqlcon.Open();

                        NpgsqlCommand command = new NpgsqlCommand("SELECT MAX(phone_trans_id) as max_phone_trans_id FROM phone_trans;", sqlcon);
                        last_phone_trans_id = (int)command.ExecuteScalar();
                    }
                    ostatok_v_bankomate += sum;
                    last_phone_trans_id += 1;
                    using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
                    {
                        sqlcon.Open();

                        NpgsqlCommand command = new NpgsqlCommand("INSERT INTO PHONE_TRANS (PHONE_NUM, SUMMA, USER_ID) " +
                            "VALUES (@phone, @sum, @user_id);", sqlcon);
                        command.Parameters.AddWithValue("@user_id", user_id);
                        command.Parameters.AddWithValue("@sum", sum);
                        command.Parameters.AddWithValue("@phone", phone);
                        command.ExecuteNonQuery();
                    }
                    using (NpgsqlConnection sqlcon = new NpgsqlConnection(Properties.Settings.Default.JbankAppConnectionString))
                    {
                        sqlcon.Open();

                        NpgsqlCommand command = new NpgsqlCommand("INSERT INTO TRANSACTIONS (USER_ID, TIP_OPERACII, SUMMA_OPERACII, OSTATOK_V_BANKOMATE, phone_trans_id) " +
                            "VALUES (@user_id, @PhonePopolnenie, @sum, @ostatok_v_bankomate, @last_phone_trans_id);", sqlcon);
                        command.Parameters.AddWithValue("@user_id", user_id);
                        command.Parameters.AddWithValue("@PhonePopolnenie", PhonePopolnenie);
                        command.Parameters.AddWithValue("@sum", sum);
                        command.Parameters.AddWithValue("@ostatok_v_bankomate", ostatok_v_bankomate);
                        command.Parameters.AddWithValue("@last_phone_trans_id", last_phone_trans_id);
                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show($"Вы пополнили баланс на: {sum}");

                    MainWindow MW = new MainWindow();
                    MW.Show();
                    this.Close();
                }
            }
            catch (NpgsqlException ex) //Блок обработки ошибок при работе програмы
            {
                MessageBox.Show($"Ошибка базы данных: {ex.Message}");
                sumBox.Clear();
                PhoneNum.Clear();
            }
            catch (Exception ex) //Блок обработки ошибок при работе програмы
            {
                MessageBox.Show($"Неожиданная ошибка: {ex.Message}");
                sumBox.Clear();
                PhoneNum.Clear();
            }
            finally //Событие выполняемое независимо от того сработала программа корректно или нет
            {
                sumBox.Clear();
                PhoneNum.Clear();
            }
        }

        private void PhoneNum_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
    }
}
