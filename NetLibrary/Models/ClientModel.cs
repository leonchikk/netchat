using NetLibrary.Classes;
using NetLibrary.Enums;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetLibrary.Models
{
    [Serializable()]
    public class ClientModel : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private Guid _id;
        /// <summary>
        /// Client Id
        /// </summary>
        public Guid Id
        {
            get { return _id; }
            set
            {
                _id = value;
            }
        }

        private string _name;
        /// <summary>
        /// Client name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private string _email;
        /// <summary>
        /// Client email
        /// </summary>
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged("Email");
            }
        }

        private bool _isFriend = false;
        /// <summary>
        /// Represent that this user is contact
        /// </summary>
        public bool IsFriend
        {
            get { return _isFriend; }
            set
            {
                _isFriend = value;
                OnPropertyChanged("IsFriend");
            }
        }

        private bool _isApproved = false;
        /// <summary>
        /// Represent that this user is approved contact
        /// </summary>
        public bool IsApproved
        {
            get { return _isApproved; }
            set
            {
                _isApproved = value;
                OnPropertyChanged("IsApproved");
            }
        }

        private bool _isInitiatorToApprove = false;
        /// <summary>
        /// Represent that this user is initiator created contact
        /// </summary>
        public bool IsInitiatorToApprove
        {
            get { return _isInitiatorToApprove; }
            set
            {
                _isInitiatorToApprove = value;
                OnPropertyChanged("IsInitiatorToApprove");
            }
        }

        private UserStates _clientState;
        /// <summary>
        /// Client state
        /// </summary>
        public UserStates ClientState
        {
            get { return _clientState; }
            set
            {
                _clientState = value;
                OnPropertyChanged("ClientState");
            }
        }

        public void Copy(ClientModel model)
        {
            this.Id = model.Id;
            this.Name = model.Name;
            this.Email = model.Email;
            this.IsFriend = model.IsFriend;
            this.IsApproved = model.IsApproved;
            this.IsInitiatorToApprove = model.IsInitiatorToApprove;
            this.ClientState = model.ClientState;
        }
    }
}
