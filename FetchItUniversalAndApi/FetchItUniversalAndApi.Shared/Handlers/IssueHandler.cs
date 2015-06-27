using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers
{
    //Author: Jakub Czapski
    /// <summary>
    /// Handles getting and maintaining issues made by users.
    /// Currently only getting issues is used in the app.
    /// 
    /// 
    /// 
    /// </summary>
    public class IssueHandler : ICreate<IssueModel>, IDelete<IssueModel>, IDisable<IssueModel>, ISuspend<IssueModel>, IUpdate<IssueModel>//, ISearch<IssueModel>
    {
        #region Fields and properties
        private IssueModel _newIssue;
        private IssueModel _selectedIssue;
        private IssueHandler _handler;
        private IEnumerable<IssueModel> _currentIssues;
        private string _userInput;
        private ProfileModel _selecteProfile;
        private const string issuemodelurl = "http://fetchit.mortentoudahl.dk/api/IssueModels";
        private ProfileHandler ph;
        private TaskHandler th;
        private ApiLink<IssueModel> apiLink;

        public ProfileModel SelecteProfile
        {
            get { return _selecteProfile; }
            set { _selecteProfile = value; }
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


        #endregion

        #region Enum(s)
        public enum IssueStatus
        {
            Deleted = 6,
            Suspended = 7,
            Disabled = 8,
            Active = 9,
            Unactivated = 10
        }
        #endregion

        public IssueHandler()
        {
            ph = ProfileHandler.GetInstance();
            th = TaskHandler.GetInstance();
            apiLink = new ApiLink<IssueModel>();
        }


        #region Create method
        //Fixed by Morten Toudahl.
        // Now it is checking properly, and setting values correctly before using the new ApiLink class to talk with the api.
        /// <summary>
        /// Creates a issue and adds it to the database
        /// </summary>
        /// <param name="issue">Issue to create.</param>
        /// <returns></returns>
        public async void Create(IssueModel issue)
        {
            if (string.IsNullOrEmpty(issue.IssueTitle) || string.IsNullOrEmpty(issue.IssueDescription))
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(issue.IssueTitle) || string.IsNullOrWhiteSpace(issue.IssueDescription))
            {
                return;
            }
            if (issue.FK_IssueTarget == 0 || issue.FK_IssueTask == 0)
            {
                return;
            }
            issue.FK_IssueCreator = ph.CurrentLoggedInProfile.ProfileId;
            issue.IssueTimeCreated = DateTime.UtcNow;
            issue.FK_IssueTask = th.SelectedTask.TaskId;

            int? target;
            if (th.SelectedTask.FK_TaskMaster == issue.FK_IssueCreator)
            {
                target = th.SelectedTask.FK_TaskFetcher;
            }
            else
            {
                target = th.SelectedTask.FK_TaskMaster;
            }
            if (target != null)
            {
                issue.FK_IssueTarget = (int)target;
            }
            else
            {
                return;
            }
            try
            {
                using (var result = await apiLink.PostAsJsonAsync(issue))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        MessageHandler.SendNotification(_userInput, _selecteProfile);
                    }
                }
            }
            catch (Exception)
            {
                //TODO Handle this exception
                throw;
            }
        }
        #endregion

        #region GetAllIssues method
        /// <summary>
        /// Gets all the issue objects from the databae.
        /// </summary>
        /// <returns></returns>
        public async Task<ObservableCollection<IssueModel>> GetAllIssues()
        {
            try
            {
                using (var result = await apiLink.GetAsync())
                {
                    if (result.IsSuccessStatusCode)
                    {
                        return await result.Content.ReadAsAsync<ObservableCollection<IssueModel>>();
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return null;
        }
        #endregion

          /// <summary>
      /// Sets the selected issues type to deleted in the database
      /// </summary>
      /// <param name="obj">Issue you want to delete.</param>
      /// <returns></returns>
      public void Delete(IssueModel obj)
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
        /// <summary>
        /// Sets the selected issues type to disabled in the database
        /// </summary>
        /// <param name="obj">Issue you want to disable.</param>
        /// <returns></returns>

      public void Disable(IssueModel obj)
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

        /// <summary>
        /// Searches for issues in the database that contains a letter or a word you entered in search bar
        /// </summary>
        /// <param name="obj">Issue that you were looking for or all issues if you dont enter anything.</param>
        /// <returns></returns>
        public async Task<IEnumerable<IssueModel>> Search(IssueModel obj)
        {
            string userinput = _userInput;

            return null; _currentIssues.Where(issue => issue.IssueTitle.Contains(userinput)) ;
            //foreach (IssueModel issueModel in _currentIssues.Where(issue => issue.IssueTitle.Contains(userinput)))
            //{
            //    return issueModel.IssueDetails;
            //}
            //return  _currentIssues;
        }
        /// <summary>
        /// Sets the selected issues type to suspended in the database
        /// </summary>
        /// <param name="obj">Issue you want to suspend.</param>
        /// <returns></returns>
        public void Suspend(IssueModel obj)
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


        /// <summary>
        /// Updates the selected issues info in the database.
        /// </summary>
        /// <param name="obj">Issue you want to update.</param>
        /// <returns></returns>
        public async void Update(IssueModel obj)
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
        /// <summary>
        /// Creates a log using loghandler.
        /// </summary>      
        /// <returns></returns>
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
