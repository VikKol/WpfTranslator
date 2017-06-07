using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace WpfTranslator
{
    /// <summary>
    /// Interaction logic for SpellCheckWindow.xaml
    /// </summary>
    public partial class SpellCheckWindow : Window
    {
        public bool ShouldClose { get; set; }

        private SpellCheckViewModel viewModel;

        public SpellCheckWindow(SpellCheckViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            this.DataContext = viewModel;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = !ShouldClose;
            ClearAndHideWindow();
        }

        private async void input_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await viewModel.CheckSpellingAsync();
            }
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (this.IsActive && e.Key == Key.Escape)
            {
                ClearAndHideWindow();
            }
        }

        private void ClearAndHideWindow()
        {
            this.viewModel.InputText = string.Empty;
            this.viewModel.OutputText = string.Empty;
            this.Hide();
        }
    }
}
