using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers
{
  public  class IssueHandler: ICreate, IDelete, IDisable, ISearch, ISuspend, IUpdate
    {
        private IssueModel _newIssue;
        private IssueModel _selectedIssue;
        private IssueHandler _handler;
        private IEnumerable<IssueModel> _currentIssues;
        private string _userInput;
        private const string issuemodelurl = "http://fetchit.mortentoudahl.dk/api/IssueModels";

        public enum IssueStatus
        {
            Deleted = 3,
            Suspended = 4,
            Disabled = 5,
            Active = 6,
            Unactivated = 7
        }
      public string UserInput
      {
          get { return _userInput; }
          set { _userInput = value; }
      }

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

        public async void Create(object obj)
        {
            if (obj is IssueModel)
            {
                
                
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("appliaction/json"));
                        await client.PostAsJsonAsync(issuemodelurl, obj);
                    }
                
            }
            else
            {
                CreateLog();
              }

           
        }

        public void Delete(object obj)
        {
            
            if (obj is IssueModel)
            {
                _selectedIssue.FK_IssueStatus = (int)IssueStatus.Deleted;
                Update(_selectedIssue);
            }
            else
            {
                CreateLog();
            }
        }

        public void Disable(object obj)
        {
            if (obj is IssueModel)
            {
                _selectedIssue.FK_IssueStatus = (int)IssueStatus.Disabled;
                Update(_selectedIssue);
            }
            else
            {
                    CreateLog();                
            }
        }

        public IEnumerable<object> Search(object obj)
        {
            string userinput = _userInput;

            foreach (IssueModel issueModel in _currentIssues.Where(issue => issue.IssueTitle.Contains(userinput)))
            {
                return issueModel.IssueDetails;
            }
            return _currentIssues;
        }

        public  void Suspend(object obj)
        {
            if (obj is IssueModel)
            {
                _selectedIssue.FK_IssueStatus = (int)IssueStatus.Suspended;
                Update(_selectedIssue);
            }
            else
            {
           CreateLog(); 
            }
        }

        public async void Update(object obj)
        {
            if (obj is IssueModel)
            {

                
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(issuemodelurl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    try
                    {
                        await client.PutAsJsonAsync("IssueModels/"+_selectedIssue.IssueId,_selectedIssue);

                    }
                    catch (Exception)
                    {
                        
                    }
                }
            }
        }

      public void CreateLog()
      {
          LogModel logModel = new LogModel
          {
              LogMessage = "The supplied model was not of the expected type",
              LogTime = DateTime.UtcNow
          };
          var lh = LogHandler.GetInstance();
          lh.Create(logModel);
      }

     
    }


    public class WrongModel : Exception
    {
        public WrongModel()
        {
            
        }
        public WrongModel(string message) :base(message)
        {

        }

        public WrongModel(string message, Exception inner):base(message,inner)
        {
            
        }

    }
    
    
}
