using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Core.AnimationMetrics;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers
{
    class IssueHandler: ICreate, IDelete, IDisable, ISearch, ISuspend, IUpdate
    {
        private IssueModel _newIssue;
        private IssueModel _selectedIssue;
        private IssueHandler _handler;
        private IEnumerable<IssueModel> _currentIssues;

        public IEnumerable<IssueModel> CurrentIssues
        {
            get { return _currentIssues; }
            set { _currentIssues = value; }
        }

        public IssueHandler Handler
        {
            get { return _handler; }
            set { _handler = value; }
        }

        public IssueModel NewIssue
        {
            get { return _newIssue; }
            set { _newIssue = value; }
        }

        public IssueModel SelectedIssue
        {
            get { return _selectedIssue; }
            set { _selectedIssue = value; }
        }

        public void Create(object obj)
        {
            _handler.Create(_newIssue);
        }

        public void Delete(object obj)
        {
            _handler.Delete(_selectedIssue);
        }

        public void Disable(object obj)
        {
            _handler.Disable(_selectedIssue);
        }

        public IEnumerable<object> Search(object obj)
        {
            string userinput = "test input";

            foreach (IssueModel issueModel in _currentIssues.Where(issue => issue.IssueTitle.Contains(userinput)))
            {
                return issueModel.IssueDetails;
            }
            return _currentIssues;
        }

        public void Suspend(object obj)
        {
            throw new NotImplementedException();
        }

        public void Update(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
