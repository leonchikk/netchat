using NetLibrary.Enums;
using Newtonsoft.Json.Linq;
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

        private JObject _data;
        /// <summary>
        /// Metadata of command which will send to server
        /// </summary>
        public JObject Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                OnPropertyChanged("Command");
            }
        }
    }
}
