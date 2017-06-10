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

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (this.IsActive && e.Key == Key.Escape)
            {
                ClearAndHideWindow();
            }
        }

        private void ClearAndHideWindow()
        {
            this.viewModel.EnglishText = string.Empty;
            this.viewModel.RussianText = string.Empty;
            this.viewModel.UkrainianText = string.Empty;
            this.Hide();
        }
    }
}
