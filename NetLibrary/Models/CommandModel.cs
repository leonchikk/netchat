using NetLibrary.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetLibrary.Models
{
    public class CommandModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private CommandTypes _commandType;
        /// <summary>
        /// Command type which will send to server
        /// </summary>
        public CommandTypes CommandType
        {
            get
            {
                return _commandType;
            }
            set
            {
                _commandType = value;
                OnPropertyChanged("CommandType");
            }
        }

        private string _command;
        /// <summary>
        /// Metadata of command which will send to server
        /// </summary>
        public string Command
        {
            get
            {
                return _command;
            }
            set
            {
                _command = value;
                OnPropertyChanged("Command");
            }
        }
    }
}
