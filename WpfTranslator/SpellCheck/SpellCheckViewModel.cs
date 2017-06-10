using System.ComponentModel;

namespace WpfTranslator
{
    public class SpellCheckViewModel : INotifyPropertyChanged
    {
        private string _englishText;
        public string EnglishText
        {
            get => _englishText;
            set
            {
                _englishText = value;
                OnPropertyChanged(nameof(EnglishText));
            }
        }

        private string _russianText;
        public string RussianText
        {
            get => _russianText;
            set
            {
                _russianText = value;
                OnPropertyChanged(nameof(RussianText));
            }
        }

        private string _ukrainianText;
        public string UkrainianText
        {
            get => _ukrainianText;
            set
            {
                _ukrainianText = value;
                OnPropertyChanged(nameof(UkrainianText));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
        private void OnPropertyChanged(string propertyName) => PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
}