using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;
using FetchItUniversalAndApi.View;

namespace FetchItUniversalAndApi.ViewModel
{
    class IssuesViewModel
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
            set { _selectedIssue = value; }
        }

        public  IssuesViewModel()
        {
            var ph = ProfileHandler.GetInstance();
            IssueHandler = new IssueHandler();
          AllIssues=IssueHandler.GetAllIssues();
            CurrentUsersIssues =
                AllIssues.Where(
                    issue =>
                        issue.IssueCreator == ph.CurrentLoggedInProfile ||
                        issue.IssueTarget == ph.CurrentLoggedInProfile).ToObservableCollection();
            if (ph.CurrentLoggedInProfile.FK_ProfileLevel >= (int)ProfileHandler.ProfileLevel.Administrator)
            {
                IssuesToDisplay = AllIssues;
                WelcomeText = "Welcome " + ph.CurrentLoggedInProfile.ProfileName + ", here are all the current issues";

            }
            else
            {
                IssuesToDisplay = CurrentUsersIssues;
                WelcomeText = "Welcome " + ph.CurrentLoggedInProfile.ProfileName + ", here are all the issues you are involved in";

            }





        }

    }
}
