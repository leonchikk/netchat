using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetLibrary.Models
{
    [Serializable()]
    public class ConversationModel: INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public ClientModel _sender;
        /// <summary>
        /// 
        /// </summary>
        public ClientModel Sender
        {
            get
            {
                return _sender;
            }
            set
            {
                _sender = value;
                OnPropertyChanged("Sender");
            }
        }

        public ClientModel _target;
        /// <summary>
        /// 
        /// </summary>
        public ClientModel Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
                OnPropertyChanged("Target");
            }
        }

        public string _message;
        /// <summary>
        /// 
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }
    }
}
