
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Client.Engine.Models
{
    public class CurrentAppState : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private bool _isSearchMode = false;
        /// <summary>
        /// Is search mode enabled or not
        /// </summary>
        public bool IsSearchMode
        {
            get { return _isSearchMode; }
            set
            {
                _isSearchMode = value;
                OnPropertyChanged("IsSearchMode");
            }
        }
    }
}
