using Client.Engine.Models;
using NetLibrary.Enums;
using Newtonsoft.Json.Linq;
using NetLibrary.EventsArgs;
using NetLibrary.Models;
using System.Windows;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Windows.Controls;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using NetLibrary.Helpers;
using System.Drawing;
using System.IO;
using System;
using System.Drawing.Imaging;
using System.Threading;

namespace Client.Views
{
    public partial class Main : Window
    { 
        /// <summary>
        /// Info about current conversation
        /// </summary>
        private CurrentConversation _currentConversation { get; set; } = new CurrentConversation();

        /// <summary>
        /// Represent all states in current app
        /// </summary>
        private CurrentAppState _currentAppState { get; set; } = new CurrentAppState();

        /// <summary>
        /// Current contacts
        /// </summary>
        private ObservableCollection<ClientModel> _contactsList = new ObservableCollection<ClientModel>();

        /// <summary>
        /// Current search results
        /// </summary>
        private ObservableCollection<ClientModel> _searchResultsList = new ObservableCollection<ClientModel>();

        public Main()
        {
            InitializeComponent();
            this.DataContext = CurrentConnection.CurrentClientInfo = new ClientModel();


            AddToContactButton.DataContext = RemoveContactButton.DataContext =
            InterlocutorNameField.DataContext = MessagePanel.DataContext =
            ApproveContactButton.DataContext = MessagesList.DataContext =
            StartVideoButton.DataContext = StopVideoButton.DataContext =
            VideoField.DataContext = MessageSection.DataContext = _currentConversation;

            SearchResultsList.DataContext =
            ContactList.DataContext = _currentAppState;

            ContactList.ItemsSource = _contactsList;
            SearchResultsList.ItemsSource = _searchResultsList;

            CurrentConnection.CurrentClient.OnReceiveCommandResult += OnReceiveCommandResult;
            CurrentConnection.CurrentClient.OnReceiveNotification += OnReceiveNotification;
            CurrentConnection.CurrentClient.OnReceiveMessage += OnReceiveMessage;
            CurrentConnection.CurrentClient.OnReceiveVideoFrame += OnReceiveVideoFrame;
        }

        private void OnReceiveVideoFrame(object sender, ReceviedVideoFrameEventsArgs e)
        {
            VideoField.Source = e.Frame;
        }

        private async Task GetSearchResultsAsync()
        {
            await CurrentConnection.CurrentClient.SendCommandAsync(CommandTypes.Search, new JObject {
                                                                                                      { "Token", CurrentConnection.ClientToken },
                                                                                                      { "Filter", SearchField.Text }
                                                                                                    }
            );
        }

        private async Task GetContactsAsync()
        {
            await CurrentConnection.CurrentClient.SendCommandAsync(CommandTypes.GetContacts, new JObject {
                                                                                                            { "Token", CurrentConnection.ClientToken },
                                                                                                            { "UserId", CurrentConnection.CurrentClientInfo.Id }
                                                                                                         });
        }

        #region Events
        ///// <summary>
        ///// Raised when take message from other user
        ///// </summary>
        private void OnReceiveMessage(object sender, ReceviedMessageEventsArgs e)
        {
            if (_currentConversation.IsSelected && e.Sender.Id == _currentConversation.Interlocutor.Id)
                MessagesList.Items.Add(e.Message);
        }

        /// <summary>
        /// Raised when take notification from server
        /// </summary>
        private async void OnReceiveNotification(ReceiveNotificationEventsArgs e)
        {
            switch (e.Notification.NotificationType)
            {
                case NotificationTypes.NewContact:
                    
                    await UpdateContainers();

                    UpdateUIWhenSendToApprove();
                    break;

                case NotificationTypes.ApprovedContact:

                    await UpdateContainers();

                    UpdateUIWhenApprove();
                    break;

                case NotificationTypes.RemoveContact:

                    await UpdateContainers();

                    UpdateUIWhenDeletedContact();
                    break;

                case NotificationTypes.StartVideoCall:

                    _currentConversation.IsVideoCallStarted = true;
                    break;

                case NotificationTypes.StopVideoCall:

                    _currentConversation.IsVideoCallStarted = false;
                    break;

                case NotificationTypes.Message:
                    break;
            }
        }

        /// <summary>
        /// Raised when take command results from server
        /// </summary>
        private async void OnReceiveCommandResult(object sender, ReceivedCommandResultsEventsArgs e)
        {
            switch (e.CommandType)
            {
                //Need add messages during error
                case CommandTypes.UserInfo:

                    var response = JObject.Parse(e.CommandResult);

                    if (e.StatusCode == StatusCodes.BadRequest)
                        return;
                    
                    CurrentConnection.ParseToClientModel(response);
                    CurrentConnection.CurrentClient.CurrentUser = CurrentConnection.CurrentClientInfo;

                    await GetContactsAsync();

                    break;

                case CommandTypes.ApproveContact:

                    response = JObject.Parse(e.CommandResult);

                    if (e.StatusCode == StatusCodes.BadRequest)
                        return;

                    UpdateUIWhenApprove();

                    break;

                case CommandTypes.SendToApproveContact:

                    response = JObject.Parse(e.CommandResult);

                    if (e.StatusCode == StatusCodes.BadRequest)
                        return;

                    if (_currentAppState.IsSearchMode)
                        UpdateUIWhenSendToApprove();
                    else
                        UpdateUIWhenDeletedContact();

                    break;

                case CommandTypes.GetContacts:

                    var arrayResponse = JArray.Parse(e.CommandResult);

                    if (e.StatusCode == StatusCodes.BadRequest)
                        return;

                    _currentAppState.IsSearchMode = false;
                    _contactsList.Clear();

                    arrayResponse.ToObject<List<ClientModel>>().ForEach(item => _contactsList.Add(item));

                    break;
                
                case CommandTypes.RemoveContact:

                    response = JObject.Parse(e.CommandResult);

                    if (e.StatusCode == StatusCodes.BadRequest)
                        return;

                    if (_currentAppState.IsSearchMode)
                        UpdateUIWhenSendToApprove();
                    else
                        UpdateUIWhenDeletedContact();

                    break;

                case CommandTypes.Search:

                    arrayResponse = JArray.Parse(e.CommandResult);

                    if (e.StatusCode == StatusCodes.BadRequest)
                        return;
                  
                    _currentAppState.IsSearchMode = true;
                    _searchResultsList.Clear();

                    arrayResponse.ToObject<List<ClientModel>>().ForEach(item => _searchResultsList.Add(item));

                    break;
            }
        }
        #endregion

        #region UI methods
        /// <summary>
        /// Method which updated search results list when user send to approve new contact
        /// </summary>
        private void UpdateUIWhenSendToApprove()
        {
            var searchUser = _searchResultsList.FirstOrDefault(u => u.Id == _currentConversation.Interlocutor.Id);
            if (searchUser == null)
                return;

            _currentConversation.Interlocutor.IsFriend = searchUser.IsFriend = !searchUser.IsFriend;
            _currentConversation.Interlocutor.IsApproved = searchUser.IsApproved = false;
        }

        private void UpdateUIWhenApprove()
        {
            var searchUser = _searchResultsList.FirstOrDefault(u => u.Id == _currentConversation.Interlocutor.Id);
            var contact = _contactsList.FirstOrDefault(c => c.Id == _currentConversation.Interlocutor.Id);

            if (searchUser != null)
                searchUser.IsApproved = true;

            if (contact != null)
                contact.IsApproved = true;

            _currentConversation.Interlocutor.IsApproved = true;
        }

        private void UpdateUIWhenDeletedContact()
        {
            var contactUser = _contactsList.FirstOrDefault(u => u.Id == _currentConversation.Interlocutor.Id);
            if (contactUser == null)
                return;

            _contactsList.Remove(contactUser);

            _currentConversation.IsSelected = false;
            _currentConversation.IsVideoCallStarted = false;
        }

        private async Task UpdateContactsAsync()
        {
            await GetContactsAsync();
        }

        private async Task UpdateSearchAsync()
        {
            await GetSearchResultsAsync();
        }

        private async Task UpdateContainers()
        {
            if (_currentAppState.IsSearchMode)
            {
                await UpdateSearchAsync();
            }
            else
            {
                await UpdateContactsAsync();
            }
        }
        #endregion

        #region UI events

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await CurrentConnection.CurrentClient.SendCommandAsync(CommandTypes.UserInfo, new JObject { { "Token", CurrentConnection.ClientToken } });
        }

        private void Lists_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ClientModel selectedUser = null;

            if (_currentAppState.IsSearchMode)
                selectedUser = SearchResultsList.SelectedItem as ClientModel;
            else
                selectedUser = ContactList.SelectedItem as ClientModel;

            if (selectedUser == null)
                return;

            _currentConversation.Interlocutor.Copy(selectedUser);
            _currentConversation.IsSelected = true;
        }

        private async void SearchField_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(SearchField.Text))
                await GetSearchResultsAsync();
        }

        private async void SearchField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchField.Text))
            {
                _currentAppState.IsSearchMode = false;
                await GetContactsAsync();
            }
        }

        private async void AddToContactButton_Click(object sender, RoutedEventArgs e)
        {
            JObject data = new JObject {
                { "Token", CurrentConnection.ClientToken },
                { "InitiatorId", CurrentConnection.CurrentClientInfo.Id },
                { "TargetId", _currentConversation.Interlocutor.Id }
            };
            _currentConversation.Interlocutor.IsInitiatorToApprove = false; 

            await CurrentConnection.CurrentClient.SendCommandAsync(CommandTypes.SendToApproveContact, data);
        }

        private async void RemoveContactButton_Click(object sender, RoutedEventArgs e)
        {
            JObject data = new JObject {
                { "Token", CurrentConnection.ClientToken },
                { "InitiatorId", CurrentConnection.CurrentClientInfo.Id },
                { "TargetId", _currentConversation.Interlocutor.Id }
            };

            await CurrentConnection.CurrentClient.SendCommandAsync(CommandTypes.RemoveContact, data);
        }

        private async void ApproveContactButton_Click(object sender, RoutedEventArgs e)
        {
            JObject data = new JObject {
                { "Token", CurrentConnection.ClientToken },
                { "InitiatorId", CurrentConnection.CurrentClientInfo.Id },
                { "TargetId", _currentConversation.Interlocutor.Id }
            };

            await CurrentConnection.CurrentClient.SendCommandAsync(CommandTypes.ApproveContact, data);
        }

        private async void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            MessagesList.Items.Add(MessageField.Text);

            await CurrentConnection.CurrentClient.SendMessageAsync(MessageField.Text, _currentConversation.Interlocutor);
        }

        private async void StartVideoButton_Click(object sender, RoutedEventArgs e)
        {
            await CurrentConnection.CurrentClient.StartSendVideoAsync(_currentConversation.Interlocutor);
        }

        private async  void StopVideoButton_Click(object sender, RoutedEventArgs e)
        {
            await CurrentConnection.CurrentClient.StopSendVideoAsync();
        }
        #endregion
    }
}
