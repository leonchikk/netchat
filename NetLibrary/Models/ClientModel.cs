using NetLibrary.Classes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetLibrary.Models
{
    [Serializable()]
    public class ClientModel : INotifyPropertyChanged
    {
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
                OnPropertyChanged("Id");
            }
        }

        private string _ip;
        /// <summary>
        /// Client Ip
        /// </summary>
        public string Ip
        {
            get { return _ip;}
            set
            {
                _ip = value;
                OnPropertyChanged("Ip");
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

        private string _login;
        /// <summary>
        /// Client login
        /// </summary>
        public string Login
        {
            get { return _login; }
            set
            {
                _login = value;
                OnPropertyChanged("Login");
            }
        }

        private UserState _clientState;
        /// <summary>
        /// Client state
        /// </summary>
        public UserState ClientState
        {
            get { return _clientState; }
            set
            {
                _clientState = value;
                OnPropertyChanged("ClientState");
            }
        }
    }
}
