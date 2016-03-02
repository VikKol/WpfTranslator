using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;

namespace WpfTranslator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int WmHotKey = 0x0312;
        private readonly IntPtr wndHandle;
        private readonly Translator translator;

        public MainWindow()
        {
            InitializeComponent();
            translator = new Translator(new AdmStsClient(new AdmStsSettings("","","","")));
            wndHandle = new WindowInteropHelper(this).Handle;
            WinApi.RegisterAppHotKey(wndHandle);
            ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcher_ThreadPreprocessMessage;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            WinApi.UnregisterAppHotKey(wndHandle);
            ComponentDispatcher.ThreadPreprocessMessage -= ComponentDispatcher_ThreadPreprocessMessage;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            base.OnClosing(e);
        }

        private void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            if (!handled && msg.message == WmHotKey && WinApi.AppHotKeyId == (int)msg.wParam)
            {
                MessageBox.Show("Ok");
                HandleTranslation();
                handled = true;
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
        }

        private void HandleTranslation()
        {
        }

        private async void translateBtn_Click(object sender, RoutedEventArgs e)
        {
            var text = translateTxt.Text;
            translatedTxt.Text = await translator.Translate(text, Languages.En, Languages.Uk);
        }

        private void pronounceBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
