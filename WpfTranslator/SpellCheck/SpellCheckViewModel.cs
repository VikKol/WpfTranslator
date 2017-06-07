using System.ComponentModel;
using System.Threading.Tasks;

namespace WpfTranslator
{
    public class SpellCheckViewModel : INotifyPropertyChanged
    {
        private SpellCheckService spellCheckService;

        public SpellCheckViewModel(SpellCheckService spellCheckService)
        {
            this.spellCheckService = spellCheckService;
        }

        private string _inputText;
        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }

        private string _outputText;
        public string OutputText
        {
            get => _outputText;
            set
            {
                _outputText = value;
                OnPropertyChanged(nameof(OutputText));
            }
        }

        public async Task CheckSpellingAsync(string text = null)
        {
            InputText = text ?? InputText;
            OutputText = (await Helpers.SafeExecute(() => spellCheckService.CheckSpellingAsync(InputText)))?.Suggestion;
        }

        public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
        private void OnPropertyChanged(string propertyName) => PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
}