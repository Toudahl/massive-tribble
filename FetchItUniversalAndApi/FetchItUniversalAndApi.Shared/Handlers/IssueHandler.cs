﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.UI.Popups;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers
{
    //Author: Jakub Czapski
    /// <summary>
    /// Handles getting and maintaining issues made by users.
    /// Currently only getting issues is used in the app.
    /// </summary>
    public class IssueHandler : ICreate<IssueModel>, IDelete<IssueModel>, IDisable<IssueModel>, ISuspend<IssueModel>, IUpdate<IssueModel>, ISearch<IssueModel>
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
            if (ph.CurrentLoggedInProfile == null) return;
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
                    if (result != null)
                    {
                        if (result.IsSuccessStatusCode)
                        {
                            MessageHandler.SendNotification(_userInput, _selecteProfile);
                        }
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
        // Updated by Morten Toudahl
        // It now uses the ApiLink class
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
                    if (result != null)
                    {
                        if (result.IsSuccessStatusCode)
                        {
                            return await result.Content.ReadAsAsync<ObservableCollection<IssueModel>>();
                        }
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

        #region Delete method
        // Fixed by Morten Toudahl
        // It now uses the argument being passed to it, instead of an other property.
        /// <summary>
        /// Sets the selected issues type to deleted in the database
        /// </summary>
        /// <param name="issue">Issue you want to delete.</param>
        /// <returns></returns>
        public void Delete(IssueModel issue)
        {
            if (ph.CurrentLoggedInProfile == null) return;
            if (!(ph.CurrentLoggedInProfile.FK_ProfileLevel >= (int) ProfileHandler.ProfileLevel.Administrator)) return;
            issue.FK_IssueStatus = (int)IssueStatus.Deleted;
            Update(issue);
        }
        #endregion

        #region Disable method
        // Fixed by Morten Toudahl
        // It now uses the argument being passed to it, instead of an other property.
        /// <summary>
        /// Sets the selected issues type to disabled in the database
        /// </summary>
        /// <param name="issue">Issue you want to disable.</param>
        /// <returns></returns>
        public void Disable(IssueModel issue)
        {
            if (ph.CurrentLoggedInProfile == null) return;
            if (ph.CurrentLoggedInProfile.FK_ProfileLevel != issue.FK_IssueCreator) return;
            issue.FK_IssueStatus = (int)IssueStatus.Disabled;
            Update(issue);
        }
        #endregion

        #region Search method
        /// <summary>
        /// Searches for issues in the database that contains a letter or a word you entered in search bar
        /// </summary>
        /// <param name="issue">Issue that you were looking for or all issues if you dont enter anything.</param>
        /// <returns></returns>
        public async Task<IEnumerable<IssueModel>> Search(IssueModel issue)
        {
            var issues = await GetAllIssues();

            if (issue.IssueId != 0)
            {
                return issues.Where(i => i.IssueId == issue.IssueId);
            }

            if (!string.IsNullOrEmpty(issue.IssueTitle) || !string.IsNullOrWhiteSpace(issue.IssueTitle))
            {
                return issues.Where(i => i.IssueTitle.Contains(issue.IssueTitle));
            }

            if (!string.IsNullOrEmpty(issue.IssueDescription) || !string.IsNullOrWhiteSpace(issue.IssueDescription))
            {
                return issues.Where(i => i.IssueTitle.Contains(issue.IssueDescription));
            }
            return null;
        }
        #endregion

        #region Suspend method
        // Fixed by Morten Toudahl
        // It now uses the argument being passed to it, instead of an other property.
        /// <summary>
        /// Sets the selected issues type to suspended in the database
        /// </summary>
        /// <param name="issue">Issue you want to suspend.</param>
        /// <returns></returns>
        public void Suspend(IssueModel issue)
        {
            if (ph.CurrentLoggedInProfile == null) return;
            if (!(ph.CurrentLoggedInProfile.FK_ProfileLevel >= (int)ProfileHandler.ProfileLevel.Administrator)) return;
            issue.FK_IssueStatus = (int)IssueStatus.Suspended;
            Update(issue);
        }
        #endregion

        #region Update method
        /// <summary>
        /// Updates the selected issues info in the database.
        /// </summary>
        /// <param name="issue">Issue you want to update.</param>
        /// <returns></returns>
        public async void Update(IssueModel issue)
        {
            try
            {
                using (var result = await apiLink.PutAsJsonAsync(issue, issue.IssueId))
                {
                    if (result != null)
                    {
                        if (result.IsSuccessStatusCode)
                        {
                            await new MessageDialog("Updated").ShowAsync();
                        }
                    }
                }
            }
            catch (Exception)
            {
                //TODO react to exception
                throw;
            }
        }
        #endregion
   
    }

}
