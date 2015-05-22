using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.ViewModel
{
    class IssuesViewModel
    {
        private IssueModel _selectedIssue;
        public IssueHandler IssueHandler { get; set; }
        public ObservableCollection<IssueModel> AllIssues { get; set; }

        public IssueModel SelectedIssue
        {
            get { return _selectedIssue; }
            set { _selectedIssue = value; }
        }

        public  IssuesViewModel()
        {
            IssueHandler = new IssueHandler();
          AllIssues=IssueHandler.GetAllIssues();
            
        }

    }
}
