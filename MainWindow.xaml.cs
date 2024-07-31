using BL;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;


namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ICollectionView _employeeView;
        private string _selectedJob;
        private string _selectedFilter;

        public BLConnection blclass { get; set; }
        public ObservableCollection<Employee> Employees { get; set; }
        public List<Candidate> Candidate { get; set; }
        public List<Interview> Interview { get; set; }
        public List<string> Jobs { get; set; }
        public ObservableCollection<string> Test { get; set; }

        public List<string> Skills { get; set; }

        public List<string> filterBy { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            blclass = new BLConnection();
            Employees = new ObservableCollection<Employee>(blclass.GetAllEmployees());
            Candidate = blclass.GetAllCandidates();
            Interview = blclass.GetAllInterviews();
            Jobs = blclass.GetJobs();
            //filterBy = GetFilterByobs();
            Test = new ObservableCollection<string>();
            Skills = blclass.GetFilterBy();

            _employeeView = CollectionViewSource.GetDefaultView(Employees);
            _employeeView.Filter = EmployeeFilter;
        }

        public List<string> GetFilterByobs(string filter)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                string propertyName = filter;
                List<string> listFilter = new List<string>();

                foreach (var employee in Employees)
                {
                    PropertyInfo prop = employee.GetType().GetProperty(propertyName);
                    if (prop != null)
                    {
                        var value = prop.GetValue(employee)?.ToString();
                        if (value != null && value.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            listFilter.Add(value);
                        }
                    }
                }
                return listFilter.Distinct().ToList();
            }
            return new List<string>();
        }
        public string SelectedJob
        {
            get { return _selectedJob; }
            set
            {
                if (_selectedJob != value)
                {
                    _selectedJob = value;
                    OnPropertyChanged(nameof(SelectedJob));
                    UpdateTestCollection(_selectedJob);
                    _employeeView.Refresh();
                }
            }
        }

        private bool EmployeeFilter(object item)
        {
            if (string.IsNullOrEmpty(SelectedJob))
                return true;

            var employee = item as Employee;
            return employee != null && employee.RoleInCompany == SelectedJob;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void UpdateTestCollection(string selectedJob)
        {
            Test.Clear();
            Test.Add(selectedJob);
        }

        private void ComboBoxFilterCategory_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                filterBy = GetFilterByobs(e.AddedItems[0].ToString());
                PropertyChanged(this, new PropertyChangedEventArgs("filterBy"));
            }
            _employeeView.Refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            AddEmployee addEmployeeWin = new AddEmployee();
            addEmployeeWin.del += AddEmployeeWin_del;
            addEmployeeWin.ShowDialog();
            Employees = new ObservableCollection<Employee>(blclass.GetAllEmployees());
            OnPropertyChanged("Employees");
        }

        private bool AddEmployeeWin_del(Employee newEmployee)
        {
            return blclass.AddEmployee(newEmployee);

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FindCandidate findCandidate = new FindCandidate();
            findCandidate.ShowDialog();
        }
    }
}