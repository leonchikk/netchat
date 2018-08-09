using NetLibrary.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Client.Engine.Models
{
    public class CurrentConversation: INotifyPropertyChanged
    {
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private ClientModel _interlocutor = new ClientModel();
        /// <summary>
        /// Current interlocutor
        /// </summary>
        public ClientModel Interlocutor
        {
            get
            {
                return _interlocutor;
            }
            set
            {
                _interlocutor = value;
                OnPropertyChanged("Interlocutor");
            }
        }

        private bool _isSelected = false;
        /// <summary>
        /// Is user selected interlocutor for conversation
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        private bool _isStarted = false;
        /// <summary>
        /// Is conversation started
        /// </summary>
        public bool IsStarted
        {
            get
            {
                return _isStarted;
            }
            set
            {
                _isStarted = value;
                OnPropertyChanged("IsStarted");
            }
        }
    }
}
