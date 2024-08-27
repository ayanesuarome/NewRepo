﻿using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfResponsive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string connectionString = $@"
            Data Source=localhost,5533;
            Initial Catalog=Northwind;
            Persist Security Info=False;
            MultipleActiveResultSets = false;
            User ID=sa;
            Password=NorthWind123456789^;
            Encrypt=True;
            Trust Server Certificate=True";

        private const string sql = @"
            WAITFOR DELAY '00:00:05';
            SELECT EmployeeId, FirstName, LastName FROM Employees";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GetEmployeesSyncButton_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch timer = Stopwatch.StartNew();

            using (SqlConnection connection = new(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        string employee = string.Format(
                            "{0}: {1} {2}",
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2));
                        EmployeesListBox.Items.Add(employee);
                    }

                    reader.Close();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            EmployeesListBox.Items.Add($"Sync: {timer.ElapsedMilliseconds:N0}ms");
        }

        private async void GetEmployeesAsyncButton_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch timer = Stopwatch.StartNew();

            using (SqlConnection connection = new(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    SqlCommand command = new(sql, connection);
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        string employee = string.Format(
                            "{0}: {1} {2}",
                            await reader.GetFieldValueAsync<int>(0),
                            await reader.GetFieldValueAsync<string>(1),
                            await reader.GetFieldValueAsync<string>(2));
                        EmployeesListBox.Items.Add(employee);
                    }

                    await reader.CloseAsync();
                    await connection.CloseAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            EmployeesListBox.Items.Add($"Sync: {timer.ElapsedMilliseconds:N0}ms");
        }
    }
}