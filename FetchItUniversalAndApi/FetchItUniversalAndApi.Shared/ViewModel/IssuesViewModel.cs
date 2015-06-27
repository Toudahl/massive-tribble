using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using FetchItUniversalAndApi.Annotations;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.ViewModel
{
    class IssuesViewModel:INotifyPropertyChanged
        // Author: Jakub Czapski
    {
        private IssueModel _selectedIssue;
        public IssueHandler IssueHandler { get; set; }
        public  ObservableCollection<IssueModel> AllIssues { get; set; }
        public string WelcomeText { get; set; }
        public ObservableCollection<IssueModel> CurrentUsersIssues { get; set; }

        public ObservableCollection<IssueModel> IssuesToDisplay { get; set; }

        public IssueModel SelectedIssue
        {
            get { return _selectedIssue; }
            set
            {
                _selectedIssue = value;
                OnPropertyChanged("SelectedIssue");
            }
        }

        public  IssuesViewModel()
        {
            var ph = ProfileHandler.GetInstance();
            IssueHandler = new IssueHandler();
            PopulateAllIssues();
            IssuesToDisplay = new ObservableCollection<IssueModel>();
            if (ph.CurrentLoggedInProfile.FK_ProfileLevel >= (int)ProfileHandler.ProfileLevel.Administrator)
            {
                IssuesToDisplay = AllIssues;
                WelcomeText = "Welcome " + ph.CurrentLoggedInProfile.ProfileName + ", here are all the current issues";
            }
            else
            {
                CurrentUsersIssues =
                                AllIssues.Where(
                                    issue =>
                                    issue.FK_IssueCreator == ph.CurrentLoggedInProfile.ProfileId ||
                                    issue.FK_IssueTarget == ph.CurrentLoggedInProfile.ProfileId).ToObservableCollection();

                IssuesToDisplay = CurrentUsersIssues;
                WelcomeText = "Welcome " + ph.CurrentLoggedInProfile.ProfileName + ", here are all the issues you are involved in";

            }

        }

        private async void PopulateAllIssues()
        {
            var issues = await IssueHandler.GetAllIssues();
                AllIssues = new ObservableCollection<IssueModel>(issues);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
