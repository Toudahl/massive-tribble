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
        public IssueHandler IssueHandler { get; set; }
        public ObservableCollection<IssueModel> AllIssues { get; set; }

        public  IssuesViewModel()
        {
            IssueHandler = new IssueHandler();
          AllIssues=  IssueHandler.GetAllIssues().Result.ToObservableCollection();
        }

    }
}
