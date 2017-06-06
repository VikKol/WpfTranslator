using System.ComponentModel;

namespace WpfTranslator
{
    public class Language : INotifyPropertyChanged
    {
        private string _code;
        private string _name;

        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                OnPropertyChanged(nameof(Code));
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string DisplayName => Code + " - " + Name;

        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };
        private void OnPropertyChanged(string propertyName) => PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
}
