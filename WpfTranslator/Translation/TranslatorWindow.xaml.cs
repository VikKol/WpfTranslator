using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace WpfTranslator
{
    /// <summary>
    /// Interaction logic for TranslatorWindow.xaml
    /// </summary>
    public partial class TranslatorWindow : Window
    {
        public bool ShouldClose { get; set; }

        private TranslatorViewModel viewModel;

        public TranslatorWindow(TranslatorViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            this.DataContext = viewModel;
        }

        private async void translateTxt_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await viewModel.TranslateAsync();
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.IsActive && e.Key == Key.Escape)
            {
                ClearAndHideWindow();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = !ShouldClose;
            ClearAndHideWindow();
        }

        private void ClearAndHideWindow()
        {
            this.viewModel.InputText = string.Empty;
            this.viewModel.TranslatedText = string.Empty;
            this.Hide();
        }
    }
}
