using NetLibrary.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetLibrary.Models
{
    [Serializable]
    public class CommandModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private string _token;
        /// <summary>
        /// Token for some commands (like get contacts, search and ect.)
        /// </summary>
        public string Token
        {
            get
            {
                return _token;
            }
            set
            {
                _token = value;
                OnPropertyChanged("Token");
            }
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

        private StatusCodes _statusCode;
        /// <summary>
        /// Code result exec command which will send to clent
        /// </summary>
        public StatusCodes StatusCode
        {
            get
            {
                return _statusCode;
            }
            set
            {
                _statusCode = value;
                OnPropertyChanged("CommandResponse");
            }
        }

        private string _data;
        /// <summary>
        /// Metadata of command which will send to server/client
        /// Important: can not be null
        /// </summary>
        public string Data
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
