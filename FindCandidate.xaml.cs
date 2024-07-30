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
    /// <summary>
    /// Interaction logic for FindCandidate.xaml
    /// </summary>
    //public partial class FindCandidate : Window, INotifyPropertyChanged
    //{
    //    public Candidate SelectedCandidate { get; set; }
    //    public string SelectedCandidate1 { get; set; }
    //    public List<Employee> Employees { get; set; }

    //    public BLConnection blclass { get; set; }
    //    public List<Candidate> Candidate { get; set; }
    //    public List<Interview> Interviews { get; set; }
    //    public List<string> nameCandidates { get; set; }
    //    public FindCandidate()
    //    {
    //        InitializeComponent();
    //        DataContext = this;
    //        blclass = new BLConnection();
    //        Candidate = blclass.GetAllCandidates();
    //        Interviews = blclass.GetAllInterviews();
    //        nameCandidates = blclass.GetNameCandidate();
    //        Employees = new List<Employee>(blclass.GetAllEmployees());

    //        //FilteredInterviews = new ObservableCollection<Interview>();
    //    }

    //    //public ObservableCollection<Interview> FilteredInterviews { get; set; }

    //    public event PropertyChangedEventHandler? PropertyChanged;

    //    public ObservableCollection<InterviewInfo> FilteredInterviews { get; set; } = new ObservableCollection<InterviewInfo>();

    //    public class InterviewInfo
    //    {
    //        public int InterviewNumber { get; set; }
    //        public string RoleInCompany { get; set; }
    //        public DateTime? InterviewDate { get; set; }
    //        public string phonInterviewer { get; set; }
    //        public string NameInterviewer { get; set; }

    //    }

    //    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    //    {
    //        var SelectedCandidate = (sender as ComboBox).SelectedItem;
    //        if (SelectedCandidate != null)
    //        {
    //            var list = Candidate.Where(i => i.FirstName + " " + i.LastName == SelectedCandidate.ToString()).ToList();
    //            if (list != null && list.Count > 0)
    //            {
    //                var objectid = list[0];
    //                if (objectid != null)
    //                {

    //                    // יצירת רשימה חדשה שתכיל את מספרי הראיונות ואת התפקידים
    //                    var FilteredInterviewInfos = Interviews
    //                        .Where(i => i.CandidateId == objectid.Id)
    //                        .Select(i => new InterviewInfo
    //                        {
    //                            InterviewNumber = i.InterviewNumber,
    //                            RoleInCompany = i.RoleInCompany,
    //                            InterviewDate = i.InterviewDate,
    //                            phonInterviewer = Employees.Where(e => e.Id == i.InterviewerId).Select(e => e.PhoneNumber).FirstOrDefault(),
    //                            NameInterviewer = Employees.Where(e => e.Id == i.InterviewerId).Select(e => e.FirstName + " " + e.LastName).FirstOrDefault()

    //                        })
    //                        .ToList();

    //                    // אם אתה משתמש בObservableCollection, תוכל לעדכן את התצוגה
    //                    FilteredInterviews.Clear();
    //                    foreach (var info in FilteredInterviewInfos)
    //                    {
    //                        FilteredInterviews.Add(info);
    //                    }

    //                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FilteredInterviews"));
    //                }
    //            }
    //        }
    //    }


    //}


    public partial class FindCandidate : Window, INotifyPropertyChanged
    {
        public Candidate SelectedCandidate { get; set; }
        public string SelectedCandidate1 { get; set; }
        public List<Employee> Employees { get; set; }

        public BLConnection blclass { get; set; }
        public List<Candidate> Candidates { get; set; } // שיניתי ל-Candidates
        public List<Interview> Interviews { get; set; }
        public List<string> nameCandidates { get; set; }

        private Dictionary<int, List<InterviewInfo>> interviewsByCandidateId; // הכרזת ה-Dictionary

        public FindCandidate()
        {
            InitializeComponent();
            DataContext = this;
            blclass = new BLConnection();
            Candidates = blclass.GetAllCandidates(); // שמירה על השם הנכון
            Interviews = blclass.GetAllInterviews();
            nameCandidates = blclass.GetNameCandidate();
            Employees = new List<Employee>(blclass.GetAllEmployees());

            // שלב 1: מילוי ה- Dictionary לאחר שהמידע זמין
            interviewsByCandidateId = Interviews
                .GroupBy(i => i.CandidateId)
                .ToDictionary(g => g.Key, g => g.ToList().Select(i => new InterviewInfo
                {
                    InterviewNumber = i.InterviewNumber,
                    RoleInCompany = i.RoleInCompany,
                    InterviewDate = i.InterviewDate,
                    phonInterviewer = Employees.FirstOrDefault(e => e.Id == i.InterviewerId)?.PhoneNumber,
                    NameInterviewer = Employees.FirstOrDefault(e => e.Id == i.InterviewerId)?.FirstName + " " + Employees.FirstOrDefault(e => e.Id == i.InterviewerId)?.LastName
                }).ToList());

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
                var selectedCandidate = Candidates.FirstOrDefault(i => $"{i.FirstName} {i.LastName}" == selectedCandidateName); // השתמש ב-Candidates

                if (selectedCandidate != null)
                {
                    // שלב 3: קבלת הראיונות מהמילון
                    if (interviewsByCandidateId.TryGetValue(selectedCandidate.Id, out var candidateInterviewsInfo))
                    {
                        //var filteredInterviewInfos = candidateInterviews.Select(i => new InterviewInfo
                        //{
                        //    InterviewNumber = i.InterviewNumber,
                        //    RoleInCompany = i.RoleInCompany,
                        //    InterviewDate = i.InterviewDate,
                        //    phonInterviewer = Employees.FirstOrDefault(e => e.Id == i.InterviewerId)?.PhoneNumber,
                        //    NameInterviewer = Employees.FirstOrDefault(e => e.Id == i.InterviewerId)?.FirstName + " " + Employees.FirstOrDefault(e => e.Id == i.InterviewerId)?.LastName
                        //});

                        FilteredInterviews = new ObservableCollection<InterviewInfo>(candidateInterviewsInfo);
                        //FilteredInterviews.Clear();
                        //foreach (var info in filteredInterviewInfos)
                        //{
                        //    FilteredInterviews.Add(info);
                        //}

                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilteredInterviews)));
                    }
                }
            }
            
        }
    }


}
