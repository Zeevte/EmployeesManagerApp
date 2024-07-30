using BL;
using DAL.Models;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UI
{
    public delegate bool delegateAddNewEmployees(Employee newEmployee);
    /// <summary>
    /// Interaction logic for AddEmployee.xaml
    /// </summary>
    public partial class AddEmployee : Window
    {
        private BLConnection blConnection;
        public ObservableCollection<Employee> Employees { get; set; }
        public List<string> Jobs { get; set; }


        public event delegateAddNewEmployees del;

        public AddEmployee()
        {
            InitializeComponent();
            blConnection = new BLConnection();
            Employees = new ObservableCollection<Employee>(blConnection.GetAllEmployees());
            //Jobs = blConnection.GetJobs();

        }

        public static bool FunDel(Employee newEmployee)
        {
            //blConnection.AddEmployee(newEmployee);
            return true;
        }


        public bool CheckId(string idNew)
        {
            foreach (var e in Employees)
            {
                if (e.Id.ToString() == idNew)
                    return true;
            }
            return false;
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {// ולידציה לשדה ה-ID
            if (IdTextBox.Text.Length != 9 || !IdTextBox.Text.All(char.IsDigit) || CheckId(IdTextBox.Text))
            {
                //{
                MessageBox.Show("This ID card is invalid or it already exists in the system.");
                return;
                //MessageBox.Show("errow id");
            }

            //else
            //{
            //    IdErrorTextBlock.Text = string.Empty;
            //}

            // ולידציה לשדה השם הפרטי
            if (FirstNameTextBox.Text.Length < 2 || !Regex.IsMatch(FirstNameTextBox.Text, @"^[a-zA-Z]+$"))
            {
                MessageBox.Show("First name must be at least 2 letters long and contain only letters.");
                return;
            }

            // ולידציה לשדה שם המשפחה
            if (LastNameTextBox.Text.Length < 2 || !Regex.IsMatch(LastNameTextBox.Text, @"^[a-zA-Z]+$"))
            {
                MessageBox.Show("Last name must be at least 2 letters long and contain only letters.");
                return;
            }

            // ולידציה לשדה הגיל
            if (!int.TryParse(AgeTextBox.Text, out int age) || age < 18 || age > 67)
            {
                MessageBox.Show("Age must be a number between 18 and 67.");
                return;
            }

            // ולידציה לשדה שנת תחילת העבודה
            if (!int.TryParse(StartOfWorkingYearTextBox.Text, out int startYear) || startYear < 1900 || startYear > DateTime.Now.Year)
            {
                MessageBox.Show($"Start of working year must be a number between 1900 and {DateTime.Now.Year}.");
                return;
            }

            // ולידציה לשדה התפקיד (בהנחה שיש לנו רשימת תפקידים מוגדרת מראש)
            if (!Jobs.Contains(JobTitleTextBox.Text))
            {
                MessageBox.Show("Job title must be one of the predefined job titles.");
                return;
            }

            // ולידציה לשדה מספר הפלאפון
            if (!Regex.IsMatch(PhoneNumberTextBox.Text, @"^\d{10}$"))
            {
                MessageBox.Show("Phone number must be exactly 10 digits.");
                return;
            }

            // ולידציה לשדה המייל
            if (!Regex.IsMatch(MailAddressTextBox.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email address.");
                return;
            }
            //string[] validJobTitles = blConnection.GetJobs().ToArray();


            Employee newEmployee = new Employee
            {
                Id = int.Parse(IdTextBox.Text),
                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Age = age,
                StartOfWorkYear = startYear,
                City = CityAddressTextBox.Text,
                Street = StreetAddressTextBox.Text,
                RoleInCompany = JobTitleTextBox.Text,
                PhoneNumber = PhoneNumberTextBox.Text,
                Email = MailAddressTextBox.Text
            };
            del(newEmployee);


            MessageBox.Show("Employee added successfully!");
            ClearForm();

        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        private void ClearForm()
        {
            IdTextBox.Clear();
            FirstNameTextBox.Clear();
            LastNameTextBox.Clear();
            AgeTextBox.Clear();
            StartOfWorkingYearTextBox.Clear();
            CityAddressTextBox.Clear();
            StreetAddressTextBox.Clear();
            JobTitleTextBox.Clear();
            PhoneNumberTextBox.Clear();
            MailAddressTextBox.Clear();
        }
    }
}