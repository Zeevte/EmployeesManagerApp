using BL;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace UI
{
    public partial class FindCandidate : Window, INotifyPropertyChanged
    {
        public Candidate SelectedCandidate { get; set; }
        public string SelectedCandidate1 { get; set; }
        public List<Employee> Employees { get; set; }
        public BLConnection blclass { get; set; }
        public List<Candidate> Candidates { get; set; } 
        public List<Interview> Interviews { get; set; }
        public List<string> nameCandidates { get; set; }

        private Dictionary<int, List<InterviewInfo>> interviewsByCandidateId; 

        public FindCandidate()
        {
            InitializeComponent();
            DataContext = this;
            blclass = new BLConnection();
            Candidates = blclass.GetAllCandidates(); 
            Interviews = blclass.GetAllInterviews();
            nameCandidates = blclass.GetNameCandidate();
            Employees = new List<Employee>(blclass.GetAllEmployees());

            interviewsByCandidateId = Interviews
                .GroupBy(i => i.CandidateId)
                .ToDictionary(g => g.Key, g => g.ToList().Select(i => new InterviewInfo
                {
                    InterviewNumber = i.InterviewNumber,
                    RoleInCompany = i.RoleInCompany,
                    InterviewDate = i.InterviewDate,
                    phonInterviewer = Employees.FirstOrDefault(e => e.Id == i.InterviewerId)?.PhoneNumber,
                    NameInterviewer = Employees.FirstOrDefault(e => e.Id == i.InterviewerId)?.FirstName + " " + Employees.FirstOrDefault(e => e.Id == i.InterviewerId)?.LastName
                }).OrderBy(i => i.InterviewDate).ToList());

            //FilteredInterviews = new ObservableCollection<Interview>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<InterviewInfo> FilteredInterviews { get; set; } = new ObservableCollection<InterviewInfo>();

        public class InterviewInfo
        {
            public int InterviewNumber { get; set; }
            public string RoleInCompany { get; set; }
            public DateTime? InterviewDate { get; set; }
            public string phonInterviewer { get; set; }
            public string NameInterviewer { get; set; }
        }
      
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCandidateName = (sender as ComboBox).SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedCandidateName))
            {
                var selectedCandidate = Candidates.FirstOrDefault(i => $"{i.FirstName} {i.LastName}" == selectedCandidateName); 

                if (selectedCandidate != null)
                {
                   
                    if (interviewsByCandidateId.TryGetValue(selectedCandidate.Id, out var candidateInterviewsInfo))
                    {
                        FilteredInterviews = new ObservableCollection<InterviewInfo>(candidateInterviewsInfo);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilteredInterviews)));
                    }
                }
            }
            
        }
    }


}
